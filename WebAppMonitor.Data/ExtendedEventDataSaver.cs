using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.Data.Entities;
using Z.Dapper.Plus;
using Z.EntityFramework.Plus;

namespace WebAppMonitor.Data
{

	public class ExtendedEventDataSaver: IExtendedEventDataSaver
	{

		private readonly IDbConnectionProvider _connectionProvider;
		private Dictionary<byte[], Guid> _normQueryMap;
		private readonly List<NormQueryTextHistory> _pendingQueries = new List<NormQueryTextHistory>();
		private readonly List<LongLocksInfo> _pendingLocksInfo = new List<LongLocksInfo>();
		private readonly SHA512 _hasher = SHA512.Create();
		private readonly SimpleLookupManager<LockingMode> _lockModeRepository;
		private readonly QueryStatsContext _queryStatsContext;
		
		public ExtendedEventDataSaver(IDbConnectionProvider connectionProvider, QueryStatsContext queryStatsContext) {
			_connectionProvider = connectionProvider;
			_queryStatsContext = queryStatsContext;
			InitDapperPlus();
			_lockModeRepository = new SimpleLookupManager<LockingMode>(connectionProvider);
		}

		private void InitDapperPlus() {
			DapperPlusManager.Entity<NormQueryTextHistory>()
				.Table("NormQueryTextHistory")
				.Identity(x => x.Id);
			DapperPlusManager.Entity<LongLocksInfo>()
				.Table("LongLocksInfo")
				.Identity(x => x.Id);
		}

		private Dictionary<byte[], Guid> NormQueryMap => _normQueryMap ?? InitNormQueryMap();

		private Dictionary<byte[], Guid> InitNormQueryMap() {
			var  result = new Dictionary<byte[], Guid>();
			_connectionProvider.GetConnection(connection => {
				var histories = connection.Query<NormQueryTextHistory>(@"SELECT Id, QueryHash FROM NormQueryTextHistory");
				foreach (NormQueryTextHistory queryTextHistory in histories) {
					result[queryTextHistory.QueryHash] = queryTextHistory.Id;
				}
			});
			_normQueryMap = result;
			return result;
		}

		internal byte[] GetQueryHash(string query) {
			return _hasher.ComputeHash(Encoding.UTF8.GetBytes(query));
		}

		private Guid RegisterQueryText(string text) {
			var hash = GetQueryHash(text);
			if (!NormQueryMap.ContainsKey(hash)) {
				var historyItem = new NormQueryTextHistory {
					Id = new Guid(),
					QueryHash = hash,
					NormalizedQuery = text
				};
				PushQueryItem(historyItem);
				NormQueryMap[hash] = historyItem.Id;
			}
			return NormQueryMap[hash];
		}

		private void PushQueryItem(NormQueryTextHistory historyItem) {
			_pendingQueries.Add(historyItem);
			if (_pendingQueries.Count > 500) {
				SavePendingQueryTextItems();
			}
		}

		private void SavePendingQueryTextItems() {
			_connectionProvider.GetConnection(connection => {
				connection.BulkInsert(_pendingQueries);
			});
			_pendingQueries.Clear();
		}

		private List<Date> _dates;
		private int GetDayId(DateTime dateTime) {
			if (_dates == null) {
				_dates = _queryStatsContext.Dates.ToList();
			}
			DateTime currentDate = dateTime.Date;
			Date foundDate = _dates.First(d => d.DateValue == currentDate);
			if (foundDate != null) {
				return foundDate.Id;
			}
			foundDate = new Date {
				DateValue = currentDate
			};
			_dates.Add(foundDate);
			_queryStatsContext.Dates.Add(foundDate);
			_queryStatsContext.SaveChanges();
			return foundDate.Id;
		}

		public void RegisterLock(QueryLockInfo lockInfo) {
			Guid blockedTextId = RegisterQueryText(lockInfo.Blocked.Text);
			Guid blockerTextId = RegisterQueryText(lockInfo.Blocker.Text);
			Guid lockingMode = _lockModeRepository.GetId(lockInfo.LockMode);
			_pendingLocksInfo.Add(new LongLocksInfo {
				Id = Guid.NewGuid(),
				BlockedQueryId = blockedTextId,
				BlockerQueryId = blockerTextId,
				LockingModeId = lockingMode,
				Date = lockInfo.TimeStamp,
				DateId = GetDayId(lockInfo.TimeStamp),
				Duration = Convert.ToInt64(lockInfo.Duration)
			});
		}

		public void Flush() {
			SavePendingQueryTextItems();
			_connectionProvider.GetConnection(connection => {
				connection.BulkInsert(_pendingLocksInfo);
			});
			_pendingLocksInfo.Clear();
		}
	}
}

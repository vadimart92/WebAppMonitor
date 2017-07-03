using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class ExtendedEventDataSaver : IExtendedEventDataSaver {
		private readonly IDbConnectionProvider _connectionProvider;
		private Dictionary<byte[], Guid> _normQueryMap;
		private readonly List<NormQueryTextHistory> _pendingQueries = new List<NormQueryTextHistory>();
		private readonly List<NormQueryTextSource> _pendingQuerySources = new List<NormQueryTextSource>();
		private readonly List<LongLocksInfo> _pendingLocksInfo = new List<LongLocksInfo>();
		private readonly List<DeadLocksInfo> _pendingDeadLocksInfo = new List<DeadLocksInfo>();
		private readonly SHA512 _hasher = SHA512.Create();
		private readonly SimpleLookupManager<LockingMode> _lockModeRepository;
		private readonly SimpleLookupManager<QuerySource> _querySourceRepository;
		private readonly QueryStatsContext _queryStatsContext;

		public ExtendedEventDataSaver(IDbConnectionProvider connectionProvider, QueryStatsContext queryStatsContext) {
			_connectionProvider = connectionProvider;
			_queryStatsContext = queryStatsContext;
			_lockModeRepository = new SimpleLookupManager<LockingMode>(connectionProvider);
			_querySourceRepository = new SimpleLookupManager<QuerySource>(connectionProvider);
		}

		private Dictionary<byte[], Guid> NormQueryMap => _normQueryMap ?? InitNormQueryMap();

		private Dictionary<byte[], Guid> InitNormQueryMap() {
			var result = new Dictionary<byte[], Guid>(ByteArrayComparer.Instance);
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
			var bytes = query != null ? Encoding.UTF8.GetBytes(query) : new byte[0];
			return _hasher.ComputeHash(bytes);
		}

		private Guid RegisterQueryText(string text, ResettableLazy<Guid> querySourceId) {
			var hash = GetQueryHash(text);
			if (NormQueryMap.ContainsKey(hash)) {
				return NormQueryMap[hash];
			}
			var historyItem = new NormQueryTextHistory {
				Id = Guid.NewGuid(),
				QueryHash = hash,
				NormalizedQuery = text
			};
			PushQueryItem(historyItem, querySourceId.Value);
			NormQueryMap[hash] = historyItem.Id;
			return historyItem.Id;
		}

		private void PushQueryItem(NormQueryTextHistory historyItem, Guid querySourceId) {
			_pendingQueries.Add(historyItem);
			_pendingQuerySources.Add(new NormQueryTextSource {
				QuerySourceId = querySourceId,
				NormQueryTextId = historyItem.Id
			});
			if (_pendingQueries.Count > 500) {
				SavePendingQueryTextItems();
			}
		}


		private List<Date> _dates;

		private int GetDayId(DateTime dateTime) {
			if (_dates == null) {
				_dates = _queryStatsContext.Dates.ToList();
			}
			DateTime currentDate = dateTime.Date;
			Date foundDate = _dates.FirstOrDefault(d => d.DateValue == currentDate);
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
			if (lockInfo.TimeStamp < LastQueryDate.Value) {
				return;
			}
			Guid blockedTextId = RegisterQueryText(lockInfo.Blocked.Text, LongLocksQuerySourceId);
			Guid blockerTextId = RegisterQueryText(lockInfo.Blocker.Text, LongLocksQuerySourceId);
			Guid lockingMode = _lockModeRepository.GetId(lockInfo.LockMode);
			var locksInfo = InitLockInfo<LongLocksInfo>(lockInfo.TimeStamp);
			locksInfo.BlockedQueryId = blockedTextId;
			locksInfo.BlockerQueryId = blockerTextId;
			locksInfo.LockingModeId = lockingMode;
			locksInfo.Duration = lockInfo.Duration;
			_pendingLocksInfo.Add(locksInfo);
		}
		
		private ResettableLazy<DateTime> LastQueryDate => new ResettableLazy<DateTime>(GetLastQueryDate<LongLocksInfo>);
		private ResettableLazy<DateTime> LastDeadLockDate => new ResettableLazy<DateTime>(GetLastQueryDate<DeadLocksInfo>);
		private ResettableLazy<Guid> LongLocksQuerySourceId => new ResettableLazy<Guid>(() => _querySourceRepository.GetId("LongLocks"));
		private ResettableLazy<Guid> DeadLocksQuerySourceId => new ResettableLazy<Guid>(() => _querySourceRepository.GetId("DeadLocks"));

		private DateTime GetLastQueryDate<T>() where T : class {
			string tableName = OrmUtils.GetTableName<T>();
			DateTime result = DateTime.MinValue;
			_connectionProvider.GetConnection(connection => {
				result = connection.ExecuteScalar<DateTime>($"SELECT MAX(Date) FROM [{tableName}]");
			});
			return result;
		}
		
		

		private void SavePendingQueryTextItems() {
			_pendingQueries.BinaryBulkInsert(_connectionProvider);
			_pendingQuerySources.BulkInsert(_connectionProvider);
			_pendingQueries.Clear();
			_pendingQuerySources.Clear();
		}

		public void Flush() {
			SavePendingQueryTextItems();
			_pendingLocksInfo.BulkInsert(_connectionProvider);
			_pendingDeadLocksInfo.BulkInsert(_connectionProvider);
			_pendingLocksInfo.Clear();
			LastQueryDate.Reset();
			LastQueryDate.Reset();
		}

		private T InitLockInfo<T>(DateTime timeStamp) where T : BaseLockInfo, new() {
			return new T {
				Id = Guid.NewGuid(),
				Date = timeStamp,
				DateId = GetDayId(timeStamp)
			};
		}

		public void RegisterDeadLock(QueryDeadLockInfo lockInfo) {
			if (lockInfo.TimeStamp < LastDeadLockDate.Value) {
				return;
			}
			Guid blockedTextId = RegisterQueryText(lockInfo.QueryA, DeadLocksQuerySourceId);
			Guid blockerTextId = RegisterQueryText(lockInfo.QueryB, DeadLocksQuerySourceId);
			var deadLocksInfo = InitLockInfo<DeadLocksInfo>(lockInfo.TimeStamp);
			deadLocksInfo.QueryAId = blockedTextId;
			deadLocksInfo.QueryBId = blockerTextId;
			_pendingDeadLocksInfo.Add(deadLocksInfo);
		}

	}
}
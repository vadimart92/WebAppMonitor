using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class ExtendedEventDataSaver : IExtendedEventDataSaver {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IQueryTextSaver _queryTextSaver;
		private readonly List<LongLocksInfo> _pendingLocksInfo = new List<LongLocksInfo>();
		private readonly List<DeadLocksInfo> _pendingDeadLocksInfo = new List<DeadLocksInfo>();
		private readonly SimpleLookupManager<LockingMode> _lockModeRepository;
		private readonly SimpleLookupManager<QuerySource> _querySourceRepository;
		private readonly QueryStatsContext _queryStatsContext;
		private readonly ResettableLazy<DateTime> LastQueryDate;
		private readonly ResettableLazy<DateTime> LastDeadLockDate;
		private ResettableLazy<Guid> LongLocksQuerySourceId => new ResettableLazy<Guid>(
			() => _querySourceRepository.GetId("LongLocks"));
		private ResettableLazy<Guid> DeadLocksQuerySourceId => new ResettableLazy<Guid>(
			() => _querySourceRepository.GetId("DeadLocks"));
		private List<Date> _dates;

		public ExtendedEventDataSaver(IDbConnectionProvider connectionProvider, QueryStatsContext queryStatsContext,
				IQueryTextSaver queryTextSaver) {
			_connectionProvider = connectionProvider;
			_queryStatsContext = queryStatsContext;
			_queryTextSaver = queryTextSaver;
			_lockModeRepository = new SimpleLookupManager<LockingMode>(connectionProvider);
			_querySourceRepository = new SimpleLookupManager<QuerySource>(connectionProvider);
			LastQueryDate = new ResettableLazy<DateTime>(GetLastQueryDate<LongLocksInfo>);
			LastDeadLockDate = new ResettableLazy<DateTime>(GetLastQueryDate<DeadLocksInfo>);
		}

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

		private DateTime GetLastQueryDate<T>() where T : class {
			string tableName = OrmUtils.GetTableName<T>();
			DateTime result = DateTime.MinValue;
			_connectionProvider.GetConnection(connection => {
				result = connection.ExecuteScalar<DateTime>($"SELECT MAX(Date) FROM [{tableName}]");
			});
			return result;
		}

		private T InitLockInfo<T>(DateTime timeStamp) where T : BaseLockInfo, new() {
			return new T {
				Id = Guid.NewGuid(),
				Date = timeStamp,
				DateId = GetDayId(timeStamp)
			};
		}

		public void RegisterDeadLock(QueryDeadLockInfo lockInfo) {
			if (lockInfo.TimeStamp <= LastDeadLockDate.Value) {
				return;
			}
			Guid blockedTextId = _queryTextSaver.GetOrCreate(lockInfo.QueryA, DeadLocksQuerySourceId.Value);
			Guid blockerTextId = _queryTextSaver.GetOrCreate(lockInfo.QueryB, DeadLocksQuerySourceId.Value);
			var deadLocksInfo = InitLockInfo<DeadLocksInfo>(lockInfo.TimeStamp);
			deadLocksInfo.QueryAId = blockedTextId;
			deadLocksInfo.QueryBId = blockerTextId;
			_pendingDeadLocksInfo.Add(deadLocksInfo);
		}

		public void RegisterLock(QueryLockInfo lockInfo) {
			if (lockInfo.TimeStamp <= LastQueryDate.Value) {
				return;
			}
			Guid blockedTextId = _queryTextSaver.GetOrCreate(lockInfo.Blocked.Text, LongLocksQuerySourceId.Value);
			Guid blockerTextId = _queryTextSaver.GetOrCreate(lockInfo.Blocker.Text, LongLocksQuerySourceId.Value);
			Guid lockingMode = _lockModeRepository.GetId(lockInfo.LockMode);
			var locksInfo = InitLockInfo<LongLocksInfo>(lockInfo.TimeStamp);
			locksInfo.BlockedQueryId = blockedTextId;
			locksInfo.BlockerQueryId = blockerTextId;
			locksInfo.LockingModeId = lockingMode;
			locksInfo.Duration = lockInfo.Duration;
			_pendingLocksInfo.Add(locksInfo);
		}

		public void BeginWork() {
			_queryTextSaver.BeginWork();
		}

		public void Flush() {
			_queryTextSaver.Flush();
			_pendingLocksInfo.BulkInsert(_connectionProvider);
			_pendingDeadLocksInfo.BulkInsert(_connectionProvider);
			_pendingLocksInfo.Clear();
			LastQueryDate.Reset();
			LastDeadLockDate.Reset();
		}
	}
}
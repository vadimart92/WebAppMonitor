using System;
using System.Collections.Generic;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class ExtendedEventDataSaver : IExtendedEventDataSaver {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IQueryTextSaver _queryTextSaver;
		private readonly IDateRepository _dateRepository;
		private readonly List<LongInfoRecord> _pendingLocksInfo = new List<LongInfoRecord>();
		private readonly List<DeadInfoRecord> _pendingDeadLocksInfo = new List<DeadInfoRecord>();
		private readonly SimpleLookupManager<LockingMode> _lockModeRepository;
		private readonly SimpleLookupManager<QuerySource> _querySourceRepository;
		
		private readonly ResettableLazy<DateTime> _lastQueryDate;
		private readonly ResettableLazy<DateTime> _lastDeadLockDate;
		private ResettableLazy<Guid> LongLocksQuerySourceId => new ResettableLazy<Guid>(
			() => _querySourceRepository.GetId("LongLocks"));
		private ResettableLazy<Guid> DeadLocksQuerySourceId => new ResettableLazy<Guid>(
			() => _querySourceRepository.GetId("DeadLocks"));
		

		public ExtendedEventDataSaver(IDbConnectionProvider connectionProvider,
				IQueryTextSaver queryTextSaver, IDateRepository dateRepository) {
			_connectionProvider = connectionProvider;
			_queryTextSaver = queryTextSaver;
			_dateRepository = dateRepository;
			_lockModeRepository = new SimpleLookupManager<LockingMode>(connectionProvider);
			_querySourceRepository = new SimpleLookupManager<QuerySource>(connectionProvider);
			_lastQueryDate = new ResettableLazy<DateTime>(connectionProvider.GetLastQueryDate<LongInfoRecord>);
			_lastDeadLockDate = new ResettableLazy<DateTime>(connectionProvider.GetLastQueryDate<DeadInfoRecord>);
		}

		public void RegisterDeadLock(QueryDeadLockInfo lockInfo) {
			if (lockInfo.TimeStamp <= _lastDeadLockDate.Value) {
				return;
			}
			Guid blockedTextId = _queryTextSaver.GetOrCreate(lockInfo.QueryA, DeadLocksQuerySourceId.Value);
			Guid blockerTextId = _queryTextSaver.GetOrCreate(lockInfo.QueryB, DeadLocksQuerySourceId.Value);
			var deadLocksInfo = _dateRepository.CreateInfoRecord<DeadInfoRecord>(lockInfo.TimeStamp);
			deadLocksInfo.QueryAId = blockedTextId;
			deadLocksInfo.QueryBId = blockerTextId;
			_pendingDeadLocksInfo.Add(deadLocksInfo);
		}

		public void RegisterLock(QueryLockInfo lockInfo) {
			if (lockInfo.TimeStamp <= _lastQueryDate.Value) {
				return;
			}
			Guid blockedTextId = _queryTextSaver.GetOrCreate(lockInfo.Blocked.Text, LongLocksQuerySourceId.Value);
			Guid blockerTextId = _queryTextSaver.GetOrCreate(lockInfo.Blocker.Text, LongLocksQuerySourceId.Value);
			Guid lockingMode = _lockModeRepository.GetId(lockInfo.LockMode);
			var locksInfo = _dateRepository.CreateInfoRecord<LongInfoRecord>(lockInfo.TimeStamp);
			locksInfo.BlockedQueryId = blockedTextId;
			locksInfo.BlockerQueryId = blockerTextId;
			locksInfo.LockingModeId = lockingMode;
			locksInfo.Duration = lockInfo.Duration;
			_pendingLocksInfo.Add(locksInfo);
		}

		public ITransaction BeginWork() {
			return new ActionTransaction(_queryTextSaver.BeginWork, Flush);
		}

		private void Flush() {
			_queryTextSaver.Flush();
			_pendingLocksInfo.BulkInsert(_connectionProvider);
			_pendingLocksInfo.Clear();
			_pendingDeadLocksInfo.BulkInsert(_connectionProvider);
			_pendingDeadLocksInfo.Clear();
			_lastQueryDate.Reset();
			_lastDeadLockDate.Reset();
		}
	}
}
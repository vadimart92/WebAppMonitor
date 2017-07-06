using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class ExtendedEventDataSaver : IExtendedEventDataSaver {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IQueryTextStoringService _queryTextStoringService;
		private readonly IDateRepository _dateRepository;
		private readonly List<LongInfoRecord> _pendingLocksInfo = new List<LongInfoRecord>();
		private readonly List<DeadInfoRecord> _pendingDeadLocksInfo = new List<DeadInfoRecord>();
		private readonly SimpleLookupManager<LockingMode> _lockModeRepository;

		private readonly ILogger _logger;
		private readonly ResettableLazy<DateTime> _lastQueryDate;
		private readonly ResettableLazy<DateTime> _lastDeadLockDate;

		public ExtendedEventDataSaver(IDbConnectionProvider connectionProvider,
				IQueryTextStoringService queryTextStoringService, IDateRepository dateRepository, 
				ILogger<ExtendedEventDataSaver> logger) {
			_connectionProvider = connectionProvider;
			_queryTextStoringService = queryTextStoringService;
			_dateRepository = dateRepository;
			_logger = logger;
			_lockModeRepository = new SimpleLookupManager<LockingMode>(connectionProvider);
			_lastQueryDate = new ResettableLazy<DateTime>(connectionProvider.GetLastQueryDate<LongInfoRecord>);
			_lastDeadLockDate = new ResettableLazy<DateTime>(connectionProvider.GetLastQueryDate<DeadInfoRecord>);
		}

		public void RegisterDeadLock(QueryDeadLockInfo lockInfo) {
			if (lockInfo.TimeStamp <= _lastDeadLockDate.Value) {
				return;
			}
			Guid blockedTextId = _queryTextStoringService.GetOrCreate(lockInfo.QueryA, "DeadLocks");
			Guid blockerTextId = _queryTextStoringService.GetOrCreate(lockInfo.QueryB, "DeadLocks");
			var deadLocksInfo = _dateRepository.CreateInfoRecord<DeadInfoRecord>(lockInfo.TimeStamp);
			deadLocksInfo.QueryAId = blockedTextId;
			deadLocksInfo.QueryBId = blockerTextId;
			_pendingDeadLocksInfo.Add(deadLocksInfo);
		}

		public void RegisterLock(QueryLockInfo lockInfo) {
			if (lockInfo.TimeStamp <= _lastQueryDate.Value) {
				return;
			}
			Guid blockedTextId = _queryTextStoringService.GetOrCreate(lockInfo.Blocked.Text, "LongLocks");
			Guid blockerTextId = _queryTextStoringService.GetOrCreate(lockInfo.Blocker.Text, "LongLocks");
			Guid lockingMode = _lockModeRepository.GetId(lockInfo.LockMode);
			var locksInfo = _dateRepository.CreateInfoRecord<LongInfoRecord>(lockInfo.TimeStamp);
			locksInfo.BlockedQueryId = blockedTextId;
			locksInfo.BlockerQueryId = blockerTextId;
			locksInfo.LockingModeId = lockingMode;
			locksInfo.Duration = lockInfo.Duration;
			_pendingLocksInfo.Add(locksInfo);
		}

		public ITransaction BeginWork() {
			return new ActionTransaction(_queryTextStoringService.BeginWork, Flush);
		}

		private void Flush() {
			_queryTextStoringService.Flush();
			_pendingLocksInfo.BulkInsert(_connectionProvider);
			_logger.LogInformation("{0} long locks inserted.", _pendingDeadLocksInfo.Count);
			_pendingLocksInfo.Clear();
			_pendingDeadLocksInfo.BulkInsert(_connectionProvider);
			_logger.LogInformation("{0} deadloks inserted.", _pendingDeadLocksInfo.Count);
			_pendingDeadLocksInfo.Clear();
			_lastQueryDate.Reset();
			_lastDeadLockDate.Reset();
		}
	}
}
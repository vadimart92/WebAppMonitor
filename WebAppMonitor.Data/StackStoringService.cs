using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;
using WebAppMonitor.DataProcessing;

namespace WebAppMonitor.Data {
	public class StackStoringService : BaseSynchronizedWorker, IStackStoringService{
		private readonly HashStorage<Guid, Stack> _stackMap;
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly List<Stack> _pendingStacks = new List<Stack>();
		private readonly SimpleLookupRepository<StackSource> _stackSourceRepository;

		public StackStoringService(ILogger<StackStoringService> logger, IDbConnectionProvider connectionProvider) : base(logger) {
			_connectionProvider = connectionProvider;
			_stackMap = new HashStorage<Guid, Stack>(item => Tuple.Create(item.StackHash, item.Id),
				@"SELECT Id, StackHash FROM Stack", _connectionProvider);
			_stackSourceRepository = new SimpleLookupRepository<StackSource>(connectionProvider);
		}

		protected override void SaveItems() {
			_pendingStacks.BulkInsert(_connectionProvider);
			Logger.LogInformation("{0} stasks inserted", _pendingStacks.Count);
			_pendingStacks.Clear();
		}

		public Guid GetOrCreate(string stackTrace, string source) {
			EnsureWorkingState();
			return _stackMap.GetOrCreate(stackTrace, hash => {
				var stack = new Stack {
					Id = Guid.NewGuid(),
					SourceId = _stackSourceRepository.GetId(source),
					StackHash = hash,
					StackTrace = stackTrace
				};
				_pendingStacks.Add(stack);
				if (_pendingStacks.Count > 500) {
					SaveItems();
				}
				return stack.Id;
			});
		}

	}
}

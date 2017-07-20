using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class PerfomanceItemCodeStoringService:BaseSynchronizedWorker, IPerfomanceItemCodeStoringService {
		private readonly HashStorage<Guid, PerformanceItemCode> _codeMap;
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly List<PerformanceItemCode> _pendingCodes = new List<PerformanceItemCode>();
		public PerfomanceItemCodeStoringService(ILogger<PerfomanceItemCodeStoringService> logger, IDbConnectionProvider connectionProvider)
			: base(logger) {
			_connectionProvider = connectionProvider;
			_codeMap = new HashStorage<Guid, PerformanceItemCode>(item => Tuple.Create(item.CodeHash, item.Id),
				@"SELECT Id, CodeHash FROM PerformanceItemCode", _connectionProvider);
		}

		protected override void SaveItems() {
			if (_pendingCodes.Count > 0) {
				_pendingCodes.BulkInsert(_connectionProvider, false);
				Logger.LogInformation("{0} codes inserted", _pendingCodes.Count);
				_pendingCodes.Clear();
			}
		}

		public Guid AddCode(string code) {
			return _codeMap.GetOrCreate(code, hash => {
				var itemCode = new PerformanceItemCode {
					Id = Guid.NewGuid(),
					CodeHash = hash,
					Code = code
				};
				_pendingCodes.Add(itemCode);
				if (_pendingCodes.Count > 500) {
					SaveItems();
				}
				return itemCode.Id;
			});
		}

	}
}

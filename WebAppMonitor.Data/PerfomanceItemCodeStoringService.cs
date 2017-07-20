using System;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class PerfomanceItemCodeStoringService : StringItemsStoringService<Guid, PerformanceItemCode>, IPerfomanceItemCodeStoringService {

		public PerfomanceItemCodeStoringService(ILogger<PerfomanceItemCodeStoringService> logger, IDbConnectionProvider connectionProvider) 
			: base(logger, connectionProvider) {
		}

		public Guid AddCode(string code) {
			return GetOrCreate(code, hash => new PerformanceItemCode {
				Id = Guid.NewGuid(),
				HashValue = hash,
				Code = code
			});
		}

		

	}

}

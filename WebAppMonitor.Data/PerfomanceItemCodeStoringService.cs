using System;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Data {
	public class PerfomanceItemCodeStoringService:BaseSynchronizedWorker, IPerfomanceItemCodeStoringService {

		public PerfomanceItemCodeStoringService(ILogger<PerfomanceItemCodeStoringService> logger)
			: base(logger) { }

		protected override void SaveItems() { }

		public Guid AddCode(string code) {
			return new Guid();
		}

	}
}

using System;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Data {
	public class StackSaver : BaseSynchronizedWorker, IStackSaver{

		public StackSaver(ILogger<StackSaver> logger)
			: base(logger) { }

		protected override void OnFlush() {
			
		}

		public Guid GetOrCreate(string stackTrace) {
			
			return new Guid();
		}

	}
}

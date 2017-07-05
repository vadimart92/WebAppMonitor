using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Core.Common {
	public abstract class BaseSynchronizedWorker : ISynchronizedWorker {

		private readonly ILogger _logger;
		private volatile bool _isWorking;
		private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);

		protected BaseSynchronizedWorker(ILogger logger) {
			_logger = logger;
		}

		public virtual void BeginWork() {
			_logger.LogInformation("BeginWork");
			_autoResetEvent.WaitOne();
			_isWorking = true;
		}

		public void Flush() {
			if (!_isWorking) {
				throw new InvalidOperationException();
			}
			OnFlush();
			_isWorking = false;
			_autoResetEvent.Set();
			_logger.LogInformation("Flush complete");
		}

		protected abstract void OnFlush();

		protected void EnsureWorkingState() {
			if (!_isWorking) {
				throw new InvalidOperationException();
			}
		}

	}
}

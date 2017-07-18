using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Core.Common {
	public abstract class BaseSynchronizedWorker : ISynchronizedWorker {

		protected readonly ILogger Logger;
		private volatile bool _isWorking;
		private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);

		protected BaseSynchronizedWorker(ILogger logger) {
			Logger = logger;
		}

		public virtual void BeginWork() {
			Logger.LogDebug("BeginWork");
			_autoResetEvent.WaitOne();
			_isWorking = true;
		}

		public void Flush() {
			if (!_isWorking) {
				throw new InvalidOperationException();
			}
			SaveItems();
			_isWorking = false;
			_autoResetEvent.Set();
			Logger.LogDebug("Flush complete");
		}

		protected abstract void SaveItems();

		protected void EnsureWorkingState() {
			if (!_isWorking) {
				throw new InvalidOperationException();
			}
		}

	}
}

using System;

namespace WebAppMonitor.Core.Import
{
    public interface IStackSaver: ISynchronizedWorker {
		Guid GetOrCreate(string stackTrace);

    }
}

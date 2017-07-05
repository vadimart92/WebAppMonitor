using System;

namespace WebAppMonitor.Core.Import
{
    public interface IStackStoringService: ISynchronizedWorker {
		Guid GetOrCreate(string stackTrace, string source);

    }
}

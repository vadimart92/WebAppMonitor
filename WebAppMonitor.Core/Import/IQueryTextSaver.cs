using System;

namespace WebAppMonitor.Core.Import
{
    public interface IQueryTextSaver: ISynchronizedWorker {
	    Guid GetOrCreate(string queryText, Guid? querySourceId);
	   
    }

}

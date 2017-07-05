using System;

namespace WebAppMonitor.Core.Import
{
    public interface IQueryTextStoringService: ISynchronizedWorker {
	    Guid GetOrCreate(string queryText, string querySource);
	   
    }

}

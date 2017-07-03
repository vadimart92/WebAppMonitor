using System;

namespace WebAppMonitor.Core
{
    public interface IQueryTextSaver {
	    Guid GetOrCreate(string queryText, Guid? querySourceId);
	    void BeginWork();
	    void Flush();
    }
}

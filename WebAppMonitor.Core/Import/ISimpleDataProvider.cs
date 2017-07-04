using System.Collections.Generic;

namespace WebAppMonitor.Core.Import
{
    public interface ISimpleDataProvider {
	    IEnumerable<T> Enumerate<T>(string query, object parameters = null);
    }
}

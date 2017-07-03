using System.Collections.Generic;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core
{
    public interface IExtendedEventParser {
	    IEnumerable<QueryLockInfo> ReadEvents(string file);
    }
}

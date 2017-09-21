using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMonitor.Core
{
	using WebAppMonitor.Core.Import;
    internal class CompositePathProvider:IDataFilePathProvider
    {

	    private readonly IDataFilePathProvider _provider;

	    public CompositePathProvider(IDataFilePathProvider provider) {
		    _provider = provider;
	    }

	    public string ExtendedEventsDirectory { get; set; }

	    public DateTime GetDate() {
		    return _provider.GetDate();
	    }

	    public IEnumerable<string> GetExecutorLogs() {
		    return _provider.GetExecutorLogs();
	    }

	    public IEnumerable<string> GetReaderLogs() {
		    return _provider.GetReaderLogs();
	    }

	    public IEnumerable<string> GetPerfomanceLogs() {
		    return _provider.GetPerfomanceLogs();
	    }

	    public IEnumerable<string> GetDailyExtEventsDirs() {
		    return string.IsNullOrWhiteSpace(ExtendedEventsDirectory)
			    ? _provider.GetDailyExtEventsDirs()
			    : Enumerable.Repeat(ExtendedEventsDirectory, 1);
	    }

    }
}

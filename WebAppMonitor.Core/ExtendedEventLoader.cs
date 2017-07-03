using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core
{
    public class ExtendedEventLoader: IExtendedEventLoader {
	    private readonly IExtendedEventParser _parser;
	    private readonly IExtendedEventDataSaver _dataSaver;
	    public ExtendedEventLoader(IExtendedEventParser parser, IExtendedEventDataSaver dataSaver) {
		    _parser = parser;
		    _dataSaver = dataSaver;
	    }

	    public void LoadLongLocksData(string file) {
		    foreach (QueryLockInfo queryLockInfo in _parser.ReadEvents(file)) {
			    _dataSaver.RegisterLock(queryLockInfo);
			}
			_dataSaver.Flush();
		}
	}
}

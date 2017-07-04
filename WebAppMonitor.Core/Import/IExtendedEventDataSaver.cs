using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core.Import
{
    public interface IExtendedEventDataSaver {
	    void RegisterLock(QueryLockInfo lockInfo);
	    void BeginWork();
	    void Flush();
	    void RegisterDeadLock(QueryDeadLockInfo queryLockInfo);

    }
}

using System;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core
{
    public interface IExtendedEventDataSaver {
	    void RegisterLock(QueryLockInfo lockInfo);
	    void BeginWork();
	    void Flush();
	    void RegisterDeadLock(QueryDeadLockInfo queryLockInfo);

    }
}

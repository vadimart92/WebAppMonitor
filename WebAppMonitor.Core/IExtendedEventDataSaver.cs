using System;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core
{
    public interface IExtendedEventDataSaver {
	    void RegisterLock(QueryLockInfo lockInfo);
	    void Flush();
    }
}

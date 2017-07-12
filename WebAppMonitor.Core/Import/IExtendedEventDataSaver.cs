using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.Core.Import {
	public interface IExtendedEventDataSaver {
		void RegisterLock(QueryLockInfo lockInfo);
		ITransaction BeginWork();
		void RegisterDeadLock(QueryDeadLockInfo queryLockInfo);
	}
}
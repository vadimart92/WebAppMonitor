using System;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Data
{
	public class ExtendedEventDataSaver: IExtendedEventDataSaver
	{
		public void RegisterLock(QueryLockInfo lockInfo) {
			throw new NotImplementedException();
		}

		public void Flush() {
			throw new NotImplementedException();
		}
	}
}

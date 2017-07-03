using System;
using System.Collections;
using System.Collections.Generic;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core
{
    public interface IExtendedEventParser {
	    IEnumerable<QueryLockInfo> ReadLongLockEvents(string file);
	    IEnumerable<QueryDeadLockInfo> ReadDeadLockEvents(string file);

    }

	public class QueryDeadLockInfo
	{

		public DateTime TimeStamp { get; set; }
		public string QueryA { get; set; }
		public string QueryB { get; set; }

	}
}

using System;
using System.Collections.Generic;
using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.Core.Import
{
    public interface IExtendedEventParser {
	    IEnumerable<QueryLockInfo> ReadLongLockEvents(string file);
	    IEnumerable<QueryDeadLockInfo> ReadDeadLockEvents(string file);

    }

	public class QueryDeadLockInfo
	{
		public string ObjectAName { get; set; }
		public string ObjectBName { get; set; }
		public DateTime TimeStamp { get; set; }
		public string QueryA { get; set; }
		public string QueryB { get; set; }

	}
}

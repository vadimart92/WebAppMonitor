using System;

namespace WebAppMonitor.Core.Entities
{

    public class QueryLockInfo
    {
	    public string LockMode { get; set; }
	    public DateTime TimeStamp { get; set; }
	    public ulong Duration { get; set; }
		public Proess Blocker { get; set; }
	    public Proess Blocked { get; set; }
    }

	public class Proess {
		public string Text { get; set; }
	}

	public class BlockedProcess {
		
	}
	public class BlockerProcess {
		
	}
}

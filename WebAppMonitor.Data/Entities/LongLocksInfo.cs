using System;

namespace WebAppMonitor.Data.Entities
{
	public class LongLocksInfo
	{

		public Guid Id { get; set; }
		public Guid BlockedQueryId { get; set; }
		public Guid BlockerQueryId { get; set; }
		public Guid LockingModeId { get; set; }
		public DateTime Date { get; set; }
		public int DateId { get; set; }
		public long Duration { get; set; }
	}
}

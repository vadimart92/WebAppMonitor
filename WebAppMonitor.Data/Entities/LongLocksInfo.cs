using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Data.Entities
{

	public class BaseLockInfo
	{

		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public int DateId { get; set; }
	}
	
	[Table("LongLocksInfo")]
	public class LongLocksInfo : BaseLockInfo
	{
		public Guid BlockedQueryId { get; set; }
		public Guid BlockerQueryId { get; set; }
		public Guid LockingModeId { get; set; }
		public long Duration { get; set; }
	}

	[Table("DeadLocksInfo")]
	public class DeadLocksInfo : BaseLockInfo
	{
		public Guid QueryAId { get; set; }
		public Guid QueryBId { get; set; }
	}
}

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Data.Entities
{
	[Table("LongLocksInfo")]
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

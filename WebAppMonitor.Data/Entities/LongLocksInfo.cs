using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Data.Entities
{

	public class BaseInfoRecord:IRecordWithDate
	{
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public int DateId { get; set; }
	}
	
	[Table("LongLocksInfo")]
	public class LongInfoRecord : BaseInfoRecord
	{
		public Guid BlockedQueryId { get; set; }
		public Guid BlockerQueryId { get; set; }
		public Guid LockingModeId { get; set; }
		public long Duration { get; set; }
	}

	[Table("DeadLocksInfo")]
	public class DeadInfoRecord : BaseInfoRecord
	{
		public Guid QueryAId { get; set; }
		public Guid QueryBId { get; set; }
	}
}

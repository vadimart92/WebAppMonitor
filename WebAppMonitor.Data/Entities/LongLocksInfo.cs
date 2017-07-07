using System;
using Dapper.Contrib.Extensions;

namespace WebAppMonitor.Data.Entities
{

	public class BaseInfoRecord:IRecordWithDate
	{
		[ExplicitKey]
		public Guid Id { get; set; }
		public DateTime Date { get; set; }
		public int DateId { get; set; }
	}
	
	[System.ComponentModel.DataAnnotations.Schema.Table("LongLocksInfo")]
	public class LongInfoRecord : BaseInfoRecord
	{
		public Guid BlockedQueryId { get; set; }
		public Guid BlockerQueryId { get; set; }
		public Guid LockingModeId { get; set; }
		public long Duration { get; set; }
	}

	[System.ComponentModel.DataAnnotations.Schema.Table("DeadLocksInfo")]
	public class DeadLockInfoRecord : BaseInfoRecord
	{
		public Guid QueryAId { get; set; }
		public Guid QueryBId { get; set; }
	}

	[System.ComponentModel.DataAnnotations.Schema.Table("PerfomanceLogInfo")]
	public class PerfomanceLogInfoRecord : BaseInfoRecord
	{
		public Guid ParentId { get; set; }
		public Guid CodeId { get; set; }
		public long Duration { get; set; }
	}
}

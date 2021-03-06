using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Core.Entities {
	[Table("QueryStatInfo")]
	public class QueryStatInfo {
		[Column(TypeName = "date"), Key]
		public DateTime? Date { get; set; }
		public int? DateId { get; set; }
		public decimal? TotalDuration { get; set; }
		public decimal? AvgDuration { get; set; }
		public long? Count { get; set; }
		public decimal? AvgRowCount { get; set; }
		public decimal? AvgLogicalReads { get; set; }
		public decimal? AvgCPU { get; set; }
		public decimal? AvgWrites { get; set; }
		public long? DeadlocksCount { get; set; }
		public long? LockerCount { get; set; }
		public decimal? LockerTotalDuration { get; set; }
		public decimal? LockerAvgDuration { get; set; }
		public long? LockedCount { get; set; }
		public decimal? LockedTotalDuration { get; set; }
		public decimal? LockedAvgDuration { get; set; }
		public string QueryText { get; set; }
		public decimal? ReaderLogsCount { get; set; }
		public decimal? TotalReaderLogsReads { get; set; }
		public decimal? AvgReaderLogsReads { get; set; }
		public long? DistinctReaderLogsStacks { get; set; }
		public decimal? ExecutorLogsCount { get; set; }
		public decimal? TotalExecutorDuration { get; set; }
		public decimal? AvgExecutorDuration { get; set; }
		public long? DistinctExecutorLogsStacks { get; set; }

		[Key]
		public Guid NormalizedQueryTextId { get; set; }
	}

	public class Date {
		public int Id { get; set; }

		[Column("Date", TypeName = "date")]
		public DateTime? DateValue { get; set; }
	}

	public class Setting {
		[Key]
		public int Id { get; set; }
		public string Code { get; set; }
		public string Value { get; set; }

	}

	public class QueryStack {
		public string StackTrace { get; set; }
		public int DateId { get; set; }
		public Guid QueryId { get; set; }
		public Guid StackId { get; set; }
	}

	[Table("VwReaderQueryStack")]
	public class ReaderQueryStack : QueryStack {

	}

	[Table("VwExecutorQueryStack")]
	public class ExecutorQueryStack : QueryStack {

	}
}
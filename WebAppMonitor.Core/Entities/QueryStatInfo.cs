using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Core.Entities {
	[Table("QueryStatInfo")]
	public class QueryStatInfo {
		[Column(TypeName = "date"), Key]
		public DateTime? Date { get; set; }

		public decimal? TotalDuration { get; set; }

		public decimal? AvgDuration { get; set; }

		public decimal? Count { get; set; }

		public decimal? AvgRowCount { get; set; }

		public decimal? AvgLogicalReads { get; set; }

		public decimal? AvgCPU { get; set; }

		public decimal? AvgWrites { get; set; }

		public decimal? AvgAdoReads { get; set; }

		public decimal? LockerCount { get; set; }
		public decimal? LockerTotalDuration { get; set; }
		public decimal? LockerAvgDuration { get; set; }

		public decimal? LockedCount { get; set; }
		public decimal? LockedTotalDuration { get; set; }
		public decimal? LockedAvgDuration { get; set; }

		public string QueryText { get; set; }

		[Key]
		public Guid NormalizedQueryTextId { get; set; }
	}
}
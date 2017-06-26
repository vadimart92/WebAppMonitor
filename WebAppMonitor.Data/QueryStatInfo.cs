namespace WebAppMonitor.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QueryStatInfo")]
    public partial class QueryStatInfo
    {
        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        public decimal? TotalDuration { get; set; }

        public decimal? AvgDuration { get; set; }

        public decimal? Count { get; set; }

        public decimal? AvgRowCount { get; set; }

        public decimal? AvgLogicalReads { get; set; }

        public decimal? AvgCPU { get; set; }

        public decimal? AvgWrites { get; set; }

        public string QueryText { get; set; }

        [Key]
        public Guid NormalizedQueryTextId { get; set; }
    }
}

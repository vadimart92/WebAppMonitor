namespace WebAppMonitor.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("QueryHistory")]
    public partial class QueryHistory
    {
        public Guid Id { get; set; }

        public string username { get; set; }

        public string session_id { get; set; }

        public string client_hostname { get; set; }

        public decimal? duration_sec { get; set; }

        public decimal? cpu_time_sec { get; set; }

        public long? logical_reads { get; set; }

        public long? writes { get; set; }

        public long? row_count { get; set; }

        public DateTime? end_time_utc { get; set; }

        public int? DateId { get; set; }

        public virtual Date Date { get; set; }
    }
}

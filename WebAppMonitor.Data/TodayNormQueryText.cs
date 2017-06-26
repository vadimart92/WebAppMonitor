namespace WebAppMonitor.Data
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TodayNormQueryText")]
    public partial class TodayNormQueryText
    {
        public Guid Id { get; set; }

        public string NormalizedQuery { get; set; }

        [MaxLength(64)]
        public byte[] QueryHash { get; set; }
    }
}

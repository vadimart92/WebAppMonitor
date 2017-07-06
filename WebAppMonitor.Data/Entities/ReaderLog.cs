using System;

namespace WebAppMonitor.Data.Entities {
	[System.ComponentModel.DataAnnotations.Schema.Table("ReaderLog")]
	public class ReaderLog: BaseInfoRecord {
		public Guid QueryId { get; set; }
		public Guid StackId { get; set; }
		public long Rows { get; set; }
	}
}
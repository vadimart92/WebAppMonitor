using System;

namespace WebAppMonitor.Data.Entities
{
	[System.ComponentModel.DataAnnotations.Schema.Table("ExecutorLog")]
	public class ExecutorLog: BaseInfoRecord {
		public Guid QueryId { get; set; }
		public Guid StackId { get; set; }
		public long Duration { get; set; }
	}
}
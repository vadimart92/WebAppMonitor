using System;
using Dapper.Contrib.Extensions;

namespace WebAppMonitor.Data.Entities {
	[System.ComponentModel.DataAnnotations.Schema.Table("ReaderLog")]
	public class ReaderLog: BaseInfoRecord {
		[ExplicitKey]
		public Guid QueryId { get; set; }
		public Guid StackId { get; set; }
		public long Rows { get; set; }
	}
}
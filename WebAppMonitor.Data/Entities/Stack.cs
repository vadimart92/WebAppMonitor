using System;
using Dapper.Contrib.Extensions;

namespace WebAppMonitor.Data.Entities {
	[System.ComponentModel.DataAnnotations.Schema.Table("Stack")]
	public class Stack : IEntityWithHash<Guid> {
		[ExplicitKey, IdColumn("Id")]
		public Guid Id { get; set; }
		public Guid SourceId { get; set; }
		public string StackTrace { get; set; }
		[HashColumn("StackHash")]
		public byte[] HashValue { get; set; }
		
	}
}

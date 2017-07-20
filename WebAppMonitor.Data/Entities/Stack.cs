using System;
using Dapper.Contrib.Extensions;

namespace WebAppMonitor.Data.Entities {
	[System.ComponentModel.DataAnnotations.Schema.Table("Stack")]
	public class Stack {
		[ExplicitKey]
		public Guid Id { get; set; }
		public Guid SourceId { get; set; }
		public string StackTrace { get; set; }
		public byte[] StackHash { get; set; }
	}
}

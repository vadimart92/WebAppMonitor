using System;
using Dapper.Contrib.Extensions;

namespace WebAppMonitor.Data.Entities {
	public class BaseHashStorage {
		[ExplicitKey]
		public Guid Id { get; set; }
		public byte[] Hash { get; set; }
	}
}
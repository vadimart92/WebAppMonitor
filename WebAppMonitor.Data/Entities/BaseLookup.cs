using System;
using Dapper.Contrib.Extensions;

namespace WebAppMonitor.Data.Entities
{
	internal class BaseLookup
	{
		[ExplicitKey]
		public Guid Id { get; set; }

		public string Code { get; set; }
	}
}

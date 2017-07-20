using System;
using Dapper.Contrib.Extensions;

namespace WebAppMonitor.Data.Entities
{
	[System.ComponentModel.DataAnnotations.Schema.Table("PerformanceItemCode")]
	public class PerformanceItemCode
	{
		[ExplicitKey]
		public Guid Id
		{
			get; set;
		}
		public string Code
		{
			get; set;
		}
		public byte[] CodeHash
		{
			get; set;
		}
	}
}

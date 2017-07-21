using System;
using Dapper.Contrib.Extensions;

namespace WebAppMonitor.Data.Entities
{
	[System.ComponentModel.DataAnnotations.Schema.Table("PerformanceItemCode")]
	public class PerformanceItemCode : IEntityWithHash<Guid>
	{
		[ExplicitKey, IdColumn("Id")]
		public Guid Id {
			get; set;
		}
		
		public string Code
		{
			get; set;
		}

		[HashColumn("Hash")]
		public byte[] HashValue
		{
			get; set;
		}
	}
}

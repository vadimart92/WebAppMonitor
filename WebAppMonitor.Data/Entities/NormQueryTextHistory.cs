using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Data.Entities
{
	[Table("NormQueryTextHistory")]
	public class NormQueryTextHistory:IEntityWithHash<Guid>
	{
		[IdColumn("Id")]
		public Guid Id { get; set; }
		public string NormalizedQuery { get; set; }
		[HashColumn("QueryHash")]
		public byte[] HashValue { get; set; }
	}
}

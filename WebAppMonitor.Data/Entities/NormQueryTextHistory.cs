using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Data.Entities
{
	[Table("NormQueryTextHistory")]
	public class NormQueryTextHistory
	{
		public Guid Id { get; set; }
		public string NormalizedQuery { get; set; }
		public byte[] QueryHash { get; set; }
	}
}

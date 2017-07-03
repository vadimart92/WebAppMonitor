using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Data.Entities
{
	[Table("NormQueryTextSource")]
	class NormQueryTextSource
	{
		public Guid QuerySourceId { get; set; }
		public Guid NormQueryTextId { get; set; }
	}
}

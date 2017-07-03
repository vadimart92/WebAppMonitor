using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppMonitor.Core.Entities
{
	public class Date
	{
		public int Id { get; set; }

		[Column("Date", TypeName = "date")]
		public DateTime? DateValue { get; set; }
	}
}

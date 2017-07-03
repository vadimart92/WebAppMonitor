using System.ComponentModel.DataAnnotations;

namespace WebAppMonitor.Core.Entities
{
	public class Setting
	{
		[Key]
		public int Id { get; set; }
		public string Code { get; set; }
		public string Value { get; set; }

	}
}

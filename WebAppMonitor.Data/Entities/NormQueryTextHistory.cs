using System;

namespace WebAppMonitor.Data.Entities
{
	public class NormQueryTextHistory
	{

		public Guid Id { get; set; }
		public string NormalizedQuery { get; set; }
		public byte[] QueryHash { get; set; }
	}
}

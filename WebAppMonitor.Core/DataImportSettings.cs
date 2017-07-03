using System;

namespace WebAppMonitor.Core {
	public class DataImportSettings {

		public DataImportSettings() {
			Date = DateTime.Now;
		}
		public string EventsDataDirectoryTemplate { get; set; }
		public string StatementsFileTemplate { get; set; }
		public DateTime Date { get; set; }
		public string CurrentEventsDataDirectory => EventsDataDirectoryTemplate?.Replace("{date}",
			Date.ToString("yyyy-MM-dd"));

		public string LongLocksFileTemplate { get; set; }
	}
}
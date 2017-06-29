using System;

namespace WebAppMonitor.Core {
	public class DataImportSettings {
		public string EventsDataDirectoryTemplate { get; set; }
		public string StatementsFileTemplate { get; set; }

		public string TodayEventsDataDirectory => EventsDataDirectoryTemplate?.Replace("{date}",
			DateTime.Now.ToString("yyyy-MM-dd"));

		public string LongLocksFileTemplate { get; set; }
	}
}
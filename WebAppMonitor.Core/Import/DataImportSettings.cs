namespace WebAppMonitor.Core.Import {
	public class DataImportSettings: ISettings {
		public string EventsDataDirectoryTemplate { get; set; }
		public string DatabaseName { get; set; }
		public string DirectoriesWithJsonLog { get; set; }
		public string DailyLogsDirectoryTemplate { get; set; }
		public string ReaderLogFileName { get; set; }
		public string ExecutorLogFileName { get; set; }
		public string PerfomanceLogFileName { get; set; }
		public string StatementsFileTemplate { get; set; }
		public string LongLocksFileTemplate { get; set; }
		public string DeadLocksFileTemplate { get; set; }
	}
}
namespace WebAppMonitor.Core {
	public interface ISettingsProvider {
		string DeadLocksFileTemplate { get; set; }
		string LongLocksFileTemplate { get; set; }
		string StatementsFileTemplate { get; set; }
		string EventsDataDirectoryTemplate { get; set; }
		string DatabaseName { get; set; }
		string DirectoriesWithJsonLog { get; set; }
		string DailyLogsDirectoryTemplate { get; set; }
		string ReaderLogFileName { get; set; }
		string ExecutorLogFileName { get; set; }
		string PerfomanceLogFileName { get; set; }
	}
}
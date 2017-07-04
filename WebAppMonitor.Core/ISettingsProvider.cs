namespace WebAppMonitor.Core {
	public interface ISettingsProvider {
		string DeadLocksFileTemplate { get; set; }
		string LongLocksFileTemplate { get; set; }
		string StatementsFileTemplate { get; set; }
		string EventsDataDirectoryTemplate { get; set; }
		string DatabaseName { get; set; }

	}
}
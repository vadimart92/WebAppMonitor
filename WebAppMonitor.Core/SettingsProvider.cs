namespace WebAppMonitor.Core
{
	public class SettingsProvider : ISettingsProvider {

		private readonly ISettingsRepository _repository;

		public SettingsProvider(ISettingsRepository repository) {
			_repository = repository;
		}
		public string StatementsFileTemplate {
			get => _repository.Get("StatementsFileTemplate", @"ts_sqlprofiler_05_sec*.xel").Value;
			set => _repository.Set("StatementsFileTemplate", value);
		}
		public string LongLocksFileTemplate {
			get => _repository.Get("LongLocksFileTemplate", @"collect_long_locks_data*.xel").Value;
			set => _repository.Set("LongLocksFileTemplate", value);
		}
		public string DeadLocksFileTemplate {
			get => _repository.Get("DeadLocksFileTemplate", @"collect_deadlock_data*.xel").Value;
			set => _repository.Set("DeadLocksFileTemplate", value);
		}
		public string EventsDataDirectoryTemplate {
			get => _repository.Get("EventsDataDirectoryTemplate", 
				@"\\tscore-dev-13\WorkAnalisys\xevents\Export_{date}\").Value;
			set => _repository.Set("EventsDataDirectoryTemplate", value);
		}

		public string DatabaseName {
			get => _repository.Get("DatabaseName", @"BPMonlineWorkRUS").Value;
			set => _repository.Set("DatabaseName", value);
		}

	}
}
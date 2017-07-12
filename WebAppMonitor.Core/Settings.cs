using System.Runtime.CompilerServices;

namespace WebAppMonitor.Core
{

	
	public class Settings : ISettings {

		private readonly ISettingsRepository _repository;

		public Settings(ISettingsRepository repository) {
			_repository = repository;
		}

		private string Get([CallerMemberName]string code = null) {
			return _repository.Get(code).Value;
		}
		private void Set(string defvalue, [CallerMemberName]string code = null) {
			_repository.Set(code, defvalue);
		}
		
		public string StatementsFileTemplate {
			get => Get();
			set => Set(value);
		}
		
		public string LongLocksFileTemplate {
			get => Get();
			set => Set(value);
		}
		
		public string DeadLocksFileTemplate {
			get => Get();
			set => Set(value);
		}
		
		public string EventsDataDirectoryTemplate {
			get => Get();
			set => Set(value);
		}

		
		public string DatabaseName {
			get => Get();
			set => Set(value);
		}

		public string DirectoriesWithJsonLog {
			get => Get();
			set => Set(value);
		}
		public string DailyLogsDirectoryTemplate {
			get => Get();
			set => Set(value);
		}

		public string ReaderLogFileName {
			get => Get();
			set => Set(value);
		}

		public string ExecutorLogFileName {
			get => Get();
			set => Set(value);
		}

		public string PerfomanceLogFileName {
			get => Get();
			set => Set(value);
		}

		public string LoadJsonLogs {
			get => Get();
			set => Set(value);
		}

	}
}
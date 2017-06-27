using System;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core {
	public interface IDataImporter {
		void ImportDailyData();
		void ChangeSettings(DataImportSettings newSettings);
		DataImportSettings GetSettings();
	}

	public class DataImportSettings {
		public string EventsDataDirectoryTemplate { get; set; }
		public string StatementsFileTemplate { get; set; }

		public string TodayEventsDataDirectory => EventsDataDirectoryTemplate?.Replace("{date}",
			DateTime.Now.ToString("yyyy-MM-dd"));
	}


	public class DataImporter : IDataImporter {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly ISettingsRepository _settingsRepository;
		public DataImporter(IDbConnectionProvider connectionProvider, ISettingsRepository settingsRepository) {
			_connectionProvider = connectionProvider;
			_settingsRepository = settingsRepository;
		}

		public void ImportDailyData() {
			DataImportSettings settings = GetSettings();
			string directoryName = Path.GetDirectoryName(settings.TodayEventsDataDirectory);
			if (!Directory.Exists(directoryName)) {
				throw new Exception($"directory {directoryName} not found.");
			}
			foreach (DirectoryInfo directory in Directory.EnumerateDirectories(directoryName).Select(p => new DirectoryInfo(p))
				.OrderBy(d => d.CreationTime)) {
				string fileName = Path.Combine(directory.FullName, settings.StatementsFileTemplate);
				_connectionProvider.GetConnection(connection => {
					var db = connection.Database;
					connection.Execute(
						$@"BACKUP DATABASE [{db}] TO DISK = N'C:\BAK\{db}_compressed.bak' WITH NAME = N'{db}-Full Database backup', COMPRESSION, NOFORMAT, NOINIT, SKIP, NOREWIND, NOUNLOAD, STATS = 1");
					connection.Execute("ImportDailyData", new {
						fileName = fileName
					}, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
					connection.Execute("SaveDailyData", commandType: CommandType.StoredProcedure, commandTimeout: 3600);
				});
			}
		}

		public void ChangeSettings(DataImportSettings newSettings) {
			_settingsRepository.Set("EventsDataDirectoryTemplate", newSettings.EventsDataDirectoryTemplate);
			_settingsRepository.Set("StatementsFileTemplate", newSettings.StatementsFileTemplate);
		}

		public DataImportSettings GetSettings() {
			Setting setting = _settingsRepository.Get("EventsDataDirectoryTemplate",
				@"\\tscore-dev-13\WorkAnalisys\xevents\Export_{date}\");
			Setting statementsFileSetting = _settingsRepository.Get("StatementsFileTemplate", @"ts_sqlprofiler_05_sec*.xel");
			var settings = new DataImportSettings {
				EventsDataDirectoryTemplate = setting.Value,
				StatementsFileTemplate = statementsFileSetting.Value
			};
			return settings;
		}
	}
}
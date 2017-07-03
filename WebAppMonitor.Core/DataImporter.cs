using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core {
	public class DataImporter : IDataImporter {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly ISettingsRepository _settingsRepository;
		private readonly IExtendedEventLoader _extendedEventLoader;
		private readonly ILogger<DataImporter> _logger;

		private static int _commandTimeout = 3600;

		public DataImporter(IDbConnectionProvider connectionProvider, ISettingsRepository settingsRepository,
			IExtendedEventLoader extendedEventLoader, ILogger<DataImporter> logger) {
			_connectionProvider = connectionProvider;
			_settingsRepository = settingsRepository;
			_extendedEventLoader = extendedEventLoader;
			_logger = logger;
		}

		private void BackupDb() {
			_connectionProvider.GetConnection(connection => {
				string db = connection.Database;
				connection.Execute(
					$@"BACKUP DATABASE [{db}] TO DISK = N'C:\BAK\{db}_compressed.bak' WITH NAME = N'{
							db
						}-Full Database backup', COMPRESSION, NOFORMAT, NOINIT, SKIP, NOREWIND, NOUNLOAD, STATS = 1",
						commandTimeout: _commandTimeout);
				_logger.LogInformation("Db backup created.");
			});
		}

		private void ImportLongLocksData(DirectoryInfo directory, DataImportSettings settings) {
			string file = Path.Combine(directory.FullName, settings.LongLocksFileTemplate);
			_logger.LogInformation("Import of long locks from {0} started.", file);
			_extendedEventLoader.LoadLongLocksData(file);
			_logger.LogInformation("Import of long locks completed.");
		}

		private void ImportLongQueriesData(DbConnection connection, DirectoryInfo directory,
			DataImportSettings settings) {
			_logger.LogInformation("Executing ImportDailyData for {0}.", directory.FullName);
			connection.Execute("ImportDailyData", new {
				fileName = Path.Combine(directory.FullName, settings.StatementsFileTemplate)
			}, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
			_logger.LogInformation("Executing SaveDailyData.");
			connection.Execute("SaveDailyData", commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
		}

		public void ImportDailyData() {
			DataImportSettings settings = GetSettings();
			string directoryName = Path.GetDirectoryName(settings.CurrentEventsDataDirectory);
			if (!Directory.Exists(directoryName)) {
				throw new Exception($"directory {directoryName} not found.");
			}
			BackupDb();
			ImportData(directoryName, settings);
			ActualizeInfo();
		}

		private void ActualizeInfo() {
			_logger.LogInformation("ActualizeQueryStatInfo started.");
			_connectionProvider.GetConnection(connection => {
				connection.Execute("ActualizeQueryStatInfo", commandTimeout: _commandTimeout,
					commandType: CommandType.StoredProcedure);
			});
			_logger.LogInformation("ActualizeQueryStatInfo completed.");
		}

		private void ImportData(string directoryName, DataImportSettings settings) {
			_logger.LogInformation("Import daily data started.");
			foreach (DirectoryInfo directory in Directory.EnumerateDirectories(directoryName)
				.Select(p => new DirectoryInfo(p))
				.OrderBy(d => d.CreationTime)) {
				_connectionProvider.GetConnection(connection => {
					ImportLongQueriesData(connection, directory, settings);
				});
				ImportLongLocksData(directory, settings);
			}
			_logger.LogInformation("Import daily data completed.");
		}

		public void ChangeSettings(DataImportSettings newSettings) {
			_settingsRepository.Set("EventsDataDirectoryTemplate", newSettings.EventsDataDirectoryTemplate);
			_settingsRepository.Set("StatementsFileTemplate", newSettings.StatementsFileTemplate);
		}

		public DataImportSettings GetSettings() {
			Setting setting = _settingsRepository.Get("EventsDataDirectoryTemplate",
				@"\\tscore-dev-13\WorkAnalisys\xevents\Export_{date}\");
			Setting statementsFileSetting = _settingsRepository.Get("StatementsFileTemplate", @"ts_sqlprofiler_05_sec*.xel");
			Setting locksFileSetting = _settingsRepository.Get("LongLocksFileTemplate", @"collect_long_locks_data*.xel");
			var settings = new DataImportSettings {
				EventsDataDirectoryTemplate = setting.Value,
				StatementsFileTemplate = statementsFileSetting.Value,
				LongLocksFileTemplate = locksFileSetting.Value
			};
			return settings;
		}

		public void ImportExtendedEvents(string filePath) {
			_extendedEventLoader.LoadLongLocksData(filePath);
		}

		public void ImportAllByDates(IEnumerable<DateTime> dates) {
			DataImportSettings settings = GetSettings();
			BackupDb();
			foreach (DateTime dateTime in dates) {
				settings.Date = dateTime;
				string directoryName = Path.GetDirectoryName(settings.CurrentEventsDataDirectory);
				if (!Directory.Exists(directoryName)) {
					_logger.LogError("Directory {0} does not exists", directoryName);
					continue;
				}
				ImportData(directoryName, settings);
			}
			ActualizeInfo();
		}

	}
}
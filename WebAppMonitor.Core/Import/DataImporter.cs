using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Logging;

namespace WebAppMonitor.Core.Import {
	public class DataImporter : IDataImporter {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IExtendedEventLoader _extendedEventLoader;
		private readonly IAppLogLoader _appLogLoader;
		private readonly ILogger<DataImporter> _logger;
		private readonly ISettingsProvider _settingsProvider;

		private static int _commandTimeout = 3600;

		public DataImporter(IDbConnectionProvider connectionProvider, ISettingsProvider settingsProvider,
			IExtendedEventLoader extendedEventLoader, ILogger<DataImporter> logger, IAppLogLoader appLogLoader) {
			_connectionProvider = connectionProvider;
			_settingsProvider = settingsProvider;
			_extendedEventLoader = extendedEventLoader;
			_logger = logger;
			_appLogLoader = appLogLoader;
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

		private void ImportDeadLocksData(DirectoryInfo directory, DataImportSettings settings) {
			string file = Path.Combine(directory.FullName, settings.DeadLocksFileTemplate);
			_logger.LogInformation("Import of dead locks from {0} started.", file);
			_extendedEventLoader.LoadDeadLocksData(file);
			_logger.LogInformation("Import of dead locks completed.");
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
				ImportDeadLocksData(directory, settings);
			}
			_logger.LogInformation("Import daily data completed.");
		}

		public void ChangeSettings(DataImportSettings newSettings) {
			_settingsProvider.EventsDataDirectoryTemplate = newSettings.EventsDataDirectoryTemplate;
			_settingsProvider.StatementsFileTemplate = newSettings.StatementsFileTemplate;
		}

		public DataImportSettings GetSettings() {
			var settings = new DataImportSettings {
				EventsDataDirectoryTemplate = _settingsProvider.EventsDataDirectoryTemplate,
				StatementsFileTemplate = _settingsProvider.StatementsFileTemplate,
				LongLocksFileTemplate = _settingsProvider.LongLocksFileTemplate,
				DeadLocksFileTemplate = _settingsProvider.DeadLocksFileTemplate
			};
			return settings;
		}

		public void ImportLongLocks(string filePath) {
			_extendedEventLoader.LoadLongLocksData(filePath);
			ActualizeInfo();
		}

		public void ImportDeadlocks(string filePath) {
			_extendedEventLoader.LoadDeadLocksData(filePath);
			//ActualizeInfo();
		}

		public void ImportDbExecutorLogs(string file) {
			_logger.LogInformation("ImportDbExecutorLogs started");
			_appLogLoader.LoadReaderLogs(file);
			_logger.LogInformation("ImportDbExecutorLogs completed");
		}

		public void ImportReaderLogs(string file) {
			throw new NotImplementedException();
		}

		public void ImportPerfomanceLoggerLogs(string file) {
			throw new NotImplementedException();
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
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Logging;

namespace WebAppMonitor.Core.Import {
	public class DataLoader : IDataLoader {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IExtendedEventLoader _extendedEventLoader;
		private readonly IAppLogLoader _appLogLoader;
		private readonly ILogger<DataLoader> _logger;
		private readonly ISettings _settings;
		private readonly ISettingsRepository _settingsRepository;
		private readonly IMapper _mapper;
		private readonly IDataFilePathProvider _dataFilePathProvider;
		private readonly IServiceProvider _serviceProvider;

		private static int _commandTimeout = 3600;

		public DataLoader(IDbConnectionProvider connectionProvider, ISettings settings,
				IExtendedEventLoader extendedEventLoader, ILogger<DataLoader> logger, IAppLogLoader appLogLoader,
				IDataFilePathProvider dataFilePathProvider, IMapper mapper, ISettingsRepository settingsRepository,
				IServiceProvider serviceProvider) {
			_connectionProvider = connectionProvider;
			_settings = settings;
			_extendedEventLoader = extendedEventLoader;
			_logger = logger;
			_appLogLoader = appLogLoader;
			_dataFilePathProvider = dataFilePathProvider;
			_mapper = mapper;
			_settingsRepository = settingsRepository;
			_serviceProvider = serviceProvider;
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

		private void ImportLongLocksData(string directory) {
			string file = Path.Combine(directory, _settings.LongLocksFileTemplate);
			_logger.LogInformation("Import of long locks from {0} started.", file);
			_extendedEventLoader.LoadLongLocksData(file);
			_logger.LogInformation("Import of long locks completed.");
		}

		private void ImportDeadLocksData(string directory) {
			string file = Path.Combine(directory, _settings.DeadLocksFileTemplate);
			_logger.LogInformation("Import of dead locks from {0} started.", file);
			_extendedEventLoader.LoadDeadLocksData(file);
			_logger.LogInformation("Import of dead locks completed.");
		}

		private void ImportLongQueriesData(string directory) {
			_logger.LogInformation("Executing ImportDailyData for {0}.", directory);
			_connectionProvider.GetConnection(connection => {
				connection.Execute("ImportDailyData", new {
					fileName = Path.Combine(directory, _settings.StatementsFileTemplate)
				}, commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
				_logger.LogInformation("Executing SaveDailyData.");
				connection.Execute("SaveDailyData", commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
			});
		}

		public void ImportDailyData() {
			BackupDb();
			ImportData(_dataFilePathProvider);
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

		private void SafeExecute(Expression<Action> action) {
			try {
				action.Compile()();
			} catch (Exception e) {
				_logger.LogError(action.Body.ToString(), e);
			}
		}

		private void ImportData(IDataFilePathProvider pathProvider) {
			_logger.LogInformation("Import daily data started.");
			foreach (var directory in pathProvider.GetDailyExtEventsDirs()) {
				SafeExecute(()=>ImportLongQueriesData(directory));
				SafeExecute(() => ImportLongLocksData(directory));
				SafeExecute(() => ImportDeadLocksData(directory));
			}
			if (bool.Parse(_settings.LoadJsonLogs)) {
				foreach (string executorLog in pathProvider.GetReaderLogs()) {
					SafeExecute(() => ImportReaderLogs(executorLog));
				}
				foreach (string executorLog in pathProvider.GetExecutorLogs()) {
					SafeExecute(() => ImportDbExecutorLogs(executorLog));
				}
				foreach (string perfomanceLog in pathProvider.GetPerfomanceLogs()) {
					SafeExecute(() => ImportPerfomanceLoggerLogs(perfomanceLog));
				}
			}
			_logger.LogInformation("Import daily data completed.");
		}

		public void ChangeSettings(DataImportSettings newSettings) {
			_settingsRepository.Change(newSettings);
		}

		public DataImportSettings GetSettings() {
			return _mapper.Map<ISettings, DataImportSettings>(_settings);
		}

		public void ImportLongLocks(string filePath) {
			_extendedEventLoader.LoadLongLocksData(filePath);
			ActualizeInfo();
		}

		public void ImportDeadlocks(string filePath) {
			_extendedEventLoader.LoadDeadLocksData(filePath);
			ActualizeInfo();
		}

		public void ImportDbExecutorLogs(string file) {
			_logger.LogInformation($"ImportDbExecutorLogs started: {file}");
			_appLogLoader.LoadDbExecutorLogs(file);
			_logger.LogInformation("ImportDbExecutorLogs completed");
		}

		public void ImportReaderLogs(string file) {
			_logger.LogInformation($"ImportReaderLogs started {file}");
			_appLogLoader.LoadReaderLogs(file);
			_logger.LogInformation("ImportReaderLogs completed");
		}

		public void ImportPerfomanceLoggerLogs(string file) {
			_logger.LogInformation($"ImportReaderLogs started {file}");
			_appLogLoader.LoadPerfomanceLogs(file);
			_logger.LogInformation("ImportReaderLogs completed");
		}

		public void ImportAllByDates(IEnumerable<DateTime> dates) {
			BackupDb();
			foreach (DateTime dateTime in dates) {
				ImportData(new DataFilePathProvider(_settings,
					new StaticDateTimeProvider(dateTime),
					(ILogger<DataFilePathProvider>)_serviceProvider.GetService(typeof(ILogger<DataFilePathProvider>))));
			}
			ActualizeInfo();
		}

	}
}
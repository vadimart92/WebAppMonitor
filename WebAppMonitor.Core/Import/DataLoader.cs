﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using AutoMapper;
using Dapper;
using Microsoft.Extensions.Logging;

namespace WebAppMonitor.Core.Import {
	using System.Diagnostics;

	class DataLoadSettings
	{

		public bool LoadExtendedEvents { get; set; }

	}

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
		private readonly IDateRepository _dateRepository;
		private readonly IFileSystem _fileSystem;

		private static int _commandTimeout = 3600;

		public DataLoader(IDbConnectionProvider connectionProvider, ISettings settings,
				IExtendedEventLoader extendedEventLoader, ILogger<DataLoader> logger, IAppLogLoader appLogLoader,
				IDataFilePathProvider dataFilePathProvider, IMapper mapper, ISettingsRepository settingsRepository,
				IServiceProvider serviceProvider, IDateRepository dateRepository, IFileSystem fileSystem) {
			_connectionProvider = connectionProvider;
			_settings = settings;
			_extendedEventLoader = extendedEventLoader;
			_logger = logger;
			_appLogLoader = appLogLoader;
			_dataFilePathProvider = dataFilePathProvider;
			_mapper = mapper;
			_settingsRepository = settingsRepository;
			_serviceProvider = serviceProvider;
			_dateRepository = dateRepository;
			_fileSystem = fileSystem;
		}

		private void BackupDb() {
#if DEBUG
		return;
#endif
#pragma warning disable 162
			_connectionProvider.GetConnection(connection => {
				string db = connection.Database;
				connection.Execute($@"BACKUP DATABASE [{db}] TO DISK = N'C:\BAK\{db}_compressed.bak' WITH NAME = N'{db
						}-Full Database backup', COMPRESSION, NOFORMAT, INIT, SKIP, NOREWIND, NOUNLOAD, STATS = 1",
						commandTimeout: _commandTimeout);
				_logger.LogInformation("Db backup created.");
			});
#pragma warning restore 162
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
				string dir = Path.Combine(directory, _settings.StatementsFileTemplate);
				connection.Execute("ImportDailyData", new {fileName = dir},
					commandType: CommandType.StoredProcedure, commandTimeout: _commandTimeout);
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
				Action compiled = action.Compile();
				compiled();
			} catch (Exception exception) {
				_logger.LogError(new EventId(), exception,
					(action.Body as MethodCallExpression)?.Method.Name ?? action.Body.ToString());
			}
		}
		
		private void ImportData(IDataFilePathProvider pathProvider, DataLoadSettings settings = null) {
			_logger.LogInformation("Import daily data started for: {0:dd-MM-yyyy}", pathProvider.GetDate());
			var stopwatch = Stopwatch.StartNew();
			if (settings == null || settings.LoadExtendedEvents) {
				LoadExtendedEvents(pathProvider);
			}
			_dateRepository.Refresh();
			ImportJsonLogs(pathProvider);
			stopwatch.Stop();
			_logger.LogInformation("Import daily data completed in: {0}", stopwatch.Elapsed);
		}

		private void ImportJsonLogs(IDataFilePathProvider pathProvider) {
			ClearTmpDir();
			foreach (string readerLog in pathProvider.GetReaderLogs()) {
				SafeExecute(() => ImportReaderLogs(readerLog));
			}
			foreach (string executorLog in pathProvider.GetExecutorLogs()) {
				SafeExecute(() => ImportDbExecutorLogs(executorLog));
			}
			foreach (string perfomanceLog in pathProvider.GetPerfomanceLogs()) {
				SafeExecute(() => ImportPerfomanceLoggerLogs(perfomanceLog));
			}
		}

		private void LoadExtendedEvents(IDataFilePathProvider pathProvider) {
			ClearTmpDir();
			foreach (string directory in pathProvider.GetDailyExtEventsDirs()) {
				_logger.LogInformation("LoadExtendedEvents from: {0}", directory);
				SafeExecute(() => ImportLongQueriesData(directory));
				SafeExecute(() => ImportLongLocksData(directory));
				SafeExecute(() => ImportDeadLocksData(directory));
			}
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
			_logger.LogInformation($"ImportDbExecutorLogs started: {Path.GetFileName(file)}");
			_appLogLoader.LoadDbExecutorLogs(file);
			_logger.LogInformation("ImportDbExecutorLogs completed");
		}

		public void ImportReaderLogs(string file) {
			_logger.LogInformation($"ImportReaderLogs started {Path.GetFileName(file)}");
			_appLogLoader.LoadReaderLogs(file);
			_logger.LogInformation("ImportReaderLogs completed");
		}

		public void ImportPerfomanceLoggerLogs(string file) {
			_logger.LogInformation($"ImportReaderLogs started {Path.GetFileName(file)}");
			_appLogLoader.LoadPerfomanceLogs(file);
			_logger.LogInformation("ImportReaderLogs completed");
		}

		public void ImportExtendedEvents() {
			_logger.LogInformation("ImportExtendedEvents started");
			LoadExtendedEvents(_dataFilePathProvider);
			ActualizeInfo();
			_logger.LogInformation("ImportExtendedEvents completed");
		}

		public void ImportJsonLogs() {
			_logger.LogInformation("ImportJsonLogs started");
			ImportJsonLogs(_dataFilePathProvider);
			_logger.LogInformation("ImportJsonLogs completed");
		}

		public void ImportAllByDates(IEnumerable<DateTime> dates) {
			BackupDb();
			var settings = new DataLoadSettings {
				LoadExtendedEvents = false
			};
			foreach (DateTime dateTime in dates) {
				var pathProvider = new DataFilePathProvider(_settings, new StaticDateTimeProvider(dateTime),
					(ILogger<DataFilePathProvider>)_serviceProvider.GetService(typeof(ILogger<DataFilePathProvider>)), 
					(IFileSystem)_serviceProvider.GetService(typeof(IFileSystem)));
				var provider = new CompositePathProvider(pathProvider);
				ImportData(provider, settings);
			}
			ActualizeInfo();
		}

		private void ClearTmpDir() {
			var tmpDir = _fileSystem.GetTempDirectoryPath();
			if (Directory.Exists(tmpDir)) {
				Directory.Delete(tmpDir, true);
			}
			Directory.CreateDirectory(tmpDir);
		}
	}
}
﻿using System;
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

		public DataImporter(IDbConnectionProvider connectionProvider, ISettingsRepository settingsRepository,
			IExtendedEventLoader extendedEventLoader) {
			_connectionProvider = connectionProvider;
			_settingsRepository = settingsRepository;
			_extendedEventLoader = extendedEventLoader;
		}


		private static void BackupDb(DbConnection connection) {
			var db = connection.Database;
			connection.Execute(
				$@"BACKUP DATABASE [{db}] TO DISK = N'C:\BAK\{db}_compressed.bak' WITH NAME = N'{
						db
					}-Full Database backup', COMPRESSION, NOFORMAT, NOINIT, SKIP, NOREWIND, NOUNLOAD, STATS = 1");
		}

		private void ImportLongLocksData(DirectoryInfo directory, DataImportSettings settings) {
			var file = Path.Combine(directory.FullName, settings.LongLocksFileTemplate);
			_extendedEventLoader.LoadLongLocksData(file);
		}

		private static void ImportLongQueriesData(DbConnection connection, DirectoryInfo directory,
			DataImportSettings settings) {
			connection.Execute("ImportDailyData", new {
				fileName = Path.Combine(directory.FullName, settings.StatementsFileTemplate)
			}, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
			connection.Execute("SaveDailyData", commandType: CommandType.StoredProcedure, commandTimeout: 3600);
		}

		public void ImportDailyData() {
			DataImportSettings settings = GetSettings();
			string directoryName = Path.GetDirectoryName(settings.TodayEventsDataDirectory);
			if (!Directory.Exists(directoryName)) {
				throw new Exception($"directory {directoryName} not found.");
			}
			foreach (DirectoryInfo directory in Directory.EnumerateDirectories(directoryName).Select(p => new DirectoryInfo(p))
				.OrderBy(d => d.CreationTime)) {
				_connectionProvider.GetConnection(connection => {
					BackupDb(connection);
					ImportLongQueriesData(connection, directory, settings);
				});
				ImportLongLocksData(directory, settings);
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
	}
}
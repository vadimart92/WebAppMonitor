using System;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using WebAppMonitor.Data;

namespace WebAppMonitor.Common
{

	public interface IDataImporter
	{

		void ImportDailyData();
		void ChangeSettings(DataImportSettings newSettings);
		DataImportSettings GetSettings();

	}

	public class DataImportSettings
	{
	
		public string EventsDataDirectoryTemplate { get; set; }
		public string StatementsFileTemplate { get; set; }

		public string TodayEventsDataDirectory => EventsDataDirectoryTemplate?.Replace("{date}", DateTime.Now.ToString("yyyy-MM-dd"));

	}


	public class DataImporter: IDataImporter
	{

	    private readonly QueryStatsContext _dbContext;

	    private readonly IDbConnectionProvider _connectionProvider;

		public DataImporter(QueryStatsContext dbContext, IDbConnectionProvider connectionProvider) {
		    _dbContext = dbContext;
		    _connectionProvider = connectionProvider;
	    }

		public void ImportDailyData() {
			DataImportSettings settings = GetSettings();
			string directoryName = Path.GetDirectoryName(settings.TodayEventsDataDirectory);
			if (!Directory.Exists(directoryName)) {
				throw new Exception($"directory {directoryName} not found.");
			}
			foreach (DirectoryInfo directory in Directory.EnumerateDirectories(directoryName).Select(p=>new DirectoryInfo(p)).OrderBy(d=>d.CreationTime)) {
				string fileName = Path.Combine(directory.FullName, settings.StatementsFileTemplate);
				_connectionProvider.GetConnection(connection => {
					connection.Execute("ImportDailyData", new { fileName = fileName }, commandType: CommandType.StoredProcedure, commandTimeout: 3600);
					connection.Execute("SaveDailyData", commandType: CommandType.StoredProcedure, commandTimeout: 3600);
				});
			}
		}

		public void ChangeSettings(DataImportSettings newSettings) { }

		public DataImportSettings GetSettings() {
			Setting setting = GetSettingFromDb("EventsDataDirectoryTemplate", 
				@"\\tscore-dev-13\WorkAnalisys\xevents\Export_{date}\");
			Setting statementsFileSetting = GetSettingFromDb("StatementsFileTemplate",
				@"ts_sqlprofiler_05_sec*.xel");
			var settings = new DataImportSettings {
				EventsDataDirectoryTemplate = setting.Value,
				StatementsFileTemplate = statementsFileSetting.Value
			};
			return settings;
		}

		private Setting GetSettingFromDb(string settingCode, string defValue) {
			Setting setting = _dbContext.Settings.First(s => s.Code == settingCode);
			if (setting == null) {
				setting = new Setting {
					Code = settingCode,
					Value = defValue
				};
				_dbContext.Settings.Add(setting);
				_dbContext.SaveChanges();
			}
			return setting;
		}

	}
}

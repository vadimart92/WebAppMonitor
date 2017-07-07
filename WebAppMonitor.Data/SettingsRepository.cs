using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Data {
	public class SettingsRepository : ISettingsRepository {
		private readonly QueryStatsContext _dbContext;
		private readonly ILogger _logger;

		public SettingsRepository(QueryStatsContext dbContext, ILogger<SettingsRepository> logger) {
			_dbContext = dbContext;
			_logger = logger;
		}

		private Setting GetSettingFromDb(string settingCode, string defValue) {
			Setting setting = _dbContext.Settings.FirstOrDefault(s => s.Code == settingCode);
			if (setting == null) {
				setting = new Setting {
					Code = settingCode,
					Value = defValue
				};
				_dbContext.Settings.Add(setting);
				_dbContext.SaveChanges();
				_logger.LogInformation("Setting {0} created as {1}", settingCode, defValue);
			}
			return setting;
		}

		public Setting Get(string code) {
			string defValue = SettingItemAttribute.GetDefValue(code);
			return GetSettingFromDb(code, defValue);
		}

		public void Set(string code, string value) {
			Setting settings = GetSettingFromDb(code?.Trim(), value);
			settings.Value = value?.Trim();
			_dbContext.SaveChanges();
			_logger.LogInformation("Setting {0} changed to {1}", code, value);
		}

		public void Change(ISettings settings) {
			var items = typeof(ISettings).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
			foreach (PropertyInfo property in items) {
				var value = property.GetValue(settings) as string;
				string defValue = SettingItemAttribute.GetDefValue(property.Name);
				Set(property.Name, value ?? defValue);
			}
		}
	}
}
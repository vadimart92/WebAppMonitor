using System.Linq;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Data {
	public class SettingsRepository : ISettingsRepository {
		private readonly QueryStatsContext _dbContext;

		public SettingsRepository(QueryStatsContext dbContext) {
			_dbContext = dbContext;
		}

		private Setting GetSettingFromDb(string settingCode, string defValue) {
			Setting setting = _dbContext.Settings.AsNoTracking().FirstOrDefault(s => s.Code == settingCode);
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

		public Setting Get(string code, string defValue) {
			return GetSettingFromDb(code, defValue);
		}

		public void Set(string code, string value) {
			Setting settings = GetSettingFromDb(code, value);
			settings.Value = value;
			_dbContext.SaveChanges();
		}

	}
}
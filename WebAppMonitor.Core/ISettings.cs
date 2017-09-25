using System;
using System.Reflection;

namespace WebAppMonitor.Core {

	[AttributeUsage(AttributeTargets.Property)]
	public sealed class SettingItemAttribute : Attribute {
		public SettingItemAttribute(string defValue) {
			DefaultValue = defValue;
		}
		public string DefaultValue { get; }

		public static string GetDefValue(string propertyName) {
			var attr = typeof(ISettings).GetProperty(propertyName)
				.GetCustomAttribute(typeof(SettingItemAttribute)) as SettingItemAttribute;
			return attr?.DefaultValue;
		}
	}
	
	public interface ISettings {
		[SettingItem("collect_deadlock_data*.xel")]
		string DeadLocksFileTemplate { get; set; }
		[SettingItem("collect_long_locks_data*.xel")]
		string LongLocksFileTemplate { get; set; }
		[SettingItem("ts_sqlprofiler_05_sec*.xel")]
		string StatementsFileTemplate { get; set; }
		[SettingItem(@"\\TSDB01\xEvent\BPMonlineWorkRUS")]
		string EventsDataDirectoryTemplate { get; set; }
		[SettingItem("BPMonlineWorkRUS")]
		string DatabaseName { get; set; }
		[SettingItem(@"\\tsinternalcrm1\BPMonlineLog\Site_1;\\tsinternalcrm3\BPMonlineLog\Site_1;\\tsinternalcrm4\BPMonlineLog\Site_1")]
		string DirectoriesWithJsonLog { get; set; }
		[SettingItem("yyyy_MM_dd")]
		string DailyLogsDirectoryTemplate { get; set; }
		[SettingItem("LoggingDataReader.0.json")]
		string ReaderLogFileName { get; set; }
		[SettingItem("Sql.0.json")]
		string ExecutorLogFileName { get; set; }
		[SettingItem("PerformanceLogger.0.json")]
		string PerfomanceLogFileName { get; set; }
		[SettingItem(@"\\tscore-dev-13\WorkAnalisys\xEventsTemp")]
		string SharedDirectoryPath { get; set; }
	}
}
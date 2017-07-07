using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core.Common;

namespace WebAppMonitor.Core.Import {
	public class DataFilePathProvider : IDataFilePathProvider {
		private readonly ILogger _logger;
		private readonly ISettings _settings;
		private readonly IDateTimeProvider _dateTimeProvider;

		public DataFilePathProvider(ISettings settings,
			IDateTimeProvider dateTimeProvider, ILogger logger) {
			_settings = settings;
			_dateTimeProvider = dateTimeProvider;
			_logger = logger;
		}

		public IEnumerable<string> GetExecutorLogs() {
			return EnumerateFiles(_settings.ExecutorLogFileName);
		}

		public IEnumerable<string> GetReaderLogs() {
			return EnumerateFiles(_settings.ReaderLogFileName);
		}

		public IEnumerable<string> GetPerfomanceLogs() {
			return EnumerateFiles(_settings.PerfomanceLogFileName);
		}

		private IEnumerable<string> EnumerateFiles(string logFileName) {
			var sources = _settings.DirectoriesWithJsonLog?.Split(new[] {';'},
				StringSplitOptions.RemoveEmptyEntries).ToList();
			string settingsDailyLogsDirectoryTemplate = _settings.DailyLogsDirectoryTemplate;
			string dirName = _dateTimeProvider.Today.ToString(settingsDailyLogsDirectoryTemplate);
			string dir = $"{Path.DirectorySeparatorChar}{dirName}{Path.DirectorySeparatorChar}";
			if (sources == null)
				yield break;
			foreach (string source in sources) {
				if (!Directory.Exists(source)) {
					_logger.LogWarning("Source directory {0} not found", source);
					continue;
				}
				var innerFiles = Directory.EnumerateFiles(source, $"*{logFileName}",
					SearchOption.AllDirectories).Where(p => p.Contain(dir));
				foreach (string file in innerFiles) {
					string tmpFile = Path.GetTempFileName();
					File.Copy(file, tmpFile, true);
					yield return tmpFile;
					File.Delete(tmpFile);
				}
			}
		}

		public IEnumerable<string> GetDailyExtEventsDirs() {
			string dataDirectory = _settings.EventsDataDirectoryTemplate?.Replace("{date}",
				_dateTimeProvider.Today.ToString("yyyy-MM-dd"));
			if (!Directory.Exists(dataDirectory)) {
				throw new Exception($"directory {dataDirectory} not found.");
			}
			return Directory.EnumerateDirectories(dataDirectory).Select(p => new DirectoryInfo(p))
				.OrderBy(d => d.CreationTime).Select(d => d.FullName);
		}
	}
}

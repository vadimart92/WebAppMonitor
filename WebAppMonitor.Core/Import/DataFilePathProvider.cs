using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core.Common;

namespace WebAppMonitor.Core.Import {
	using System.Text.RegularExpressions;

	public class DataFilePathProvider : IDataFilePathProvider {
		private readonly ILogger _logger;
		private readonly ISettings _settings;
		private readonly IFileSystem _fileSystem;
		private readonly IDateTimeProvider _dateTimeProvider;

		public DataFilePathProvider(ISettings settings, IDateTimeProvider dateTimeProvider,
				ILogger<DataFilePathProvider> logger, IFileSystem fileSystem) {
			_settings = settings;
			_dateTimeProvider = dateTimeProvider;
			_logger = logger;
			_fileSystem = fileSystem;
		}

		public DateTime GetDate() {
			return _dateTimeProvider.Today;
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
					string tmpPrefix = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
					string tmpFile = Path.Combine(_fileSystem.GetTempDirectoryPath(), tmpPrefix + Path.GetFileName(file));
					try {
						_logger.LogInformation("Copying file {0} to {1}", file, Path.GetFileNameWithoutExtension(tmpFile));
						File.Copy(file, tmpFile, true);
						yield return tmpFile;
					} finally {
						try {
							if (File.Exists(tmpFile)) {
								File.Delete(tmpFile);
							}
						} catch (Exception e) {
							_logger.LogError(new EventId(0), e, "file processing error, source file: {0}", file);
						}
					}
				}
			}
		}

		public IEnumerable<string> GetDailyExtEventsDirs() {
			string dataDirectory = _settings.EventsDataDirectoryTemplate;
			if (!Directory.Exists(dataDirectory)) {
				_logger.LogError($"directory {dataDirectory} not found.");
				yield break;
			}
			DateTime now = _dateTimeProvider.UtcNow;
			DateTime startTime = now - TimeSpan.FromHours(25);
			var nameTpls = new[] {
				_settings.LongLocksFileTemplate,
				_settings.DeadLocksFileTemplate,
				_settings.StatementsFileTemplate
			}.Select(re => new Regex(Regex.Escape(re).Replace(Regex.Escape("*"), "(.*)") + "$")).ToList();
			var xevents = Directory.EnumerateFiles(dataDirectory)
				.Select(p => new FileInfo(p))
				.Where(f => CheckIsMatchDate(f, startTime, now))
				.Where(f => CheckIsMatchName(f, nameTpls))
				.Select(f=>f.FullName).ToList();
			var dir = Path.Combine(_fileSystem.GetTempDirectoryPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(dir);
			_logger.LogInformation("Copying xEvents from {0} to {1}", dataDirectory, dir);
			foreach (var file in xevents) {
				string fileName = Path.GetFileName(file);
				string destFileName = Path.Combine(dir, fileName);
				_logger.LogDebug("Copying file: {0}", fileName);
				File.Copy(file, destFileName);
			}
			yield return dir;
			_logger.LogInformation("Deleting: {0}", dir);
			Directory.Delete(dir, true);
		}

		private bool CheckIsMatchName(FileInfo fileInfo, IEnumerable<Regex> templates) {
			return templates.Any(regex => regex.IsMatch(fileInfo.FullName));
		}

		private static bool CheckIsMatchDate(FileInfo f, DateTime beginTime, DateTime today) {
			return f.CreationTimeUtc >= beginTime && f.CreationTimeUtc <= today;
		}

	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace WebAppMonitor.Core.Import
{
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
		    var sources = _settings.DirectoriesWithJsonLog?.Split(new[]{';'}, 
				StringSplitOptions.RemoveEmptyEntries).ToList();
		    var dirName = _dateTimeProvider.Today.ToString(_settings.DailyLogsDirectoryTemplate);
			if (sources == null) yield break;
		    foreach (var source in sources) {
			    if (!Directory.Exists(source)) {
				    _logger.LogWarning("Source directory {0} not found", source);
					continue;
			    };
			    string pattern = $"/**/{dirName}/**/{_settings.ExecutorLogFileName}.*";
			    var innerDirs = Directory.EnumerateFiles(source, pattern, SearchOption.AllDirectories).ToList();
		    }
			yield break;
	    }

	    public IEnumerable<string> GetReaderLogs() {
		    yield break;
	    }

	    public IEnumerable<string> GetPerfomanceLogs() {
			yield break;
		}

	    public IEnumerable<string> GetDailyExtEventsDirs() {
		     string dataDirectory = _settings.EventsDataDirectoryTemplate?.Replace("{date}", 
				 _dateTimeProvider.Today.ToString("yyyy-MM-dd"));
		    if (!Directory.Exists(dataDirectory)) {
			    throw new Exception($"directory {dataDirectory} not found.");
		    }
			return Directory.EnumerateDirectories(dataDirectory).Select(p => new DirectoryInfo(p))
			    .OrderBy(d => d.CreationTime).Select(d=>d.FullName);

	    }
    }
}

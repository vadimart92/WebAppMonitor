using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.DataProcessing
{
	public class AppLogLoader: IAppLogLoader
	{

		private readonly IJsonLogParser _jsonLogParser;
		private readonly IJsonLogStoringService _jsonLogStoringService;

		public AppLogLoader(IJsonLogParser jsonLogParser, IJsonLogStoringService jsonLogStoringService) {
			_jsonLogParser = jsonLogParser;
			_jsonLogStoringService = jsonLogStoringService;
		}

		public void LoadReaderLogs(string file) {
			using (_jsonLogStoringService.BeginWork()) {
				foreach (ReaderLogRecord logRecord in _jsonLogParser.ReadFile<ReaderLogRecord>(file)) {
					_jsonLogStoringService.RegisterReaderLogItem(logRecord);
				}
			}
		}

	}
}

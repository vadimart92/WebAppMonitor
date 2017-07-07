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

		public void LoadDbExecutorLogs(string file) {
			using (_jsonLogStoringService.BeginWork()) {
				foreach (ExecutorLogRecord logRecord in _jsonLogParser.ReadFile<ExecutorLogRecord>(file)) {
					_jsonLogStoringService.RegisterExecutorLogRecord(logRecord);
				}
			}
		}

		public void LoadPerfomanceLogs(string file) {
			using (_jsonLogStoringService.BeginWork()) {
				foreach (PerfomanceLogRecord logRecord in _jsonLogParser.ReadFile<PerfomanceLogRecord>(file)) {
					_jsonLogStoringService.RegisterPerfomanceLogItem(logRecord);
				}
			}
		}

	}
}

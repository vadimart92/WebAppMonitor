using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.DataProcessing
{
	public class AppLogLoader: IAppLogLoader
	{

		private readonly IJsonLogParser _jsonLogParser;
		private readonly IJsonLogSaver _jsonLogSaver;

		public AppLogLoader(IJsonLogParser jsonLogParser, IJsonLogSaver jsonLogSaver) {
			_jsonLogParser = jsonLogParser;
			_jsonLogSaver = jsonLogSaver;
		}

		public void LoadReaderLogs(string file) {
			using (_jsonLogSaver.BeginWork()) {
				foreach (ReaderLogRecord logRecord in _jsonLogParser.ReadFile<ReaderLogRecord>(file)) {
					_jsonLogSaver.RegisterReaderLogItem(logRecord);
				}
			}
		}

	}
}

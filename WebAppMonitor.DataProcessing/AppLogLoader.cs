using System.Diagnostics;

namespace WebAppMonitor.DataProcessing
{
	using System;
	using WebAppMonitor.Core.Import;
	using WebAppMonitor.Core.Import.Entity;

	public class AppLogLoader: IAppLogLoader
	{

		private readonly IJsonLogParser _jsonLogParser;
		private readonly IJsonLogStoringService _jsonLogStoringService;

		public AppLogLoader(IJsonLogParser jsonLogParser, IJsonLogStoringService jsonLogStoringService) {
			_jsonLogParser = jsonLogParser;
			_jsonLogStoringService = jsonLogStoringService;
		}

		private void SafeExecute<TLogRecord>(string file, Func<IJsonLogStoringService, Action<TLogRecord>> action)
				where TLogRecord : IJsonLogWithHash {
			using (_jsonLogStoringService.BeginWork()){
				var addRecordAction = action(_jsonLogStoringService);
				foreach(TLogRecord logRecord in _jsonLogParser.ReadFile<TLogRecord>(file)) {
					try {
						addRecordAction(logRecord);
					} catch(Exception) {
					    if (Debugger.IsAttached) {
					        Debugger.Break();
					    }
						throw;
					}
				}
			}
		}

		public void LoadReaderLogs(string file) {
			SafeExecute<ReaderLogRecord>(file, store => store.RegisterReaderLogItem);
		}

		public void LoadDbExecutorLogs(string file) {
			SafeExecute<ExecutorLogRecord>(file, store => store.RegisterExecutorLogRecord);
		}

		public void LoadPerfomanceLogs(string file) {
			SafeExecute<PerfomanceLogRecord>(file, store => store.RegisterPerfomanceLogItem);
		}

	}
}

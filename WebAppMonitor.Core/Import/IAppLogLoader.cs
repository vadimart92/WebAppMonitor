namespace WebAppMonitor.Core.Import {
	public interface IAppLogLoader {

		void LoadReaderLogs(string file);

		void LoadDbExecutorLogs(string file);

		void LoadPerfomanceLogs(string file);

	}
}

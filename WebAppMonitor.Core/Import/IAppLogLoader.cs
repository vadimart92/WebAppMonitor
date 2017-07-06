namespace WebAppMonitor.Core.Import {
	public interface IAppLogLoader {

		void LoadReaderLogs(string file);

		void ImportDbExecutorLogs(string file);

	}
}

using System;
using System.Collections.Generic;

namespace WebAppMonitor.Core.Import
{
	public interface IDataLoader {
		void ImportDailyData();
		void ChangeSettings(DataImportSettings newSettings);
		DataImportSettings GetSettings();
		void ImportLongLocks(string filePath);
		void ImportAllByDates(IEnumerable<DateTime> dates);
		void ImportDeadlocks(string file);
		void ImportDbExecutorLogs(string file);
		void ImportReaderLogs(string file);
		void ImportPerfomanceLoggerLogs(string file);
	}
}
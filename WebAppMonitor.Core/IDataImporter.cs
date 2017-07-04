using System;
using System.Collections.Generic;

namespace WebAppMonitor.Core
{
	public interface IDataImporter {
		void ImportDailyData();
		void ChangeSettings(DataImportSettings newSettings);
		DataImportSettings GetSettings();
		void ImportLongLocks(string filePath);
		void ImportAllByDates(IEnumerable<DateTime> dates);
		void ImportDeadlocks(string file);

	}
}
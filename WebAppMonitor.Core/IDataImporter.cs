namespace WebAppMonitor.Core
{
	public interface IDataImporter {
		void ImportDailyData();
		void ChangeSettings(DataImportSettings newSettings);
		DataImportSettings GetSettings();
		void ImportExtendedEvents(string filePath);
	}
}
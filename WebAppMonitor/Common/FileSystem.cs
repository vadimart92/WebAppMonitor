namespace WebAppMonitor.Common
{
	using WebAppMonitor.Core;

	public class FileSystem : IFileSystem
	{

		private readonly ISettings _settings;

		public FileSystem(ISettings settings) {
			_settings = settings;
		}

		public string GetTempDirectoryPath() {
			return _settings.SharedDirectoryPath;
		}

	}
}

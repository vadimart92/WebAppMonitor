using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core
{

	public interface ISettingsRepository
	{
		Setting Get(string code);
		void Set(string code, string value);
		void Change(ISettings settings);
	}
}

using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.Core
{
	public interface ISettingsRepository
	{
		Setting Get(string code, string defValue);
		void Set(string code, string value);
	}
}

using System.Collections.Generic;

namespace WebAppMonitor.Core.Import
{
    public interface IJsonLogFilePathProvider
    {

	    IEnumerable<string> EnumerateDailyLogs(ISettingsProvider settingsProvider);

    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace WebAppMonitor.Core.Import
{
    public class JsonLogFilePathProvider : IJsonLogFilePathProvider {

	    public IEnumerable<string> EnumerateDailyLogs(ISettingsProvider settingsProvider) {
		    var sources = settingsProvider.DirectoriesWithJsonLog?.Split(new[]{';'}, 
				StringSplitOptions.RemoveEmptyEntries).ToList();
		    
		yield break;
	    }

    }
}

using System.Collections.Generic;

namespace WebAppMonitor.Core
{
    public class CaheUtils
    {
	    public static void ClearCache(IMemoryCache cache) {
		    var keys = cache.Get("Keys") as IEnumerable<string>;
		    if (keys != null)
			    foreach (string key in keys) {
				    cache.Remove(key);
			    }
	    }
	}
}

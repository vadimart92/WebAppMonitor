using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

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

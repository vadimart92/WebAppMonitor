using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace WebAppMonitor.Common
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

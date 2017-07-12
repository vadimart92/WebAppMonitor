using System;

namespace WebAppMonitor.Core.Common {
	public static class StringUtils {
		public static bool Contain(this string value, string find) {
			if (string.IsNullOrEmpty(value)) {
				return false;
			}
			return value.IndexOf(find, StringComparison.OrdinalIgnoreCase) > -1;
		}
	}
}

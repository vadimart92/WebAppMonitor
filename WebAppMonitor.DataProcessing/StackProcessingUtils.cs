using System;
using System.IO;
using System.Text;
using WebAppMonitor.Core.Common;

namespace WebAppMonitor.DataProcessing {
	public static class StackProcessingUtils {
		public static string NormalizeReaderStack(this string stackTrace) {
			if (stackTrace == null) {
				return null;
			}
			var result = new StringBuilder();
			using (var reader = new StringReader(stackTrace)) {
				bool readerStackEndFound = false;
				while (reader.Peek()>-1) {
					var line = reader.ReadLine();
					if (line==null) continue;
					if (line.Contain("Terrasoft.Core.DB.LoggingDataReader") 
						&& (line.Contain("Dispose") || line.Contain("Close"))) {
						readerStackEndFound = true;
						continue;
					}
					if (!readerStackEndFound) continue;
					var indexOfClosingBrace = line.IndexOf(")", StringComparison.OrdinalIgnoreCase);
					line = line.Substring(0, indexOfClosingBrace + 1).Trim();
					result.AppendLine(line);
				}
				if (!readerStackEndFound) {
					return stackTrace.Trim();
				}
			}
			return result.ToString(0, result.Length - Environment.NewLine.Length);
		}
	}
}

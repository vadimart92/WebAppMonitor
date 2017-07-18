using System;
using System.IO;
using System.Text;
using WebAppMonitor.Core.Common;

namespace WebAppMonitor.DataProcessing {
	using System.Collections.Generic;

	public static class StackProcessingUtils {

		public static string NormalizeExecutorStack(this string stackTrace) {
			return TrimStackLines(stackTrace, ExecutorFirstLineLocator);
		}

		public static string NormalizeReaderStack(this string stackTrace) {
			return TrimStackLines(stackTrace, ReaderFirstLineLocator);
		}

		private static string TrimStackLines(string stackTrace, Func< string, bool> firstLineLocator) {
			if (stackTrace == null) {
				return null;
			}
			var result = new StringBuilder();
			using (var reader = new StringReader(stackTrace)) {
				var readerStackEndFound = false;
				List<string> linesToAppend = new List<string>();
				while (reader.Peek() > -1) {
					string line = reader.ReadLine();
					if (line == null)
						continue;
					if (!readerStackEndFound && firstLineLocator(line)) {
						readerStackEndFound = true;
						continue;
					}
					if (!readerStackEndFound)
						continue;
					int indexOfClosingBrace = line.IndexOf(")", StringComparison.OrdinalIgnoreCase);
					line = line.Substring(0, indexOfClosingBrace + 1).Trim();
					if (line.StartsWith("at Terrasoft", StringComparison.OrdinalIgnoreCase)) {
						if (linesToAppend.Count > 0) {
							foreach (string l in linesToAppend) {
								result.AppendLine(l);
							}
							linesToAppend.Clear();
						}
						result.AppendLine(line);
					} else {
						linesToAppend.Add(line);
					}
				}
				if (!readerStackEndFound) {
					return stackTrace.Trim();
				}
			}
			return result.ToString(0, result.Length - Environment.NewLine.Length);
		}

		private static bool ReaderFirstLineLocator(string line) {
			return line.Contain("Terrasoft.Core.DB.LoggingDataReader") 
			       && (line.Contain("OnDestroy") || line.Contain("Close"));
		}
		private static bool ExecutorFirstLineLocator(string line) {
			return line.Contain("Terrasoft.Core.DB.DBExecutor.LoggingWrap");
		}

	}
}

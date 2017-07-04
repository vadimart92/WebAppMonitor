using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebAppMonitor.DataProcessing
{
	public static class SqlExtractionUtils
	{
		private static readonly Regex _eolRegex = new Regex(@"\r\s*|\n\s*|\r\ns*|\n\r\s*",
			RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Compiled);
	
		public static string ExtractRpcText(this string input) {
			if (string.IsNullOrWhiteSpace(input)) {
				return input;
			}
			int first = input.IndexOf("'", StringComparison.Ordinal) + 1;
			int last = input.IndexOf("'", first, StringComparison.Ordinal);
			int lenght = last - first;
			bool found = first > -1 && lenght > 0 && input.Length - first > lenght;
			string subInput = found ? input.Substring(first, lenght) : input;
			return _eolRegex.Replace(subInput, " ");
		}

		public static string ExtractLocksSqlText(this string input) {
			if (input == null) {
				return null;
			}
			string result = input.Trim();
			if (result.StartsWith("(", StringComparison.OrdinalIgnoreCase)) {
				int indexOfClosingBrace = result.IndexOf(")", StringComparison.OrdinalIgnoreCase);
				int indexOfNewLine = result.IndexOf("\n", StringComparison.OrdinalIgnoreCase);
				if (indexOfClosingBrace > -1 && indexOfNewLine > indexOfClosingBrace) {
					result = result.Substring(indexOfNewLine + 1).Trim();
				} else if (result.StartsWith("(@", StringComparison.OrdinalIgnoreCase)) {
					using (var reader = new StringReader(result)) {
						int currebtChar = -1;
						int startBrace = char.ConvertToUtf32("(", 0);
						int endBrace = char.ConvertToUtf32(")", 0);
						int startBracesCount = 0;
						int endBracesCount = 0;
						int posistion = -1;
						int maxAllowedPosition = result.Length - 1;
						while ((currebtChar = reader.Read()) != -1) {
							posistion++;
							if (currebtChar == startBrace) {
								startBracesCount++;
							} else if (currebtChar == endBrace) {
								endBracesCount++;
							}
							if (startBracesCount == endBracesCount && posistion < maxAllowedPosition) {
								result = result.Substring(posistion + 1).Trim();
								break;
							}
						}
					}
				}
			}
			return _eolRegex.Replace(result, " ");
		}
	}
}

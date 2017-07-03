using System;
using System.IO;
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

		public static string ExtractLongLocksSqlText(this string input) {
			if (input == null) {
				return null;
			}
			string result = input.Trim();
			if (result.StartsWith("(", StringComparison.OrdinalIgnoreCase)) {
				int indexOfClosingBrace = result.IndexOf(")", StringComparison.OrdinalIgnoreCase);
				int indexOfNewLine = result.IndexOf("\n", StringComparison.OrdinalIgnoreCase);
				if (indexOfClosingBrace > -1 && indexOfNewLine > indexOfClosingBrace) {
					result = result.Substring(indexOfNewLine + 1).Trim();
				}
			}
			return _eolRegex.Replace(result, " ");
		}
	}
}

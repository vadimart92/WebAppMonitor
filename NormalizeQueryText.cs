using System;
using System.Data.SqlTypes;
using System.Text.RegularExpressions;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString NormalizeQueryText(SqlString input)
    {
        return new SqlString (Normalize(input.Value));
    }
	private static readonly Regex _eolRegex = new Regex(@"\r\s*|\n\s*|\r\ns*|\n\r\s*",
		RegexOptions.CultureInvariant | RegexOptions.Multiline | RegexOptions.Compiled);

	private static string Normalize(string input) {
		if (string.IsNullOrWhiteSpace(input)) {
			return input;
		}
		input = input.Trim();
		int first = input.IndexOf("'", StringComparison.Ordinal) + 1;
		int last = input.IndexOf("'", first, StringComparison.Ordinal);
		int lenght = last - first;
		bool found = first > -1 && lenght > 0 && input.Length - first > lenght;
		string subInput = found ? input.Substring(first, lenght) : input;
		return _eolRegex.Replace(subInput, " ").Trim();
	}
}

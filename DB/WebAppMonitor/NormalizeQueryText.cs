using System.Data.SqlTypes;
using WebAppMonitor.DataProcessing;

public partial class UserDefinedFunctions
{
    [Microsoft.SqlServer.Server.SqlFunction]
    public static SqlString NormalizeQueryText(SqlString input)
    {
		return new SqlString(input.Value.ExtractRpcText());
	}
}

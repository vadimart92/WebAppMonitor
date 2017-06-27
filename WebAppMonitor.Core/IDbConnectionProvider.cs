using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace WebAppMonitor.Core
{
  public interface IDbConnectionProvider
  {

    void GetConnection(Action<SqlConnection> action);
	IDataReader GetReader(CommandDefinition command);
  }
}

using System;
using System.Data;
using System.Data.Common;
using Dapper;

namespace WebAppMonitor.Core
{
  public interface IDbConnectionProvider
  {

    void GetConnection(Action<DbConnection> action);
    void GetConnection(IsolationLevel isolationLevel, Action<DbConnection, DbTransaction> action);
	IDataReader GetReader(CommandDefinition command);
  }
}

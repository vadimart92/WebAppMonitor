using System;
using System.Data;
using System.Data.Common;
using Dapper;

namespace WebAppMonitor.Core
{
  public interface IDbConnectionProvider
  {

    void GetConnection(Action<DbConnection> action);
	IDataReader GetReader(CommandDefinition command);
  }
}

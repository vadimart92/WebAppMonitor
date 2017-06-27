using System;
using System.Data.SqlClient;

namespace WebAppMonitor.Core
{
  public interface IDbConnectionProvider
  {

    void GetConnection(Action<SqlConnection> action);

  }
}

using System;
using System.Data.SqlClient;

namespace WebAppMonitor.Common
{
  public interface IDbConnectionProvider
  {

    void GetConnection(Action<SqlConnection> action);

  }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppMonitor.Common
{
  public interface IDbConnectionProvider
  {

    void GetConnection(Action<SqlConnection> action);

  }
}

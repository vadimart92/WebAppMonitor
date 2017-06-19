using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppMonitor.Common
{
	public class DbConnectionProviderImpl : IDbConnectionProvider
	{

		private string _cs;

		public DbConnectionProviderImpl(string cs) {
			_cs = cs;
		}

		public void GetConnection(Action<SqlConnection> action) {
			using (var connection = new SqlConnection(_cs)) {
				connection.Open();
				action(connection);
			}
		}

	}
}

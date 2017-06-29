using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace WebAppMonitor.Core
{
	public class DbConnectionProviderImpl : IDbConnectionProvider
	{

		private readonly string _cs;

		public DbConnectionProviderImpl(string cs) {
			_cs = cs;
		}

		public void GetConnection(Action<SqlConnection> action) {
			using (var connection = new SqlConnection(_cs)) {
				connection.Open();
				action(connection);
			}
		}

		public IDataReader GetReader(CommandDefinition command) {
			var connection = new SqlConnection(_cs);
			return connection.ExecuteReader(command, CommandBehavior.CloseConnection);
		}
	}
}

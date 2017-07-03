using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Dapper;

namespace WebAppMonitor.Core
{
	public class DbConnectionProviderImpl : IDbConnectionProvider
	{

		private readonly string _cs;

		public DbConnectionProviderImpl(string cs) {
			_cs = cs;
			Ping();
		}

		public void GetConnection(Action<DbConnection> action) {
			using (var connection = GetConnection()) {
				action(connection);
			}
		}

		public void GetConnection(IsolationLevel isolationLevel, Action<DbConnection, DbTransaction> action) {
			using (SqlConnection connection = GetConnection()) {
				using (DbTransaction transaction = connection.BeginTransaction(isolationLevel)) {
					action(connection, transaction);
					transaction.Commit();
				}
			}
		}

		public IDataReader GetReader(CommandDefinition command) {
			var connection = GetConnection();
			return connection.ExecuteReader(command, CommandBehavior.CloseConnection);
		}

		private SqlConnection GetConnection() {
			var connection = new SqlConnection(_cs);
			connection.Open();
			return connection;
		}

		private void Ping() {
			using (SqlConnection connection = GetConnection()) {
				if (new SqlCommand("SELECT @@version") {
					Connection = connection
				}.ExecuteScalar() == null) {
					throw new ArgumentException("invalid connection string");
				}
			}
		}
	}
}

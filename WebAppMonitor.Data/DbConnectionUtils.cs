using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using FastMember;
using WebAppMonitor.Core;

namespace WebAppMonitor.Data
{
	public interface IRecordWithDate
	{

		DateTime Date
		{
			get; set;
		}

	}

	public static class DbConnectionUtils
	{
		public static void BulkInsert<T>(this DbConnection connection, IEnumerable<T> items) where T : class {
			if (IsItemsEmpty(items)) {
				return;
			}
			Insert(connection, items);
		}

		public static void BulkInsert<T>(this IEnumerable<T> items, IDbConnectionProvider connectionProvider,
				bool checkConstraints = true)
			where T : class {
			if (IsItemsEmpty(items)) {
				return;
			}
			connectionProvider.GetConnection(connection => {
				Insert(connection, items, checkConstraints);
			});
		}

		private static void Insert<T>(DbConnection connection, IEnumerable<T> items, bool checkConstraints = true)
			where T : class {
			string tableName = OrmUtils.GetTableName<T>();
			var options = SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction;
			if (checkConstraints) {
				options = options | SqlBulkCopyOptions.CheckConstraints;
			}
			using (var sqlBulkCopy = new SqlBulkCopy(connection.ConnectionString, options)) {
				sqlBulkCopy.DestinationTableName = tableName;
				sqlBulkCopy.EnableStreaming = true;
				foreach (var columnName in OrmUtils.GetColumnNames<T>()) {
					sqlBulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(columnName, columnName));
				}
				using (var reader = ObjectReader.Create(items)) {
					sqlBulkCopy.WriteToServer(reader);
				}
			}
		}


		public static DateTime GetLastQueryDate<T>(this IDbConnectionProvider connectionProvider) where T : class, IRecordWithDate {
			string tableName = OrmUtils.GetTableName<T>();
			DateTime result = DateTime.MinValue;
			connectionProvider.GetConnection(connection => {
				result = connection.ExecuteScalar<DateTime>($"SELECT MAX(Date) FROM [{tableName}]");
			});
			return result;
		}

		private static bool IsItemsEmpty<T>(IEnumerable<T> items) where T : class {
			var itemsList = items as IList<T>;
			if (!itemsList?.Any() ?? false) {
				return true;
			}
			return false;
		}

	}
}

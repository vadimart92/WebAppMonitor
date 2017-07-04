using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using FastMember;
using WebAppMonitor.Core;

namespace WebAppMonitor.Data
{

	public static class DbConnectionUtils
	{
		public static void BulkInsert<T>(this DbConnection connection, IEnumerable<T> items) where T : class {
			if (IsItemsEmpty(items)) {
				return;
			}
			Insert(connection, items);
		}

		private static void Insert<T>(DbConnection connection, IEnumerable<T> items) where T : class {
			string tableName = OrmUtils.GetTableName<T>();
			using (var sqlBulkCopy = new SqlBulkCopy(connection.ConnectionString,
				SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.FireTriggers 
				| SqlBulkCopyOptions.UseInternalTransaction)) {
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

		public static void BulkInsert<T>(this IEnumerable<T> items, IDbConnectionProvider connectionProvider) 
				where T : class {
			if (IsItemsEmpty(items)) {
				return;
			}
			connectionProvider.GetConnection(connection => {
				Insert(connection, items);
			});
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

using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using FastMember;
using Joti.Data.Helper;
using WebAppMonitor.Core;

namespace WebAppMonitor.Data
{

	public static class DbConnectionUtils
	{
		public static void BulkInsert<T>(this DbConnection connection, IEnumerable<T> items) where T : class {
			if (IsItemsEmpty(items)) {
				return;
			}
			string tableName = OrmUtils.GetTableName<T>();
			Joti.Connection.Bulk.BulkDbConnectionExtensions.BulkInsert(connection, items, tableName);
		}

		public static void BinaryBulkInsert<T>(this IEnumerable<T> items, IDbConnectionProvider connectionProvider)
			where T : class {
			if (IsItemsEmpty(items)) {
				return;
			}
			string tableName = OrmUtils.GetTableName<T>();
			var dataTable = items.ToDataTable(tableName, true);
			var binaryColumns = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Where(pi => pi.PropertyType == typeof(byte[])).ToList();
			foreach (PropertyInfo column in binaryColumns) {
				dataTable.AddColumn(column.Name, typeof(byte[]));
			}
			using (var reader = ObjectReader.Create(items)) {
				dataTable.Load(reader);
			}
			connectionProvider.GetConnection(connection => {
				Joti.Connection.Bulk.BulkDbConnectionExtensions.BulkInsert(connection, dataTable);
			});
		}

		public static void BulkInsert<T>(this IEnumerable<T> items, IDbConnectionProvider connectionProvider) 
				where T : class {
			if (IsItemsEmpty(items)) {
				return;
			}
			string tableName = OrmUtils.GetTableName<T>();
			connectionProvider.GetConnection(connection => {
				Joti.Connection.Bulk.BulkDbConnectionExtensions.BulkInsert(connection, items, tableName);
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Dapper;

namespace WebAppMonitor.Data
{
	

	public static class DbConnectionUtils
	{
		public static void BulkInsert<T>(this DbConnection connection, IEnumerable<T> items) where T : class {
			var itemsList = items as IList<T>;
			if (!itemsList?.Any() ?? false) {
				return;
			}
			Type itemType = typeof(T);
			var tableAttr = itemType.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute;
			string tableName = tableAttr != null ? tableAttr.Name : typeof(T).Name;
			Joti.Connection.Bulk.BulkDbConnectionExtensions.BulkInsert(connection, items, tableName);
		}
	}
}

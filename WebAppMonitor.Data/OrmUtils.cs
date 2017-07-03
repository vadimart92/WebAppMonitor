using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace WebAppMonitor.Data
{
	public static class OrmUtils
	{

		public static string GetTableName<T>() where T : class {
			Type itemType = typeof(T);
			var tableAttr = itemType.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute;
			string tableName = tableAttr != null ? tableAttr.Name : typeof(T).Name;
			return tableName;
		}

	}
}
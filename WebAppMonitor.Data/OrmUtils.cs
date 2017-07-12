using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data
{
	public static class OrmUtils
	{
		private static readonly ConcurrentDictionary<Type, string> TableNameCahe = new ConcurrentDictionary<Type, string>();
		private static readonly ConcurrentDictionary<Type, IEnumerable<string>> ColumnNameCahe = new ConcurrentDictionary<Type, IEnumerable<string>>();
		public static string GetTableName<T>() where T : class {
			Type itemType = typeof(T);
			if (TableNameCahe.TryGetValue(itemType, out var result)) {
				return result;
			}
			var tableAttr = itemType.GetCustomAttribute(typeof(TableAttribute), false) as TableAttribute;
			string tableName = tableAttr != null ? tableAttr.Name : itemType.Name;
			TableNameCahe[itemType] = tableName;
			return tableName;
		}

		public static IEnumerable<string> GetColumnNames<T>() where T : class {
			Type itemType = typeof(T);
			if (ColumnNameCahe.TryGetValue(itemType, out var result)) {
				return result;
			}
			var columnNames = typeof(T)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Select(pi => pi.GetColumnName())
				.ToList();
			ColumnNameCahe[itemType] = columnNames;
			return columnNames;
		}

		public static string GetColumnName(this PropertyInfo propertyInfo) {
			var attribute = propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), false) as ColumnAttribute;
			string columnName = attribute != null ? attribute.Name : propertyInfo.Name;
			return columnName;
		}

		public static T CreateInfoRecord<T>(this IDateRepository dateRepository, DateTime timeStamp)
				where T : BaseInfoRecord, new() {
			return new T {
				Id = Guid.NewGuid(),
				Date = timeStamp,
				DateId = dateRepository.GetDayId(timeStamp)
			};
		}

	}
}
﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data
{

	public class IdColumnAttribute: ColumnAttribute
	{
		public IdColumnAttribute(string name):base(name) {
			
		}
	}
	public class HashColumnAttribute: ColumnAttribute
	{
		public HashColumnAttribute(string name):base(name) {

		}
	}

	public static class OrmUtils
	{
		private static readonly ConcurrentDictionary<Type, string> TableNameCahe = new ConcurrentDictionary<Type, string>();
		private static readonly ConcurrentDictionary<Type, IEnumerable<Tuple<string, string>>> ColumnNameCahe = new ConcurrentDictionary<Type, IEnumerable<Tuple<string, string>>>();
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

		public static IEnumerable<Tuple<string,string>> GetColumnNames<T>() where T : class {
			Type itemType = typeof(T);
			if (ColumnNameCahe.TryGetValue(itemType, out var result)) {
				return result;
			}
			var columnNames = typeof(T)
				.GetProperties(BindingFlags.Public | BindingFlags.Instance)
				.Select(pi => Tuple.Create(pi.Name, pi.GetColumnName()))
				.ToList();
			ColumnNameCahe[itemType] = columnNames;
			return columnNames;
		}

		public static string GetColumnName(this PropertyInfo propertyInfo) {
			var attribute = propertyInfo.GetCustomAttribute(typeof(ColumnAttribute), false) as ColumnAttribute;
			string columnName = attribute != null ? attribute.Name : propertyInfo.Name;
			return columnName;
		}

		public static string GetIdColumnName<TType>() {
			return GetColumnName<TType, IdColumnAttribute>();
		}
		public static string GetHashColumnName<TType>() {
			return GetColumnName<TType, HashColumnAttribute>();
		}
		private static string GetColumnName<TType, TColumnAttribute>() where TColumnAttribute: ColumnAttribute {
			var mi = (from member in typeof(TType).GetMembers()
							where member.GetCustomAttributes<TColumnAttribute>().Count() == 1
							select new { Member=member, Attribute=member.GetCustomAttribute<TColumnAttribute>()}
						).Single();
			return String.IsNullOrWhiteSpace(mi.Attribute.Name)? mi.Member.Name : mi.Attribute.Name;
		}

		public static T CreateInfoRecord<T>(this IDateRepository dateRepository, DateTime timeStamp)
				where T : BaseInfoRecord, new() {
			return new T {
				Id = Guid.NewGuid(),
				Date = timeStamp,
				DateId = dateRepository.GetDayId(timeStamp)
			};
		}

		public static string GetHashQueryText<T>() where T : class {
			var idColumn = GetIdColumnName<T>();
			var hashColumn = GetHashColumnName<T>();
			var tableName = GetTableName<T>();
			return $"SELECT [{idColumn}] as [Id], [{hashColumn}] as [Hash] FROM [{tableName}]";
		}
		
	}
}
using System;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;
using WebAppMonitor.Core;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data
{
	internal class SimpleLookupRepository<TLookup> where TLookup: BaseLookup, new()
	{

		private Dictionary<string, Guid> _itemsMapDictionary ;

		private Dictionary<string, Guid> ItemsMapDictionary => _itemsMapDictionary ?? InitItemsMap();

		private readonly IDbConnectionProvider _connectionProvider;
		private readonly string _lookupName = typeof(TLookup).Name;

		private Dictionary<string, Guid> InitItemsMap() {
			_itemsMapDictionary = new Dictionary<string, Guid>();
			_connectionProvider.GetConnection(connection => {
				string sql = $@"SELECT Id, Code FROM {_lookupName}";
				foreach (BaseLookup baseLookup in connection.Query<BaseLookup>(sql)) {
					_itemsMapDictionary[baseLookup.Code] = baseLookup.Id;
				}
			});
			return _itemsMapDictionary;
		}

		public SimpleLookupRepository(IDbConnectionProvider connectionProvider) {
			_connectionProvider = connectionProvider;
		}

		public Guid GetId(string code) {
			if (!ItemsMapDictionary.ContainsKey(code)) {
				var item = new TLookup {
					Id = Guid.NewGuid(),
					Code = code
				};
				ItemsMapDictionary[code] = item.Id;
				_connectionProvider.GetConnection(connection => {
					connection.Insert(item);
				});
			}
			return ItemsMapDictionary[code];
		}

	}
}
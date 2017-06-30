using System;
using System.Collections.Generic;
using Dapper;
using WebAppMonitor.Core;
using WebAppMonitor.Data.Entities;
using Z.Dapper.Plus;

namespace WebAppMonitor.Data
{
	internal class SimpleLookupManager<TLookup> where TLookup: BaseLookup, new()
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

		public SimpleLookupManager(IDbConnectionProvider connectionProvider) {
			_connectionProvider = connectionProvider;
			DapperPlusManager.Entity<TLookup>()
				.Table(_lookupName)
				.Key(x => x.Id);
		}

		public Guid GetId(string code) {
			if (!ItemsMapDictionary.ContainsKey(code)) {
				var item = new TLookup {
					Id = Guid.NewGuid(),
					Code = code
				};
				ItemsMapDictionary[code] = item.Id;
				_connectionProvider.GetConnection(connection => {
					connection.BulkInsert(item);
				});
			}
			return ItemsMapDictionary[code];
		}

	}
}
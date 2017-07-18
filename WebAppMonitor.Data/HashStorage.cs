using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {

	public class HashStorage<TData> where TData : BaseHashStorage, new() {
		private readonly ILogger _logger;
		private readonly IDbConnectionProvider _dbConnection;
		private readonly ISimpleDataProvider _dataProvider;
		private readonly ResettableLazy<HashSet<byte[]>> _hashStorage;
		private readonly List<TData> _pendingHashes = new List<TData>();
		private readonly string _tableName;

		public HashStorage(ISimpleDataProvider dataProvider,
				IDbConnectionProvider dbConnection, ILogger logger) {
			_dataProvider = dataProvider;
			_dbConnection = dbConnection;
			_logger = logger;
			_hashStorage = new ResettableLazy<HashSet<byte[]>>(InitHashSet);
			_tableName = OrmUtils.GetTableName<TData>();
		}

		private HashSet<byte[]> InitHashSet() {
			var res = new HashSet<byte[]>(ByteArrayComparer.Instance);
			string query = $"SELECT [Hash] FROM [{_tableName}]";
			foreach (BaseHashStorage item in _dataProvider.Enumerate<BaseHashStorage>(query)) {
				res.Add(item.Hash);
			}
			return res;
		}

		public bool Add(byte[] value) {
			if (_hashStorage.Value.Add(value)) {
				var item = new TData {
					Id = Guid.NewGuid(),
					Hash = value
				};
				_pendingHashes.Add(item);
				return true;
			}
			return false;
		}

		public void Flush() {
			if (_pendingHashes.Count == 0) {
				return;
			}
			_pendingHashes.BulkInsert(_dbConnection);
			_logger.LogInformation("Inserted {0} hashes for {1}", _pendingHashes.Count, _tableName);
			_pendingHashes.Clear();
		}
	}

	public class HashStorage<TData, TStorage> {
		private readonly IDbConnectionProvider _dbConnection;
		private readonly SHA512 _hasher = SHA512.Create();
		private readonly ResettableLazy<Dictionary<byte[], TData>> _map;
		private Dictionary<byte[], TData> NormQueryMap => _map.Value;

		public HashStorage(Func<TStorage, Tuple<byte[], TData>> mapFunc, string query, 
				IDbConnectionProvider dbConnection) {
			_dbConnection = dbConnection;
			_map = new ResettableLazy<Dictionary<byte[], TData>>(()=> InitMap(mapFunc, query));
		}

		public void Reset() {
			_map.Reset();
		}

		public TData GetOrCreate(string text, Func<byte[], TData> func) {
			var hash = GetHash(text);
			if (NormQueryMap.ContainsKey(hash)) {
				return NormQueryMap[hash];
			}
			TData data = func(hash);
			NormQueryMap[hash] = data;
			return data;
		}

		internal byte[] GetHash(string query) {
			var bytes = query != null ? Encoding.UTF8.GetBytes(query) : new byte[0];
			return _hasher.ComputeHash(bytes);
		}

		private Dictionary<byte[], TData> InitMap(Func<TStorage, Tuple<byte[], TData>> mapFunc, string query) {
			var result = new Dictionary<byte[], TData>(ByteArrayComparer.Instance);
			_dbConnection.GetConnection(connection => {
				var items = connection.Query<TStorage>(query);
				foreach (TStorage storageItem in items) {
					var map = mapFunc(storageItem);
					result[map.Item1] = map.Item2;
				}
			});
			return result;
		}
	}
}

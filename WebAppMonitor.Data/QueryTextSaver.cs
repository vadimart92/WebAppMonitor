using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Dapper;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class QueryTextSaver : IQueryTextSaver {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly List<NormQueryTextHistory> _pendingQueries = new List<NormQueryTextHistory>();
		private readonly List<NormQueryTextSource> _pendingQuerySources = new List<NormQueryTextSource>();
		private readonly SHA512 _hasher = SHA512.Create();
		private readonly Lazy<Dictionary<byte[], Guid>> _normQueryMap;
		private Dictionary<byte[], Guid> NormQueryMap => _normQueryMap.Value;
		private volatile bool _isWorking;
		private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
		private readonly Logger<QueryTextSaver> _logger;

		public QueryTextSaver(IDbConnectionProvider connectionProvider, Logger<QueryTextSaver> logger) {
			_connectionProvider = connectionProvider;
			_logger = logger;
			_normQueryMap = new Lazy<Dictionary<byte[], Guid>>(InitNormQueryMap);
		}

		private Dictionary<byte[], Guid> InitNormQueryMap() {
			var result = new Dictionary<byte[], Guid>(ByteArrayComparer.Instance);
			_connectionProvider.GetConnection(connection => {
				var histories = connection.Query<NormQueryTextHistory>(@"SELECT Id, QueryHash FROM NormQueryTextHistory");
				foreach (NormQueryTextHistory queryTextHistory in histories) {
					result[queryTextHistory.QueryHash] = queryTextHistory.Id;
				}
			});
			return result;
		}

		internal byte[] GetQueryHash(string query) {
			var bytes = query != null ? Encoding.UTF8.GetBytes(query) : new byte[0];
			return _hasher.ComputeHash(bytes);
		}

		private void PushQueryItem(NormQueryTextHistory historyItem, Guid querySourceId) {
			_pendingQueries.Add(historyItem);
			_pendingQuerySources.Add(new NormQueryTextSource {
				QuerySourceId = querySourceId,
				NormQueryTextId = historyItem.Id
			});
			if (_pendingQueries.Count > 500) {
				SaveItems();
			}
		}

		public void BeginWork() {
			_logger.LogInformation("BeginWork");
			_autoResetEvent.WaitOne();
			_isWorking = true;
		}

		public Guid GetOrCreate(string queryText, Guid? querySourceId) {
			if (!_isWorking) {
				throw new InvalidOperationException();
			}
			var hash = GetQueryHash(queryText);
			if (NormQueryMap.ContainsKey(hash)) {
				return NormQueryMap[hash];
			}
			var historyItem = new NormQueryTextHistory {
				Id = Guid.NewGuid(),
				QueryHash = hash,
				NormalizedQuery = queryText
			};
			if (querySourceId.HasValue) {
				PushQueryItem(historyItem, querySourceId.Value);
			}
			NormQueryMap[hash] = historyItem.Id;
			return historyItem.Id;
		}

		public void Flush() {
			if (!_isWorking) {
				throw new InvalidOperationException();
			}
			SaveItems();
			_isWorking = false;
			_autoResetEvent.Set();
			_logger.LogInformation("Flush complete");
		}

		private void SaveItems() {
			_pendingQueries.BulkInsert(_connectionProvider);
			_pendingQuerySources.BulkInsert(_connectionProvider);
			_pendingQueries.Clear();
			_pendingQuerySources.Clear();
		}
	}
}
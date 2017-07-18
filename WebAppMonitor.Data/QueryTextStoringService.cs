using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class QueryTextStoringService : BaseSynchronizedWorker, IQueryTextStoringService {
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly List<NormQueryTextHistory> _pendingQueries = new List<NormQueryTextHistory>();
		private readonly List<NormQueryTextSource> _pendingQuerySources = new List<NormQueryTextSource>();
		private readonly HashStorage<Guid, NormQueryTextHistory> _queryMap;
		private readonly SimpleLookupRepository<QuerySource> _querySourceRepository;

		public QueryTextStoringService(IDbConnectionProvider connectionProvider, ILogger<QueryTextStoringService> logger) : base(logger) {
			_connectionProvider = connectionProvider;
			_queryMap = new HashStorage<Guid, NormQueryTextHistory>(item => Tuple.Create(item.QueryHash, item.Id),
				@"SELECT Id, QueryHash FROM NormQueryTextHistory", _connectionProvider);
			_querySourceRepository = new SimpleLookupRepository<QuerySource>(connectionProvider);
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

		public Guid GetOrCreate(string queryText, string querySource) {
			EnsureWorkingState();
			return _queryMap.GetOrCreate(queryText, hash => {
				var historyItem = new NormQueryTextHistory {
					Id = Guid.NewGuid(),
					QueryHash = hash,
					NormalizedQuery = queryText
				};
				PushQueryItem(historyItem, _querySourceRepository.GetId(querySource));
				return historyItem.Id;
			});
		}

		protected override void SaveItems() {
			if (_pendingQueries.Count > 0) {
				_pendingQueries.BulkInsert(_connectionProvider);
				Logger.LogInformation("{0} queries inserted", _pendingQueries.Count);
				_pendingQueries.Clear();
			}
			_pendingQuerySources.BulkInsert(_connectionProvider);
			_pendingQuerySources.Clear();
		}
	}
}
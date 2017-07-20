using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {

	public class QueryTextStoringService : StringItemsStoringService<Guid, NormQueryTextHistory>, IQueryTextStoringService {
		private readonly List<NormQueryTextSource> _pendingQuerySources = new List<NormQueryTextSource>();
		private readonly SimpleLookupRepository<QuerySource> _querySourceRepository;
		private readonly IDbConnectionProvider _connectionProvider;


		public QueryTextStoringService(ILogger<QueryTextStoringService> logger,
			IDbConnectionProvider connectionProvider) : base(logger, connectionProvider) {
			_connectionProvider = connectionProvider;
			_querySourceRepository = new SimpleLookupRepository<QuerySource>(connectionProvider);
		}

		protected override void SaveItems() {
			base.SaveItems();
			_pendingQuerySources.BulkInsert(_connectionProvider);
			_pendingQuerySources.Clear();
		}

		public Guid GetOrCreate(string queryText, string querySource) {
			return GetOrCreate(queryText, hash => {
				var historyItem = new NormQueryTextHistory {
					Id = Guid.NewGuid(),
					HashValue = hash,
					NormalizedQuery = queryText
				};
				_pendingQuerySources.Add(new NormQueryTextSource {
					QuerySourceId = _querySourceRepository.GetId(querySource),
					NormQueryTextId = historyItem.Id
				});
				return historyItem;
			});
		}
	}

}
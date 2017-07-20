using System;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class StackStoringService : StringItemsStoringService<Guid, Stack>, IStackStoringService {
		private readonly SimpleLookupRepository<StackSource> _stackSourceRepository;

		public StackStoringService(ILogger<StackStoringService> logger, IDbConnectionProvider connectionProvider)
			: base(logger, connectionProvider) {
			_stackSourceRepository = new SimpleLookupRepository<StackSource>(connectionProvider);
		}

		public Guid GetOrCreate(string stackTrace, string source) {
			return GetOrCreate(stackTrace, hash => new Stack {
				Id = Guid.NewGuid(),
				SourceId = _stackSourceRepository.GetId(source),
				HashValue = hash,
				StackTrace = stackTrace
			});
		}

	}
}

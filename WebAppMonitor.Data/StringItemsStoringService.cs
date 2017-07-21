using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;

namespace WebAppMonitor.Data {
	public abstract class StringItemsStoringService<TKey, TItemContainer> : BaseSynchronizedWorker where TItemContainer : class, IEntityWithHash<TKey> {
		protected readonly MemoryHashStorage<TKey, TItemContainer> ItemsMap;
		private readonly IDbConnectionProvider _connectionProvider;
		protected readonly List<TItemContainer> PendingItems = new List<TItemContainer>();
		protected virtual int ItemsToFlush => 500;

		protected StringItemsStoringService(ILogger<StringItemsStoringService<TKey, TItemContainer>> logger,
			IDbConnectionProvider connectionProvider) : base(logger) {
			_connectionProvider = connectionProvider;
			ItemsMap = new MemoryHashStorage<TKey, TItemContainer>(connectionProvider);
		}

		protected override void SaveItems() {
			if (PendingItems.Count > 0) {
				PendingItems.BulkInsert(_connectionProvider, false);
				Logger.LogInformation($"{PendingItems.Count} {typeof(TItemContainer).Name} inserted");
				PendingItems.Clear();
			}
		}

		protected void AddItem(TItemContainer itemCode) {
			PendingItems.Add(itemCode);
			if (PendingItems.Count > ItemsToFlush) {
				SaveItems();
			}
		}

		protected TKey GetOrCreate(string value, Func<byte[], TItemContainer> createItem) {
			EnsureWorkingState();
			return ItemsMap.GetOrCreate(value, hash => {
				var item = createItem(hash);
				AddItem(item);
				return item.Id;
			});
		}
	}
}
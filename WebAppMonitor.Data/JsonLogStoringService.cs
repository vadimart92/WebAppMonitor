using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;
using WebAppMonitor.Data.Entities;
using WebAppMonitor.DataProcessing;

namespace WebAppMonitor.Data {
	public class JsonLogStoringService: IJsonLogStoringService
	{

		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IQueryTextStoringService _queryTextStoringService;
		private readonly IStackStoringService _stackStoringService;
		private readonly IDateRepository _dateRepository;
		private readonly List<ReaderLog> _pendingReaderLogs = new List<ReaderLog>();
		private readonly List<ExecutorLog> _pendingExecutorLogs = new List<ExecutorLog>();
		private readonly ILogger _logger;
		private readonly IPerfomanceItemCodeStoringService _codeStoringService;
		private readonly HashStorage<ImportedReaderLogRecord> _importedReaderLogs;
		private readonly HashStorage<ImportedExecutorLogRecord> _importedExecutorLogs;
		private readonly HashStorage<ImportedPerfomanceLogRecord> _importedPerfLogs;

		public JsonLogStoringService(IQueryTextStoringService queryTextStoringService, IDbConnectionProvider connectionProvider,
				IDateRepository dateRepository, IStackStoringService stackStoringService, ILogger<JsonLogStoringService> logger,
				ISimpleDataProvider dataProvider, IPerfomanceItemCodeStoringService codeStoringService) {
			_queryTextStoringService = queryTextStoringService;
			_connectionProvider = connectionProvider;
			_dateRepository = dateRepository;
			_stackStoringService = stackStoringService;
			_logger = logger;
			_codeStoringService = codeStoringService;
			_importedReaderLogs = new HashStorage<ImportedReaderLogRecord>(dataProvider, connectionProvider, logger);
			_importedExecutorLogs = new HashStorage<ImportedExecutorLogRecord>(dataProvider, connectionProvider, logger);
			_importedPerfLogs = new HashStorage<ImportedPerfomanceLogRecord>(dataProvider, connectionProvider, logger);
		}

		private void Start() {
			_queryTextStoringService.BeginWork();
			_stackStoringService.BeginWork();
			_codeStoringService.BeginWork();
		}

		private void Flush() {
			_queryTextStoringService.Flush();
			_stackStoringService.Flush();
			_codeStoringService.Flush();
			if (_pendingReaderLogs.Any()) {
				_pendingReaderLogs.BulkInsert(_connectionProvider);
				_logger.LogInformation("{0} reader logs inserted", _pendingReaderLogs.Count);
				_pendingReaderLogs.Clear();
			}
			_importedReaderLogs.Flush();
			if (_pendingExecutorLogs.Any()) {
				_pendingExecutorLogs.BulkInsert(_connectionProvider);
				_logger.LogInformation("{0} executor logs inserted", _pendingExecutorLogs.Count);
				_pendingExecutorLogs.Clear();
			}
			_importedExecutorLogs.Flush();

			_importedPerfLogs.Flush();
		}

		public ITransaction BeginWork() {
			return new ActionTransaction(Start, Flush);
		}

		public void RegisterReaderLogItem(ReaderLogRecord logRecord) {
			var hash = logRecord.GetSourceLogHash();
			if (!_importedReaderLogs.Add(hash)) {
				return;
			}
			ReaderLogRecord.Messageobject msg = logRecord.MessageObject;
			if (msg == null)return;
			string query = msg.Sql.ExtractLogSqlText();
			Guid textId = _queryTextStoringService.GetOrCreate(query, "ReaderLog");
			string stackTrace = msg.StackTrace.NormalizeReaderStack();
			Guid stackId = _stackStoringService.GetOrCreate(stackTrace, "ReaderLog");
			var item = _dateRepository.CreateInfoRecord<ReaderLog>(logRecord.Date);
			item.QueryId = textId;
			item.StackId = stackId;
			item.Rows = msg.RowsAffected?.Sum() ?? 0;
			_pendingReaderLogs.Add(item);
		}

		public void RegisterExecutorLogRecord(ExecutorLogRecord logRecord) {
			var hash = logRecord.GetSourceLogHash();
			if (!_importedExecutorLogs.Add(hash)) {
				return;
			}
			ExecutorLogRecord.Messageobject msg = logRecord.MessageObject;
			if (msg == null) return;
			string query = msg.Sql.ExtractLogSqlText();
			Guid textId = _queryTextStoringService.GetOrCreate(query, "ExecutorLog");
			string stackTrace = msg.StackTrace.NormalizeExecutorStack();
			Guid stackId = _stackStoringService.GetOrCreate(stackTrace, "ExecutorLog");
			var item = _dateRepository.CreateInfoRecord<ExecutorLog>(logRecord.Date);
			item.QueryId = textId;
			item.StackId = stackId;
			item.Duration = msg.Duration;
			_pendingExecutorLogs.Add(item);
		}

		public void RegisterPerfomanceLogItem(PerfomanceLogRecord logRecord) {
			var hash = logRecord.GetSourceLogHash();
			if (!_importedPerfLogs.Add(hash)) {
				return;
			}
			PerfomanceLogRecord.Messageobject msg = logRecord.MessageObject;
			if (msg == null) return;
			var item = _dateRepository.CreateInfoRecord<PerfomanceLogInfoRecord>(logRecord.Date);
			item.Id = msg.Id;
			item.ParentId = msg.ParentId;
			item.Duration = msg.Duration;
			item.CodeId = _codeStoringService.AddCode(msg.Code);
		}

	}
}

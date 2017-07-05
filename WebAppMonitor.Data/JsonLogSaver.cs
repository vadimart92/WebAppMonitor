using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data {
	public class JsonLogSaver: IJsonLogSaver
	{

		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IQueryTextSaver _queryTextSaver;
		private readonly IStackSaver _stackSaver;
		private readonly SimpleLookupManager<QuerySource> _querySourceRepository;
		private ResettableLazy<Guid> ReaderLogQuerySourceId => new ResettableLazy<Guid>(
			() => _querySourceRepository.GetId("ReaderLog"));
		private readonly ResettableLazy<DateTime> _lastReaderLogRecord;
		private readonly IDateRepository _dateRepository;
		private readonly List<ReaderLog> _pendingReaderLogs = new List<ReaderLog>();

		public JsonLogSaver(IQueryTextSaver queryTextSaver, IDbConnectionProvider connectionProvider,
				IDateRepository dateRepository, IStackSaver stackSaver) {
			_queryTextSaver = queryTextSaver;
			_connectionProvider = connectionProvider;
			_dateRepository = dateRepository;
			_stackSaver = stackSaver;
			_querySourceRepository = new SimpleLookupManager<QuerySource>(connectionProvider);
			_lastReaderLogRecord = new ResettableLazy<DateTime>(connectionProvider.GetLastQueryDate<ReaderLog>);
		}

		private void Start() {
			_queryTextSaver.BeginWork();
			_stackSaver.BeginWork();
		}

		private void Flush() {
			_queryTextSaver.Flush();
			_stackSaver.Flush();
			_pendingReaderLogs.BulkInsert(_connectionProvider);
			_pendingReaderLogs.Clear();
			_lastReaderLogRecord.Reset();
		}

		public ITransaction BeginWork() {
			return new ActionTransaction(Start, Flush);
		}

		public void RegisterReaderLogItem(ReaderLogRecord logRecord) {
			if (logRecord.Date < _lastReaderLogRecord.Value) {
				return;
			}
			Messageobject msg = logRecord.MessageObject;
			Guid textId = _queryTextSaver.GetOrCreate(msg.Sql, ReaderLogQuerySourceId.Value);
			Guid stackId = _stackSaver.GetOrCreate(msg.StackTrace);
			var item = _dateRepository.CreateInfoRecord<ReaderLog>(logRecord.Date);
			item.QueryId = textId;
			item.StackId = stackId;
			item.Rows = msg.RowsAffected.Sum();
			_pendingReaderLogs.Add(item);
		}

	}
}

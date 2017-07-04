using System;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.DataProcessing {
	public class ExtendedEventLoader : IExtendedEventLoader {
		private readonly IExtendedEventParser _parser;
		private readonly IExtendedEventDataSaver _dataSaver;
		private readonly ISettingsProvider _settingsProvider;

		public ExtendedEventLoader(IExtendedEventParser parser, IExtendedEventDataSaver dataSaver,
				ISettingsProvider settingsProvider) {
			_parser = parser;
			_dataSaver = dataSaver;
			_settingsProvider = settingsProvider;
		}

		public void LoadLongLocksData(string file) {
			string databaseName = _settingsProvider.DatabaseName;
			_dataSaver.BeginWork();
			foreach (QueryLockInfo queryLockInfo in _parser.ReadLongLockEvents(file)) {
				if (databaseName.Equals(queryLockInfo.DatabaseName, StringComparison.OrdinalIgnoreCase)) {
					_dataSaver.RegisterLock(queryLockInfo);
				}
			}
			_dataSaver.Flush();
		}

		public void LoadDeadLocksData(string file) {
			string databaseName = _settingsProvider.DatabaseName;
			_dataSaver.BeginWork();
			foreach (QueryDeadLockInfo queryLockInfo in _parser.ReadDeadLockEvents(file)) {
				if (queryLockInfo.ObjectAName.Contain(databaseName) ||
						queryLockInfo.ObjectBName.Contain(databaseName)) {
					_dataSaver.RegisterDeadLock(queryLockInfo);
				}
			}
			_dataSaver.Flush();
		}
	}
}
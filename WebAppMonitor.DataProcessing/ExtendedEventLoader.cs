﻿using System;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.DataProcessing {
	public class ExtendedEventLoader : IExtendedEventLoader {
		private readonly IExtendedEventParser _parser;
		private readonly IExtendedEventDataSaver _dataSaver;
		private readonly ISettings _settings;

		public ExtendedEventLoader(IExtendedEventParser parser, IExtendedEventDataSaver dataSaver,
				ISettings settings) {
			_parser = parser;
			_dataSaver = dataSaver;
			_settings = settings;
		}

		public void LoadLongLocksData(string file) {
			string databaseName = _settings.DatabaseName;
			using (_dataSaver.BeginWork()) {
				foreach (QueryLockInfo queryLockInfo in _parser.ReadLongLockEvents(file)) {
					if (databaseName.Equals(queryLockInfo.DatabaseName, StringComparison.OrdinalIgnoreCase)) {
						_dataSaver.RegisterLock(queryLockInfo);
					}
				}
			}
		}

		public void LoadDeadLocksData(string file) {
			string databaseName = _settings.DatabaseName;
			using (_dataSaver.BeginWork()) {
				foreach (QueryDeadLockInfo queryLockInfo in _parser.ReadDeadLockEvents(file)) {
					if (queryLockInfo.ObjectAName.Contain(databaseName) ||
						queryLockInfo.ObjectBName.Contain(databaseName)) {
						_dataSaver.RegisterDeadLock(queryLockInfo);
					}
				}
			}
		}
	}
}
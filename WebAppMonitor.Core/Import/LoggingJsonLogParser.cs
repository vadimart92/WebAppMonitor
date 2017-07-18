using System.Collections.Generic;

namespace WebAppMonitor.Core.Import
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Linq;
	using Microsoft.Extensions.Logging;

	public class LoggingJsonLogParser: IJsonLogParser
	{

		private ILogger<LoggingJsonLogParser> _log;

		private readonly IJsonLogParser _instance;

		public LoggingJsonLogParser(ILogger<LoggingJsonLogParser> log, IJsonLogParser instance) {
			_log = log;
			_instance = instance;
		}

		public IEnumerable<T> ReadFile<T>(string filePath)
			where T : IJsonLogWithHash {
			_log.LogInformation("Parsing of {0} in {1} started", typeof(T).Name, 
				Path.GetFileNameWithoutExtension(filePath));
			IEnumerator<T> sourceEnumerator = _instance.ReadFile<T>(filePath).GetEnumerator();
			var enumerator = new LoggingEnumerator<T>(sourceEnumerator, _log);
			return new LoggingEnumerable<T>(enumerator);
		}

		class LoggingEnumerable<T>: IEnumerable<T>
		{

			private readonly IEnumerator<T> _instance;

			public LoggingEnumerable(IEnumerator<T> instance) {
				_instance = instance;
			}

			public IEnumerator<T> GetEnumerator() {
				return _instance;
			}

			IEnumerator IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}

		}

		class LoggingEnumerator<T> : IEnumerator<T> {

			int _counter = 0;
			DateTime _lastLogDate = DateTime.Now;
			private readonly IEnumerator<T> _sourceEnumerator;
			private readonly ILogger<LoggingJsonLogParser> _log;

			public LoggingEnumerator(IEnumerator<T> sourceEnumerator, ILogger<LoggingJsonLogParser> log) {
				_sourceEnumerator = sourceEnumerator;
				_log = log;
			}
			public void Dispose() {
				_log.LogInformation("Total {0} items were parsed", _counter - 1);
			}

			public bool MoveNext() {
				do {
					try {
						_counter++;
						if (DateTime.Now - _lastLogDate > TimeSpan.FromMinutes(1)) {
							_lastLogDate = DateTime.Now;
							_log.LogInformation("{0} items were parsed", _counter);
						}
						return _sourceEnumerator.MoveNext();
					} catch(Exception e) {
						_log.LogError(new EventId(0), e, "Error while reading line {0}", _counter);
					}
				} while (true);
			}

			public void Reset() {
				_sourceEnumerator.Reset();
			}

			public T Current => _sourceEnumerator.Current;

			object IEnumerator.Current => _sourceEnumerator.Current;

		}
	}
}

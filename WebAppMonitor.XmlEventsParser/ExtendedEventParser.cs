using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.DataProcessing;

namespace WebAppMonitor.XmlEventsParser
{
	public class QueryLockInfoRow
	{

		public string XML { get; set; }

	}
	public class ExtendedEventParser: IExtendedEventParser {
		private readonly ISimpleDataProvider _simpleDataProvider;
		private readonly ILogger<ExtendedEventParser> _logger;

		public ExtendedEventParser(ISimpleDataProvider simpleDataProvider, ILogger<ExtendedEventParser> logger) {
			_simpleDataProvider = simpleDataProvider;
			_logger = logger;
		}

		public IEnumerable<QueryLockInfo> ReadEvents(string file) {
			var query = @"SELECT CAST(event_data AS XML) XML FROM sys.fn_xe_file_target_read_file(@fileName, NULL, NULL, NULL)";
			var parser = new ReportParser();
			foreach (QueryLockInfoRow row in _simpleDataProvider.Enumerate<QueryLockInfoRow>(query, new { fileName = file })) {
				QueryLockInfo info;
				try {
					info = GetQueryLockInfo(parser, row);
				} catch (Exception e) {
					_logger.LogError(new EventId(1), e, "Error while parsing long locks xml record: {0}", row.XML);
					continue;
				}
				yield return info;
			}
		}

		private static QueryLockInfo GetQueryLockInfo(ReportParser parser, QueryLockInfoRow row) {
			var info = parser.Parse(row.XML);
			string durationStr = GetDataValue(info, "duration");
			long duration;
			long.TryParse(durationStr, out duration);
			eventDataValueBlockedprocessreport processesInfo =
				GetDataItem(info, "blocked_process").value.blockedprocessreport;
			string blockedText = processesInfo.blockedprocess.process.inputbuf;
			string blockerText = processesInfo.blockingprocess.process.inputbuf;
			return new QueryLockInfo {
				TimeStamp = info.timestamp,
				Duration = duration,
				LockMode = GetDataItem(info, "lock_mode")?.text,
				Blocked = new Proess {
					Text = blockedText.ExtractLongLocksSqlText()
				},
				Blocker = new Proess {
					Text = blockerText.ExtractLongLocksSqlText()
				},
				SourceXml = row.XML
			};
		}

		private static string GetDataValue(@event info, string propertyName) {
			eventData data = GetDataItem(info, propertyName);
			return data?.value.Text.FirstOrDefault();
		}

		private static eventData GetDataItem(@event info, string propertyName) {
			return info.data.FirstOrDefault(d => propertyName.Equals(d.name, StringComparison.OrdinalIgnoreCase));
		}
	}
}

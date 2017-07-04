using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.Core.Import;
using WebAppMonitor.DataProcessing;

namespace WebAppMonitor.XmlEventsParser
{
	public class XmlRow
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

		private IEnumerable<XmlRow> ReadEventData(string file) {
			var query = @"SELECT CAST(event_data AS XML) XML FROM sys.fn_xe_file_target_read_file(@fileName, NULL, NULL, NULL)";
			return _simpleDataProvider.Enumerate<XmlRow>(query, new { fileName = file });
		}

		public IEnumerable<QueryLockInfo> ReadLongLockEvents(string file) {
			var parser = new LongLockParser();
			foreach (XmlRow row in ReadEventData(file)) {
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

		public IEnumerable<QueryDeadLockInfo> ReadDeadLockEvents(string file) {
			var parser = new DeadLockParser();
			foreach (XmlRow row in ReadEventData(file)) {
				QueryDeadLockInfo info;
				try {
					var parsedInfo = parser.Parse(row.XML);
					var queryAText = parsedInfo.data.value.deadlock.processlist.First()?.inputbuf;
					var queryBText = parsedInfo.data.value.deadlock.processlist.Last()?.inputbuf;
					info = new QueryDeadLockInfo {
						TimeStamp = parsedInfo.timestamp,
						QueryA = queryAText.ExtractLocksSqlText(),
						QueryB = queryBText.ExtractLocksSqlText()
					};
					var objects = parsedInfo.data.value.deadlock.resourcelist?
						.SelectNodes("//@objectname")?.OfType<XmlNode>().ToList();
					if (objects != null) {
						info.ObjectAName = objects.FirstOrDefault()?.InnerText;
						info.ObjectBName = objects.LastOrDefault()?.InnerText;
					}
				} catch (Exception e) {
					_logger.LogError(new EventId(1), e, "Error while parsing deadlocks xml record: {0}", row.XML);
					continue;
				}
				yield return info;
			}
		}

		private static QueryLockInfo GetQueryLockInfo(LongLockParser parser, XmlRow row) {
			var info = parser.Parse(row.XML);
			string durationStr = GetDataValue(info, "duration");
			long.TryParse(durationStr, out long duration);
			eventDataValueBlockedprocessreport processesInfo =
				GetDataItem(info, "blocked_process").value.blockedprocessreport;
			string blockedText = processesInfo.blockedprocess.process.inputbuf;
			string blockerText = processesInfo.blockingprocess.process.inputbuf;
			return new QueryLockInfo {
				TimeStamp = info.timestamp,
				Duration = duration,
				LockMode = GetDataItem(info, "lock_mode")?.text,
				DatabaseName = GetDbName(info),
				Blocked = new Proess {
					Text = blockedText.ExtractLocksSqlText()
				},
				Blocker = new Proess {
					Text = blockerText.ExtractLocksSqlText()
				},
				SourceXml = row.XML
			};
		}

		private static string GetFirstObjectName(Deadlocks.@event info) {
			var dbName = info.data.value.deadlock.resourcelist;
			return dbName.ToString();
		}
		private static string GetLatsObjectName(Deadlocks.@event info) {
			var dbName = info.data.value.deadlock.resourcelist;
			return dbName.ToString();
		}

		private static string GetDbName(@event info) {
			return GetDataItem(info, "database_name")?.value.Text.FirstOrDefault();
		}

		private static string GetDataValue(@event info, string propertyName) {
			eventData data = GetDataItem(info, propertyName);
			return data?.value.Text.FirstOrDefault();
		}

		private static eventData GetDataItem(@event info, string propertyName) {
			var item = info.data.FirstOrDefault(d => propertyName.Equals(d.name, StringComparison.OrdinalIgnoreCase));
			return item;
		}

	}
}

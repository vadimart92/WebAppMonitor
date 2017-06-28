using System;
using System.Collections.Generic;
using System.Linq;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Entities;

namespace WebAppMonitor.XmlEventsParser
{
	public class ExtendedEventParser: IExtendedEventParser {
		private readonly ISimpleDataProvider _simpleDataProvider;

		public ExtendedEventParser(ISimpleDataProvider simpleDataProvider) {
			_simpleDataProvider = simpleDataProvider;
		}

		public IEnumerable<QueryLockInfo> ReadEvents(string file) {
			var query = @"SELECT CAST(event_data AS XML) XML FROM sys.fn_xe_file_target_read_file(@fileName, NULL, NULL, NULL)";
			var parser = new ReportParser();
			foreach (var row in _simpleDataProvider.Enumerate<string>(query, new { fileName = file })) {
				var info = parser.Parse(row);
				var durationStr = GetDataValue(info, "duration");
				ulong duration;
				ulong.TryParse(durationStr, out duration);
				var processesInfo = GetDataItem(info, "blocked_process").value.blockedprocessreport;
				var blockedText = processesInfo.blockedprocess.process.inputbuf;
				var blockerText = processesInfo.blockingprocess.process.inputbuf;
				yield return new QueryLockInfo {
					TimeStamp = info.timestamp,
					Duration = duration,
					LockMode = GetDataValue(info, "lock_mode"),
					Blocked = new Proess {
						Text = blockedText
					},
					Blocker = new Proess {
						Text =  blockerText
					}
			};
			}
		}

		private static string GetDataValue(@event info, string propertyName) {
			var data = GetDataItem(info, propertyName);
			return data?.value.Text.FirstOrDefault();
		}

		private static eventData GetDataItem(@event info, string propertyName) {
			return info.data.FirstOrDefault(d => propertyName.Equals(d.name, StringComparison.OrdinalIgnoreCase));
		}
	}
}

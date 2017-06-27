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
			foreach (var row in _simpleDataProvider.Enumerate<string>(query, new { fileName = file })) {
				var info = ReportDeserializer.Parse(row);
				var durationStr = info.data.Where(d => "duration".Equals(d.name, StringComparison.OrdinalIgnoreCase)).Select(d=>d.value.Text).FirstOrDefault();
				ulong duration;
				ulong.TryParse(durationStr?.FirstOrDefault(), out duration);
				yield return new QueryLockInfo {
					TimeStamp = info.timestamp,
					Duration = duration
				};
			}
		}
	}
}

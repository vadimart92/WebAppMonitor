using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WebAppMonitor.Core.JsonLogParser;

namespace WebAppMonitor.Core.Tests {

	#region Class: JsonLogParserTestCase

	[TestFixture]
	public class JsonLogParserTestCase {

		#region Methods: Public

		[Test]
		public void ReadFile_ReaderLogRecord() {
			string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "LoggingDataReader.json.0.json");
			var parser = new JsonLogParser.JsonLogParser();
			var data = parser.ReadFile<ReaderLogRecord>(file).ToList();
			data.Count.Should().Be(392);
			data.Any(d => string.IsNullOrWhiteSpace(d.MessageObject.Sql) || 
				string.IsNullOrWhiteSpace(d.MessageObject.StackTrace))
			.Should().BeFalse();
			data.Any(d => d.Date == default(DateTime)).Should().BeFalse();
		}

		#endregion

	}

	#endregion

}

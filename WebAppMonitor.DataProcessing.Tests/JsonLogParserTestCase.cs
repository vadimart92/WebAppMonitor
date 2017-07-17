using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.DataProcessing.Tests {

	#region Class: JsonLogParserTestCase

	[TestFixture]
	public class JsonLogParserTestCase {

		#region Methods: Public

		[Test]
		public void ReadFile_ReaderLogRecord() {
			string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "LoggingDataReader.json.0.json");
			var parser = new Json.JsonLogParser();
			var data = parser.ReadFile<ReaderLogRecord>(file).ToList();
			data.Count.Should().Be(392);
			data.Any(d => string.IsNullOrWhiteSpace(d.MessageObject.Sql) || 
				string.IsNullOrWhiteSpace(d.MessageObject.StackTrace))
			.Should().BeFalse();
			data.Any(d => d.Date == default(DateTime)).Should().BeFalse();
			data.Any(d => d.GetSourceLogHash() == null).Should().BeFalse();
		}

		[Test]
		public void ReadFile_ExecutorLogRecord() {
			string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "Sql.0.json");
			var parser = new Json.JsonLogParser();
			var data = parser.ReadFile<ExecutorLogRecord>(file).ToList();
			data.Count.Should().Be(18);
			data.Any(d => string.IsNullOrWhiteSpace(d.Exception) && (string.IsNullOrWhiteSpace(d.MessageObject.Sql) || 
				string.IsNullOrWhiteSpace(d.MessageObject.StackTrace))).Should().BeFalse();
			data.Any(d => d.Date == default(DateTime)).Should().BeFalse();
			data.Any(d => d.GetSourceLogHash() == null).Should().BeFalse();
		}

		#endregion

	}

	#endregion

}

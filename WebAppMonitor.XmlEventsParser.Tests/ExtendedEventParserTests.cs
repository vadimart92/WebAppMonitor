using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.XmlEventsParser.Tests {
	[TestFixture]
	public class ExtendedEventParserTests {
		private static string GetFilePath(string filePath) {
			return Path.Combine(TestContext.CurrentContext.TestDirectory, filePath);
		}

		private IEnumerable<XmlRow> ReadXmlLines(string fileName) {
			using (var reader = new JsonTextReader(new StreamReader(GetFilePath(fileName)))) {
				if (!reader.Read()) {
					yield break;
				}
				while (reader.Read()) {
					if (reader.TokenType != JsonToken.StartObject) {
						yield break;
					}
					while (reader.TokenType != JsonToken.PropertyName) {
						reader.Read();
					}
					yield return new XmlRow { XML = reader.ReadAsString() };
					reader.Read();
				}
			}
		}

		[Test, AutoNSubstituteData]
		public void ReadLongLockEvents(ISimpleDataProvider dataProvider, ILogger<ExtendedEventParser> logger) {
			dataProvider.Enumerate<XmlRow>(Arg.Any<string>(), Arg.Any<object>()).Returns(ReadXmlLines("data.json"));
			var p = new ExtendedEventParser(dataProvider, logger);
			var events = p.ReadLongLockEvents("someFile");
			var result = events.ToList();
			result.Should().NotBeEmpty();
			result.Any(r => string.IsNullOrWhiteSpace(r.Blocker.Text)).Should().BeFalse();
			result.Any(r => string.IsNullOrWhiteSpace(r.Blocked.Text)).Should().BeFalse();
			result.Any(r => r.Duration == 0).Should().BeFalse();
			result.Any(r => !string.IsNullOrWhiteSpace(r.DatabaseName)).Should().BeTrue();
		}

		[Test, AutoNSubstituteData]
		public void ReadDeadlocksEvents(ISimpleDataProvider dataProvider, ILogger<ExtendedEventParser> logger) {
			dataProvider.Enumerate<XmlRow>(Arg.Any<string>(), Arg.Any<object>()).Returns(ReadXmlLines("deadlockData.json"));
			var p = new ExtendedEventParser(dataProvider, logger);
			var events = p.ReadDeadLockEvents("someFile");
			var result = events.ToList();
			result.Should().NotBeEmpty();
			result.Any(r => string.IsNullOrWhiteSpace(r.QueryA)).Should().BeFalse();
			result.Any(r => string.IsNullOrWhiteSpace(r.QueryB)).Should().BeFalse();
			result.Any(r => r.TimeStamp == DateTime.MinValue).Should().BeFalse();
			result.Any(r => !string.IsNullOrWhiteSpace(r.ObjectAName) && !string.IsNullOrWhiteSpace(r.ObjectBName)).Should().BeTrue();
		}
	}
}

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
using WebAppMonitor.Core;

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
		public void ReadEvents(ISimpleDataProvider dataProvider, ILogger<ExtendedEventParser> logger) {
			dataProvider.Enumerate<XmlRow>(Arg.Any<string>(), Arg.Any<object>()).Returns(ReadXmlLines("data.json"));
			var p = new ExtendedEventParser(dataProvider, logger);
			var events = p.ReadLongLockEvents("someFile");
			var result = events.ToList();
			bool emptyBlockers = result.Any(r => string.IsNullOrWhiteSpace(r.Blocker.Text));
			bool emptyBlocked = result.Any(r => string.IsNullOrWhiteSpace(r.Blocked.Text));
			bool emptyDuration = result.Any(r => r.Duration == 0);
			Assert.IsFalse(emptyBlockers);
			Assert.IsFalse(emptyBlocked);
			Assert.IsFalse(emptyDuration);
			result.Should().NotBeEmpty();
		}

		[Test, AutoNSubstituteData]
		public void ReadDeadlocksEvents(ISimpleDataProvider dataProvider, ILogger<ExtendedEventParser> logger) {
			dataProvider.Enumerate<XmlRow>(Arg.Any<string>(), Arg.Any<object>()).Returns(ReadXmlLines("deadlockData.json"));
			var p = new ExtendedEventParser(dataProvider, logger);
			var events = p.ReadDeadLockEvents("someFile");
			var result = events.ToList();
			bool emptyBlockers = result.Any(r => string.IsNullOrWhiteSpace(r.QueryA));
			bool emptyBlocked = result.Any(r => string.IsNullOrWhiteSpace(r.QueryB));
			bool emptyDuration = result.Any(r => r.TimeStamp == DateTime.MinValue);
			Assert.IsFalse(emptyBlockers);
			Assert.IsFalse(emptyBlocked);
			Assert.IsFalse(emptyDuration);
			result.Should().NotBeEmpty();
		}
	}
}

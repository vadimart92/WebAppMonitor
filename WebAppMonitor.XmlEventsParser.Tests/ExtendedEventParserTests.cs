using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofixture.NUnit3;
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

		private IEnumerable<string> ReadXmlLines() {
			using (var reader = new JsonTextReader(new StreamReader(GetFilePath("data.json")))) {
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
					yield return reader.ReadAsString();
					reader.Read();
				}
			}
		}

		[Test, AutoNSubstituteData]
		public void ReadEvents(ISimpleDataProvider dataProvider) {
			dataProvider.Enumerate<string>(Arg.Any<string>(), Arg.Any<object>()).Returns(ReadXmlLines());
			var p = new ExtendedEventParser(dataProvider);
			var events = p.ReadEvents("someFile");
			var result = events.ToList();
			var emptyBlockers = result.Any(r => string.IsNullOrWhiteSpace(r.Blocker.Text));
			var emptyBlocked = result.Any(r => string.IsNullOrWhiteSpace(r.Blocked.Text));
			var emptyDuration = result.Any(r => r.Duration == 0;
			Assert.IsFalse(emptyBlockers);
			Assert.IsFalse(emptyBlocked);
			Assert.IsFalse(emptyDuration);
		}
	}
}

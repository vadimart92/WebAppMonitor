using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace WebAppMonitor.Data.Tests {
	[TestFixture]
	public class ByteArrayComparerTestCase {

		[Test]
		public void CheckColisions() {
			var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "LoggingDataReader.json.0.json");
			var hashCodes = new List<int>();
			foreach (string line in File.ReadAllLines(file)) {
				hashCodes.Add(ByteArrayComparer.Instance.GetHashCode(Encoding.UTF8.GetBytes(line)));
			}
			hashCodes.Distinct().Count().Should().Be(hashCodes.Count);
		}
	}
}

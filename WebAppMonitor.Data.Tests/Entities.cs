using FluentAssertions;
using NUnit.Framework;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data.Tests {
	[TestFixture]
	public class Entities {
		[Test]
		public void GetHashQueryText() {
			OrmUtils.GetHashQueryText<Stack>().Should().Be("SELECT [Id], [StackHash] FROM [Stack]");
			OrmUtils.GetHashQueryText<NormQueryTextHistory>().Should().Be("SELECT [Id], [QueryHash] FROM [NormQueryTextHistory]");
			OrmUtils.GetHashQueryText<PerformanceItemCode>().Should().Be("SELECT [Id], [CodeHash] FROM [PerformanceItemCode]");
		}
		[Test]
		public void GetColumnNames() {
			OrmUtils.GetColumnNames<Stack>().Should().BeEquivalentTo("Id", "SourceId", "StackTrace", "StackHash");
		}
	}
}

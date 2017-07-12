
using FluentAssertions;
using NUnit.Framework;
using WebAppMonitor.Core.Common;

namespace WebAppMonitor.Core.Tests
{
	[TestFixture()]
    public class StringUtilsTestCase
    {
		[Test]
		[TestCase("", "a", false)]
		[TestCase(null, "a", false)]
		[TestCase("adw.rtet.v", "adw", true)]
		[TestCase("adw.rtet.v", "Adw", true)]
		[TestCase("adw.rtet.v", "rtet", true)]
	    public void Contain(string value, string find, bool result) {
			value.Contain(find).Should().Be(result);
		}
    }
}

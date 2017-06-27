using System.IO;
using NUnit.Framework;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.NUnit3;
using WebAppMonitor.Core;

namespace WebAppMonitor.XmlEventsParser.Tests
{
    [TestFixture]
    public class ExtendedEventParserTests
    {
        [Test, AutoData]
        public void ReadEvents(int x) {
	        //[Substitute]
	        ISimpleDataProvider dataProvider = null;

			var p = new ExtendedEventParser(dataProvider);
	        var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "collect_long_locks.xel");
	        var events = p.ReadEvents(filePath);
        }
    }
}

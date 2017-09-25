namespace WebAppMonitor.Core.Tests
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using Autofixture.NUnit3;
	using FluentAssertions;
	using Microsoft.Extensions.Logging;
	using NSubstitute;
	using NUnit.Framework;
	using WebAppMonitor.Core.Import;

	[TestFixture]
    class DataFilePathProviderTestCase
    {
		[Test, AutoNSubstituteData]
		public void Get(IDateTimeProvider timeProvider, ILogger<DataFilePathProvider> logger, IFileSystem fileSystem) {
			var settings = Substitute.ForPartsOf<AutoSettings>();
			fileSystem.GetTempDirectoryPath().Returns(Path.GetTempPath());
			var sut = new DataFilePathProvider(settings, timeProvider, logger, fileSystem);
			string resultDir = TestContext.CurrentContext.TestDirectory;
			var expectedFiles = new[] {
				"collect_deadlock_data_123.xel",
				"collect_long_locks_data_123dfds.xel",
				"ts_sqlprofiler_05_sec_asdv.xel"
			};
			string xEventsDir = CreateTestFiles(timeProvider, expectedFiles);
			var now = DateTime.UtcNow.AddHours(1);
			timeProvider.UtcNow.Returns(now);
			settings.EventsDataDirectoryTemplate.Returns(xEventsDir);
			List<string> files = new List<string>();
			foreach (string dir in sut.GetDailyExtEventsDirs()) {
				resultDir = dir;
				files.AddRange(Directory.EnumerateFiles(dir).Select(Path.GetFileName));
			}
			files.Should().BeEquivalentTo(expectedFiles);
			DirectoryAssert.DoesNotExist(resultDir);
		}

	    private static string CreateTestFiles(IDateTimeProvider timeProvider, string[] expectedFiles) {
		    var today = DateTime.Today;
		    timeProvider.Today.Returns(today);
		    var xEventsDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "xEvents");
		    if (Directory.Exists(xEventsDir)) {
			    Directory.Delete(xEventsDir, true);
		    }
		    Directory.CreateDirectory(xEventsDir);
		    foreach (string fileName in expectedFiles.Concat(new[] {
				"ts_sqlprofiler_05_sec_asdv.xel.bak",
				"ts_sqlprofiler_asdv.xel"
			})) {
			    var path = Path.Combine(xEventsDir, fileName);
			    File.WriteAllText(path, "test");
		    }
		    return xEventsDir;
	    }

    }
}

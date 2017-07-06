using System;
using System.IO;
using System.Linq;
using Autofixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Core.Tests
{
	[TestFixture]
    public class JsonLogFilePathProviderTestCase
    {
		[Test, AutoNSubstituteData]
	    public void GetExecutorLogs(ISettings settings, ILogger logger) {
			settings.DailyLogsDirectoryTemplate.Returns("yyyy_MM_dd");
			settings.ExecutorLogFileName.Returns("Sql.0.json");
			var dirPrefix = TestContext.CurrentContext.TestDirectory;
			settings.DirectoriesWithJsonLog.Returns($"{dirPrefix}\\TestDir\\LogDir3;{dirPrefix}\\TestDir\\LogDir2;{dirPrefix}\\TestDir\\LogDir1");
		    var sut = new DataFilePathProvider(settings, new StaticDateTimeProvider(new DateTime(2017,07,13)), logger);
			var expectedDirs = new [] {
				""
			}.Select(d=>Path.Combine(TestContext.CurrentContext.TestDirectory, d)).ToList();
			//provider.
			var actual = sut.GetExecutorLogs().ToList();
			actual.Should().BeEquivalentTo(expectedDirs);
		}
    }
}

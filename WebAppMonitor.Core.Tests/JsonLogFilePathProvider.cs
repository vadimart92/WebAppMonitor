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
			DataFilePathProvider sut = SetupProvider(settings, logger);
			settings.ExecutorLogFileName.Returns("LoggingDataReader.json.0.json");
			var counter = 0;
			foreach (string executorLog in sut.GetExecutorLogs()) {
				AssertValidFile(executorLog);
				counter++;
			}
			counter.Should().Be(4);
		}

	    private static void AssertValidFile(string executorLog) {
		    File.ReadAllText(executorLog).Should().Be("8d8adbe6-894e-4f64-b92b-e6641f10bfc3");
	    }

	    [Test, AutoNSubstituteData]
	    public void GetReaderLogs(ISettings settings, ILogger logger) {
			DataFilePathProvider sut = SetupProvider(settings, logger);
		    settings.ReaderLogFileName.Returns("LoggingDataReader.json.0.json");
			var counter = 0;
			foreach (string executorLog in sut.GetReaderLogs()) {
				AssertValidFile(executorLog);
				counter++;
			}
			counter.Should().Be(4);
		}

		[Test, AutoNSubstituteData]
	    public void GetPerfomanceLogs(ISettings settings, ILogger logger) {
			DataFilePathProvider sut = SetupProvider(settings, logger);
			settings.PerfomanceLogFileName.Returns("LoggingDataReader.json.0.json");
			var counter = 0;
			foreach (string executorLog in sut.GetPerfomanceLogs()) {
				AssertValidFile(executorLog);
				counter++;
			}
			counter.Should().Be(4);
		}

	    private static DataFilePathProvider SetupProvider(ISettings settings, ILogger logger) {
		    settings.DailyLogsDirectoryTemplate.Returns("yyyy_MM_dd");
		    string dirPrefix = TestContext.CurrentContext.TestDirectory;
		    settings.DirectoriesWithJsonLog.Returns(
			    $"{dirPrefix}\\TestDir\\LogDir3;{dirPrefix}\\TestDir\\LogDir2;{dirPrefix}\\TestDir\\LogDir1");
		    var sut = new DataFilePathProvider(settings, new StaticDateTimeProvider(new DateTime(2017, 07, 13)), logger);
		    return sut;
	    }

    }
}

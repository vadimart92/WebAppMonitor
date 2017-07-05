using System.Collections.Generic;
using Autofixture.NUnit3;
using NSubstitute;
using NUnit.Framework;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.DataProcessing.Tests {
	
	[TestFixture]
	public class AppLogLoaderTestCase {

	[Test, AutoNSubstituteData]
		public void LoadReaderLogs(IJsonLogParser parser, IJsonLogStoringService storingService, string fileName) {
			var logRecord = new ReaderLogRecord{Logger = "test"};
			parser.ReadFile<ReaderLogRecord>(fileName).Returns(new List<ReaderLogRecord> {logRecord});
			var loader = new AppLogLoader(parser, storingService);
			loader.LoadReaderLogs(fileName);
			storingService.Received(1).BeginWork();
			storingService.Received(1).RegisterReaderLogItem(logRecord);
		}
	}
}

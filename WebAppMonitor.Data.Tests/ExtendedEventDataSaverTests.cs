using System;
using Autofixture.NUnit3;
using NUnit.Framework;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.Data.Tests
{
	[TestFixture]
    public class ExtendedEventDataSaverTests
	{
		private static byte[] StringToByteArray(String hex) {
			int NumberChars = hex.Length;
			byte[] bytes = new byte[NumberChars / 2];
			for (int i = 0; i < NumberChars; i += 2)
				bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
			return bytes;
		}

		[Test, Category("PreCommit")]
		[AutoNSubstituteData]
		public void GetQueryHash(IDbConnectionProvider connectionProvider, QueryStatsContext queryStatsContext) {
			var sut = new ExtendedEventDataSaver(connectionProvider, queryStatsContext);
			var query =
				@"UPDATE [dbo].[SysProcessData] SET [ModifiedOn] = @Now, [PropertiesData] = @P1 WHERE [Id] = @Id";
			var expectedvalue = StringToByteArray("BD54F39D47CAE9B82133084321748F00C6827C3FE68AC8CFCCEBA7C9A25149" +
			                                      "371A8A183A9179B5FA23460C90A9181D9A1C2CAE32EFF696A636AE7D1422A8B584");
			var actual = sut.GetQueryHash(query);
			actual.Should().BeEquivalentTo(expectedvalue);
		}

		[Test, Category("PreCommit"), Ignore("not completed")]
		public void RegisterLock(IDbConnectionProvider connectionProvider, QueryStatsContext queryStatsContext) {
			var sut = new ExtendedEventDataSaver(connectionProvider, queryStatsContext);
			sut.RegisterLock(new QueryLockInfo());
			sut.Flush();
		}
    }
}

using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace WebAppMonitor.DataProcessing.Tests
{
	[TestFixture]
    public class SqlExtractionUtilsTests
	{

		private static IEnumerable<object> TestCaseSources {
			get {
				yield return new TestCaseData(null, null);
				yield return new TestCaseData("()aa", "()aa");
				yield return new TestCaseData(@"(adad
				)ax
a", @"(adad )ax a");
				yield return new TestCaseData(@"
delete from [dbo].Activity where CreatedOn = '2100-01-01'   ", 
@"delete from [dbo].Activity where CreatedOn = '2100-01-01'");
				yield return new TestCaseData(@"
(@Id uniqueidentifier,@Now datetime2(7),@P1 varbinary(1634))
UPDATE [dbo].[SysProcessData]
SET
	[ModifiedOn] = @Now,
	[PropertiesData] = @P1
WHERE
	[Id] = @Id   ",
	@"UPDATE [dbo].[SysProcessData] SET [ModifiedOn] = @Now, [PropertiesData] = @P1 WHERE [Id] = @Id");
			}
		}

		[Test, Category("PreCommit")]
		[TestCaseSource(nameof(TestCaseSources))]
		public void ExtractLongLocksSqlText(string sourceString, string expectedString) {
			string actual = sourceString.ExtractLongLocksSqlText();
			actual.Should().BeEquivalentTo(expectedString);
		}
		
    }
}

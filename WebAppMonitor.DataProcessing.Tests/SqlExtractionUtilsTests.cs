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
				yield return new TestCaseData(@"(asd delete from [dbo].Activity where CreatedOn = '2100-01-01'   ",
@"(asd delete from [dbo].Activity where CreatedOn = '2100-01-01'");
				yield return new TestCaseData(@"
(@Id uniqueidentifier,@Now datetime2(7),@P1 varbinary(1634))
UPDATE [dbo].[SysProcessData]
SET
	[ModifiedOn] = @Now,
	[PropertiesData] = @P1
WHERE
	[Id] = @Id   ",
	@"UPDATE [dbo].[SysProcessData] SET [ModifiedOn] = @Now, [PropertiesData] = @P1 WHERE [Id] = @Id");
	yield return new TestCaseData(@"(@schemaName nvarchar(19),@lastSyncVersion datetime2(7),@localStoreId uniqueidentifier,@remoteStoreId uniqueidentifier,@currentUserContactId uniqueidentifier,@utcDateTimeNow datetime2(7))EXEC [dbo].[tsp_ActualizeSysSyncMetaData] @schemaName, @lastSyncVersion, @localStoreId, @remoteStoreId, @currentUserContactId, @utcDateTimeNow   ",
	@"EXEC [dbo].[tsp_ActualizeSysSyncMetaData] @schemaName, @lastSyncVersion, @localStoreId, @remoteStoreId, @currentUserContactId, @utcDateTimeNow");
	yield return new TestCaseData(@"(@P1 uniqueidentifier,@P2 datetime2(7),@P3 uniqueidentifier,@P4 nvarchar(112),@P5 uniqueidentifier,@P6 uniqueidentifier,@P7 datetime2(7),@P8 nvarchar(19),@P9 int,@P10 nvarchar(4000),@P11 int,@P12 int)
UPDATE [dbo].[SysSyncMetaData] WITH(ROWLOCK)
SET
	[ModifiedOn] = @P2,
	[ModifiedById] = @P3,
	[RemoteId] = @P4,
	[RemoteStoreId] = @P5,
	[ModifiedInStoreId] = @P6,
	[Version] = @P7,
	[RemoteItemName] = @P8,
	[SchemaOrder] = @P9,
	[ExtraParameters] = @P10,
	[LocalState] = @P11,
	[RemoteState] = @P12
WHERE
	[Id] = @P1   ", @"UPDATE [dbo].[SysSyncMetaData] WITH(ROWLOCK) SET [ModifiedOn] = @P2, [ModifiedById] = @P3, [RemoteId] = @P4, [RemoteStoreId] = @P5, [ModifiedInStoreId] = @P6, [Version] = @P7, [RemoteItemName] = @P8, [SchemaOrder] = @P9, [ExtraParameters] = @P10, [LocalState] = @P11, [RemoteState] = @P12 WHERE [Id] = @P1");
				yield return new TestCaseData(@"(@p1 int)
SELECT
	[SysSchema].[Name] [Name],
	MAX([SysLocalizableValue].[ModifiedOn]) [MaxModifiedOn]
FROM
	[dbo].[SysLocalizableValue] WITH(NOLOCK)
	INNER JOIN [dbo].[SysSchema] WITH(NOLOCK) ON ([SysSchema].[Id] = [SysLocalizableValue].[SysSchemaId])
WHERE
	[SysLocalizableValue].[SysPackageId] IN (
SELECT
	[SysPackage].[Id]
FROM
	[dbo].[SysPackage] WITH(NOLOCK)
WHERE
	[SysPackage].[SysWorkspaceId] = @P1)
	AND [SysSchema].[ManagerName] = @P2
GROUP BY
	[SysSchema].[Name]", @"SELECT [SysSchema].[Name] [Name], MAX([SysLocalizableValue].[ModifiedOn]) [MaxModifiedOn] FROM [dbo].[SysLocalizableValue] WITH(NOLOCK) INNER JOIN [dbo].[SysSchema] WITH(NOLOCK) ON ([SysSchema].[Id] = [SysLocalizableValue].[SysSchemaId]) WHERE [SysLocalizableValue].[SysPackageId] IN ( SELECT [SysPackage].[Id] FROM [dbo].[SysPackage] WITH(NOLOCK) WHERE [SysPackage].[SysWorkspaceId] = @P1) AND [SysSchema].[ManagerName] = @P2 GROUP BY [SysSchema].[Name]");
			}
	}

		[Test, Category("PreCommit")]
		[TestCaseSource(nameof(TestCaseSources))]
		public void ExtractLongLocksSqlText(string sourceString, string expectedString) {
			string actual = sourceString.ExtractLocksSqlText();
			actual.Should().BeEquivalentTo(expectedString);
		}
		
    }
}

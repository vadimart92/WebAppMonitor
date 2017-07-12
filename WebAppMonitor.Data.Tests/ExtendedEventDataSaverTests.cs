using NUnit.Framework;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Import;
using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.Data.Tests
{
	[TestFixture]
    public class ExtendedEventDataSaverTests
	{

		[Test, Category("PreCommit"), Ignore("not completed")]
		public void RegisterLock(IDbConnectionProvider connectionProvider, 
				IQueryTextStoringService textStoringService, IDateRepository dateRepository) {
			ExtendedEventDataSaver sut = null;
			sut.RegisterLock(new QueryLockInfo());
			
		}
    }
}

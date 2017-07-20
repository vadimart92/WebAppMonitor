using System;
using System.Collections.Generic;
using Autofixture.NUnit3;
using NUnit.Framework;
using WebAppMonitor.Core;
using WebAppMonitor.Data.Entities;

namespace WebAppMonitor.Data.Tests
{
	[TestFixture]
	public class DbConnectionUtilsTests
	{
		[Test, Category("PreCommit")]
		[AutoNSubstituteData]
		public void BinaryBulkInsert(IDbConnectionProvider connectionProvider) {
			var sut = new List<NormQueryTextHistory> {
				new NormQueryTextHistory{Id = Guid.NewGuid(), HashValue = new byte[]{1,2,3}, NormalizedQuery = "asd"}
			};
			sut.BulkInsert(connectionProvider);
		}
	}
}

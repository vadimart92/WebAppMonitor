using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Ploeh.AutoFixture.NUnit3;
using WebAppMonitor.Core.Common;

namespace WebAppMonitor.Core.Tests
{
	[TestFixture]
	public class ResettableLazyTests
	{
		[Test, AutoData]
		public void Value() {
			var func = Substitute.For<Func<bool>>();
			func.Invoke().Returns(true);
			var rl = new ResettableLazy<bool>(func);
			rl.Value.Should().BeTrue();
			func.Received(1).Invoke();
			rl.Value.Should().BeTrue();
			func.Received(1).Invoke();
			rl.Reset();
			rl.Value.Should().BeTrue();
			func.Received(2).Invoke();
		}
	}
}

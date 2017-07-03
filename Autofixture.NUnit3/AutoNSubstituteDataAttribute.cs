using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.NUnit3;

namespace Autofixture.NUnit3
{
	public class AutoNSubstituteDataAttribute : AutoDataAttribute
	{
		public AutoNSubstituteDataAttribute()
			: base(CreateFixture())
		{
		}

		public static IFixture CreateFixture()
		{
			return new Fixture().Customize(new AutoNSubstituteCustomization());
		}
	}
}

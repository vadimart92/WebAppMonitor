using System;

namespace WebAppMonitor.Core.Import {
	public interface IDateTimeProvider {
		DateTime Today { get; }
	}

	public class CurrentDateTimeProvider : IDateTimeProvider {
		public DateTime Today => DateTime.Today;
	}

	public class StaticDateTimeProvider : IDateTimeProvider {
		public StaticDateTimeProvider(DateTime today) {
			Today = today;
		}

		public DateTime Today { get; }
	}
}
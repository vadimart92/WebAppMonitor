using System;

namespace WebAppMonitor.Core.Import {
	public interface IDateTimeProvider {
		DateTime Today { get; }
		DateTime UtcNow { get; }
	}

	public class CurrentDateTimeProvider : IDateTimeProvider {
		public DateTime Today => DateTime.Today;

		public DateTime UtcNow => DateTime.UtcNow;

	}

	public class StaticDateTimeProvider : IDateTimeProvider {
		public StaticDateTimeProvider(DateTime utcNow) {
			Today = utcNow.Date;
			UtcNow = utcNow;
		}

		public DateTime Today { get; }

		public DateTime UtcNow { get; }

	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Data {
	using Microsoft.Extensions.Logging;

	public class DateRepository : IDateRepository{
		private List<Date> _dates;
		private readonly QueryStatsContext _queryStatsContext;
		private readonly ILogger _logger;

		public DateRepository(QueryStatsContext queryStatsContext, ILogger<DateRepository> logger) {
			_queryStatsContext = queryStatsContext;
			_logger = logger;
		}

		public int GetDayId(DateTime dateTime) {
			if (_dates == null) {
				_dates = _queryStatsContext.Dates.AsNoTracking().ToList();
			}
			DateTime currentDate = dateTime.Date;
			Date foundDate = _dates.FirstOrDefault(d => d.DateValue == currentDate);
			if (foundDate != null) {
				return foundDate.Id;
			}
			foundDate = new Date {
				DateValue = currentDate
			};
			_dates.Add(foundDate);
			_queryStatsContext.Dates.Add(foundDate);
			_queryStatsContext.SaveChanges();
			_logger.LogInformation("Date {0} added", currentDate);
			return foundDate.Id;
		}

		public void Refresh() {
			_dates = null;
		}

	}
}

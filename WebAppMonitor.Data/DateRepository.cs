using System;
using System.Collections.Generic;
using System.Linq;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Data {
	public class DateRepository : IDateRepository{
		private List<Date> _dates;
		private readonly QueryStatsContext _queryStatsContext;

		public DateRepository(QueryStatsContext queryStatsContext) {
			_queryStatsContext = queryStatsContext;
		}

		public int GetDayId(DateTime dateTime) {
			if (_dates == null) {
				_dates = _queryStatsContext.Dates.ToList();
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
			return foundDate.Id;
		}


	}
}

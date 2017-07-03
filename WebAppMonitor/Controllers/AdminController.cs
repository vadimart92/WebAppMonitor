using System;
using System.Linq;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAppMonitor.Core;

namespace WebAppMonitor.Controllers {
	public class ImportDailyDataRequest {
		public DataImportSettings ImportSettings { get; set; }
	}

	public class GetStatsInfoResult {
		public DateTime LastQueryInHistory { get; set; }
		public long TotalRecords { get; set; }

		public bool ImportJobActive { get; set; }
		public DataImportSettings ImportSettings { get; set; }
	}

	[Route("api/[controller]")]
	public class AdminController : Controller {
		private const string ImportDailyStatementDatajobId = "10";

		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IMemoryCache _memoryCache;

		private readonly IDataImporter _dataImporter;

		public AdminController(IDbConnectionProvider connectionProvider, IMemoryCache memoryCache,
			IDataImporter dataImporter) {
			_connectionProvider = connectionProvider;
			_memoryCache = memoryCache;
			_dataImporter = dataImporter;
		}

		[HttpGet("importExtendedEvents")]
		public IActionResult ImportExtendedEvents(string file) {
			_dataImporter.ImportExtendedEvents(file);
			return Ok();
		}

		[HttpGet("importAllByDates")]
		public IActionResult ImportAllByDates(string dates) {
			var parsedDates = dates.Split(',').Select(DateTime.Parse).ToList();
			if (parsedDates.Count > 0) {
				_dataImporter.ImportAllByDates(parsedDates);
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("importDailyData")]
		public IActionResult ImportDailyData([FromBody] ImportDailyDataRequest value) {
			if (value.ImportSettings != null) {
				_dataImporter.ChangeSettings(value.ImportSettings);
			}
			try {
				_dataImporter.ImportDailyData();
			}
			catch (Exception e) {
				return BadRequest(e.Message);
			}
			CaheUtils.ClearCache(_memoryCache);
			return Ok();
		}

		[HttpPost("toggleImportJob")]
		public IActionResult ToggleImportJob() {
			bool exists = GetIsJobExists();
			if (exists) {
				RecurringJob.RemoveIfExists(ImportDailyStatementDatajobId);
			}
			else {
				RecurringJob.AddOrUpdate<IDataImporter>(ImportDailyStatementDatajobId, d => d.ImportDailyData(), Cron.Daily(5, 30));
			}
			return Ok();
		}

		private static bool GetIsJobExists() {
			var jobs = JobStorage.Current.GetConnection()
				.GetAllEntriesFromHash($"recurring-job:{ImportDailyStatementDatajobId}");
			bool exists = jobs?.Any() ?? false;
			return exists;
		}

		[HttpGet("getStatsInfo")]
		public GetStatsInfoResult GetStatsInfo() {
			var result = new GetStatsInfoResult();
			_connectionProvider.GetConnection((connection) => {
				result.LastQueryInHistory =
					connection.ExecuteScalar<DateTime>("SELECT TOP 1 end_time_utc FROM QueryHistory ORDER BY end_time_utc DESC");
				result.TotalRecords = connection.ExecuteScalar<long>("SELECT Count(*) FROM QueryHistory");
			});
			result.ImportSettings = _dataImporter.GetSettings();
			result.ImportJobActive = GetIsJobExists();
			return result;
		}
	}
}

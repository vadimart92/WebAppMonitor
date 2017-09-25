using System;
using System.Linq;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Common;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Controllers {

	public class GetStatsInfoResult {
		public DateTime LastQueryInHistory { get; set; }
		public long TotalRecords { get; set; }
		public bool ImportJobActive { get; set; }
		public ISettings ImportSettings { get; set; }
	}

	[Route("api/[controller]")]
	public class AdminController : Controller {
		private const string ImportDailyStatementDatajobId = "10";

		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IMemoryCache _memoryCache;

		private readonly IDataLoader _dataLoader;

		public AdminController(IDbConnectionProvider connectionProvider, IMemoryCache memoryCache,
			IDataLoader dataLoader) {
			_connectionProvider = connectionProvider;
			_memoryCache = memoryCache;
			_dataLoader = dataLoader;
		}

		[HttpGet("importLongLocks")]
		public IActionResult ImportLongLocks(string file) {
			_dataLoader.ImportLongLocks(file);
			return Ok();
		}

		[HttpGet("importExtendedEvents")]
		public IActionResult ImportExtendedEvents(string file) {
			_dataLoader.ImportExtendedEvents();
			return Ok();
		}

		[HttpGet("importJsonLogs")]
		public IActionResult ImportJsonLogs(string file) {
			_dataLoader.ImportJsonLogs();
			return Ok();
		}

		[HttpGet("importDeadLocks")]
		public IActionResult ImportDeadLocks(string file) {
			_dataLoader.ImportDeadlocks(file);
			return Ok();
		}
		
		[HttpGet("importReaderLogs")]
		public IActionResult ImportReaderLogs(string file) {
			_dataLoader.ImportReaderLogs(file);
			return Ok();
		}
		[HttpGet("importDbExecutorLogs")]
		public IActionResult ImportDbExecutorLogs(string file) {
			_dataLoader.ImportDbExecutorLogs(file);
			return Ok();
		}

		[HttpGet("importPerformanceLogs")]
		public IActionResult ImportPerformanceLogs(string file) {
			_dataLoader.ImportPerfomanceLoggerLogs(file);
			return Ok();
		}

		[HttpGet("importAllByDates")]
		public IActionResult ImportAllByDates(string dates) {
			var parsedDates = dates.Split(',').Select(DateTime.Parse).ToList();
			if (parsedDates.Count > 0) {
				_dataLoader.ImportAllByDates(parsedDates);
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("saveSettings")]
		public IActionResult SaveSettings([FromBody]DataImportSettings settings) {
			_dataLoader.ChangeSettings(settings);
			return Ok();
		}

		[HttpPost("importDailyData")]
		public IActionResult ImportDailyData() {
			try {
				_dataLoader.ImportDailyData();
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
				RecurringJob.AddOrUpdate<IDataLoader>(ImportDailyStatementDatajobId, d => d.ImportDailyData(), Cron.Daily(5, 30));
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
			result.ImportSettings = _dataLoader.GetSettings();
			result.ImportJobActive = GetIsJobExists();
			return result;
		}
	}
}

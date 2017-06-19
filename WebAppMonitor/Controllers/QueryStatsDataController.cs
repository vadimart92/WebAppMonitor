using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using gudusoft.gsqlparser;
using gudusoft.gsqlparser.pp.para;
using gudusoft.gsqlparser.pp.stmtformatter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAppMonitor.Common;

namespace WebAppMonitor.Controllers
{

	public class GetQueriesByTotalTimeResponse
	{

		public GetQueriesByTotalTimeResponse() {
			Queries = new List<dynamic>();
		}
		public List<dynamic> Queries { get; set; }
		public int TotalQueries { get; set; }

	}

	public class QueryStatsInfo
	{

		public IEnumerable<DateTime> DatesWithData { get; set; }

	}
	public class DailyReportInfo
	{

		public IEnumerable<dynamic> Queries { get; set; }
	}
	public class FormatSqlRequest
	{

		public Guid QueryId { get; set; }
	}
	public class FormatSqlResponse
	{

		public string Result { get; set; }
	}

	[Route("api/[controller]")]
	public class QueryStatsDataController : Controller
	{
		private readonly IDbConnectionProvider _connectionProvider;
		private readonly IMemoryCache _memoryCache;
		
		public QueryStatsDataController(IDbConnectionProvider connectionProvider, IMemoryCache memoryCache) {
			_connectionProvider = connectionProvider;
			_memoryCache = memoryCache;
		}

		// GET api/requestStatsData/byTotalTime/date
		[HttpGet("byTotalTime/{date}")]
		public GetQueriesByTotalTimeResponse GetQueriesByTotalTime(string date, int rowCount = 100,
			int totalDuration = 300) {
			DateTime parsedDate = DateTime.Parse(date);
			string cacheKey =
				$"GetQueriesByTotalTimeResponse_{parsedDate:MM/dd/yyyy}_rows:{rowCount}_total:{totalDuration}";
			return _memoryCache.GetOrCreate(cacheKey, entry => {
				StoreKey(entry);
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
				var result = new GetQueriesByTotalTimeResponse();
				_connectionProvider.GetConnection((connection) => {
					var times = connection.Query("GetQueriesByTotalTime", new {
						date = parsedDate,
						totalDuration = totalDuration,
						rowsCount = rowCount
					}, commandType: CommandType.StoredProcedure);
					result.Queries.AddRange(times);
					result.TotalQueries =
						connection.QuerySingle<int>(
							"SELECT COUNT(Id) FROM QueryHistory WHERE DateId = (SELECT Id FROM Dates d WHERE d.Date = CAST(@date as DATE))", new { date = parsedDate.Date });
				});
				return result;
			});

		}
		public async Task<DailyReportInfo> DailyReport(int rowsCountPerDay = 10) {
			return await _memoryCache.GetOrCreateAsync($"DailyReportInfo_{rowsCountPerDay}", entry => {
				StoreKey(entry);
				var m = new DailyReportInfo();
				_connectionProvider.GetConnection((connection) => {
					m.Queries = connection.Query("GetQueriesByTotalTimeByDates", new {
						rowsCountPerDay = rowsCountPerDay
					}, commandType: CommandType.StoredProcedure);
				});
				return Task.FromResult(m);
			});

		}
		[HttpPost("formatSQl")]
		public FormatSqlResponse FormatSQl([FromBody]FormatSqlRequest request) {
			string formattedText = _memoryCache.GetOrCreate("SQL_" + request.QueryId, entry => {
				StoreKey(entry);
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
				string text = string.Empty;
				_connectionProvider.GetConnection(c => {
					text = c.QuerySingle<string>("SELECT NormalizedQuery FROM NormQueryTextHistory WHERE Id = @id", new {
						id = request.QueryId
					});
				});
				try {
					var parser = new TGSqlParser(EDbVendor.dbvmssql) {
						sqltext = text
					};
					parser.parse();
					GFmtOpt option = GFmtOptFactory.newInstance();
					option.outputFmt = GOutputFmt.ofSql;
					string result = FormatterFactory.pp(parser, option);
					return result;
				} catch (Exception) {
					return "Error while formatting sql" + text;
				}
			});
			return new FormatSqlResponse {
				Result = formattedText
			};
		}

		[HttpGet("info")]
		public async Task<QueryStatsInfo> GetQueryStatsInfo() {
			return await _memoryCache.GetOrCreateAsync("datesWithData", entry => {
				StoreKey(entry);
				var result = new QueryStatsInfo();
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
				_connectionProvider.GetConnection((c) => {
					result.DatesWithData = c.Query<DateTime>("SELECT Date FROM Dates");
				});
				return Task.FromResult(result);
			}).ConfigureAwait(false);
		}

		[HttpPost("clearCache")]
		public void ClearCache() {
			var keys = _memoryCache.Get("Keys") as IEnumerable<string>;
			if (keys != null)
				foreach (string key in keys) {
					_memoryCache.Remove(key);
				}
		}

		private void StoreKey(ICacheEntry entry) {
			_memoryCache.GetOrCreate("Keys", e => new List<string>()).Add(entry.Key.ToString());
		}
	}
}

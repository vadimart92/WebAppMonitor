using System;
using System.Data;
using System.IO;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAppMonitor.Common;

namespace WebAppMonitor.Controllers
{

	public class ImportDailyDataRequest
	{

		public string FileName { get; set; }

	}
	public class GetStatsInfoResult
	{

		public DateTime LastQueryInHistory { get; set; }
		public long TotalRecords { get; set; }

	}



	[Route("api/[controller]")]
    public class AdminController : Controller
    {
	    private readonly IDbConnectionProvider _connectionProvider;
	    private readonly IMemoryCache _memoryCache;

	    public AdminController(IDbConnectionProvider connectionProvider, IMemoryCache memoryCache) {
		    _connectionProvider = connectionProvider;
		    _memoryCache = memoryCache;
	    }

	    [HttpPost("importDailyData")]
        public IActionResult ImportDailyData([FromBody]ImportDailyDataRequest value)
        {
	        if (!Directory.Exists(Path.GetDirectoryName(value.FileName))) {
		        return BadRequest($"directory {value.FileName} not found.");
	        }
			_connectionProvider.GetConnection((connection) => {
				connection.Execute("ImportDailyData", new { fileName=value.FileName }, commandType: CommandType.StoredProcedure, commandTimeout:3600);
				connection.Execute("SaveDailyData", commandType: CommandType.StoredProcedure, commandTimeout: 3600);
			});
	        CaheUtils.ClearCache(_memoryCache);
	        return Ok();
		}

	    [HttpGet("getStatsInfo")]
        public GetStatsInfoResult ImportDailyData() {
		    var result = new GetStatsInfoResult();
			_connectionProvider.GetConnection((connection) => {
				result.LastQueryInHistory =  connection.ExecuteScalar<DateTime>("SELECT TOP 1 end_time_utc FROM QueryHistory ORDER BY end_time_utc DESC");
				result.TotalRecords =  connection.ExecuteScalar<long>("SELECT Count(*) FROM QueryHistory");
			});
	        return result;
		}

    }
}

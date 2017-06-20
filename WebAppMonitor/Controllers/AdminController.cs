﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

    }
}

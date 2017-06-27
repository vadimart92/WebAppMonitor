﻿using System.Linq;
using Breeze.AspNetCore;
using Breeze.Persistence.EF6;
using Microsoft.AspNetCore.Mvc;
using WebAppMonitor.Core.Entities;
using WebAppMonitor.Data;

namespace WebAppMonitor.Controllers
{
	[Route("breeze/[controller]/[action]")]
	[BreezeQueryFilter]
	public class DataServiceController : Controller
	{
		private readonly EFPersistenceManager<QueryStatsContext> _efPersistenceManager;
		public DataServiceController(QueryStatsContext context)
		{
			_efPersistenceManager = new EFPersistenceManager<QueryStatsContext>(context);
		}

		[HttpGet]
		public IQueryable<QueryStatInfo> QueryStatInfo()
		{
			return _efPersistenceManager.Context.QueryStatInfo;
		}

		[HttpGet]
		public string Metadata()
		{
			return _efPersistenceManager.Metadata();
		}
	}
}

﻿using System;
using System.Collections.Generic;

namespace WebAppMonitor.Core.Import
{
	public interface IDataFilePathProvider
	{
		DateTime GetDate();
	    IEnumerable<string> GetExecutorLogs();
	    IEnumerable<string> GetReaderLogs();
	    IEnumerable<string> GetPerfomanceLogs();
	    IEnumerable<string> GetDailyExtEventsDirs();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAppMonitor.Core;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.Data
{
	public class AppLogLoader: IAppLogLoader
	{

		private readonly IJsonLogParser _jsonLogParser;

		public AppLogLoader(IJsonLogParser jsonLogParser) {
			_jsonLogParser = jsonLogParser;
		}

		public void LoadReaderLogs(string file) {
			
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMonitor.Core.Tests
{
	using System.Runtime.CompilerServices;

	public class AutoSettings:ISettings
    {
		private string Get([CallerMemberName] string code = null) {
			return SettingItemAttribute.GetDefValue(code);
		}

	    public virtual string DeadLocksFileTemplate {
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string LongLocksFileTemplate{
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string StatementsFileTemplate {
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string EventsDataDirectoryTemplate {
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string DatabaseName{
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string DirectoriesWithJsonLog {
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string DailyLogsDirectoryTemplate{
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string ReaderLogFileName {
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string ExecutorLogFileName {
		    get => Get();
		    set => throw new NotImplementedException();
	    }

		public virtual string PerfomanceLogFileName {
		    get => Get();
		    set => throw new NotImplementedException();
	    }
		public virtual string SharedDirectoryPath {
		    get => Get();
		    set => throw new NotImplementedException();
	    }

    }
}

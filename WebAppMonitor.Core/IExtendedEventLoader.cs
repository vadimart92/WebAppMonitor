using System;
using System.Collections.Generic;
using System.Text;

namespace WebAppMonitor.Core
{
    public interface IExtendedEventLoader {
	    void LoadLongLocksData(string file);
    }
}

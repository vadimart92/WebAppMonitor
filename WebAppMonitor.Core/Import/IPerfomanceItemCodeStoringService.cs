using System;

namespace WebAppMonitor.Core.Import
{
    public interface IPerfomanceItemCodeStoringService: ISynchronizedWorker
    {

	    Guid AddCode(string code);

    }
}

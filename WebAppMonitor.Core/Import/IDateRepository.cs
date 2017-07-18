using System;

namespace WebAppMonitor.Core.Import
{
    public interface IDateRepository
    {

	    int GetDayId(DateTime dateTime);

	    void Refresh();

    }
}

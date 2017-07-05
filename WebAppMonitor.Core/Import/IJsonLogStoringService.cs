using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.Core.Import
{
    public interface IJsonLogStoringService
    {
	    ITransaction BeginWork();
	    void RegisterReaderLogItem(ReaderLogRecord logRecord);
    }
}

using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.Core.Import
{
    public interface IJsonLogSaver
    {
	    ITransaction BeginWork();
	    void RegisterReaderLogItem(ReaderLogRecord logRecord);
    }
}

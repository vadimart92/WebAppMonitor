using WebAppMonitor.Core.Import.Entity;

namespace WebAppMonitor.Core.Import
{
    public interface IJsonLogStoringService
    {
	    ITransaction BeginWork();
	    void RegisterReaderLogItem(ReaderLogRecord logRecord);
	    void RegisterExecutorLogRecord(ExecutorLogRecord logRecord);
	    void RegisterPerfomanceLogItem(PerformanceLogRecord logRecord);

    }
}

namespace WebAppMonitor.Core.Import
{
    public interface IExtendedEventLoader {
	    void LoadLongLocksData(string file);
	    void LoadDeadLocksData(string file);
    }
}

namespace WebAppMonitor.Core
{
    public interface IExtendedEventLoader {
	    void LoadLongLocksData(string file);
	    void LoadDeadLocksData(string file);
    }
}

namespace WebAppMonitor.Core.Import
{
	public interface ISynchronizedWorker
	{
		void BeginWork();
		void Flush();
	}
}
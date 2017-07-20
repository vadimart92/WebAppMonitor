namespace WebAppMonitor.Data {
	public interface IEntityWithHash<TKey> {
		TKey Id {get; set;}
		byte[] HashValue { get; set;}
	}
}
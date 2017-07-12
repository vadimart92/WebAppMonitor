using System.Collections.Generic;

namespace WebAppMonitor.Core.Import
{
	public interface IJsonLogWithHash {
		void SetSourceLogHash(byte[] hash);
	}

	public interface IJsonLogParser
	{

		IEnumerable<T> ReadFile<T>(string filePath) where T: IJsonLogWithHash;

	}
}

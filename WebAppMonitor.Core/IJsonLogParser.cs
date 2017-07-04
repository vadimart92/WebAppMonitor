using System.Collections.Generic;

namespace WebAppMonitor.Core
{
	public interface IJsonLogParser
	{

		IEnumerable<T> ReadFile<T>(string filePath);

	}
}

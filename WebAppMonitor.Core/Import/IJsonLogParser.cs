using System.Collections.Generic;

namespace WebAppMonitor.Core.Import
{
	public interface IJsonLogParser
	{

		IEnumerable<T> ReadFile<T>(string filePath);

	}
}

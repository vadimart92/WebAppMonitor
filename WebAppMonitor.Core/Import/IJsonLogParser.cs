using System.Collections.Generic;

namespace WebAppMonitor.Core.Import
{
	using System;

	public interface IJsonLogWithHash {
		void SetSourceLogHash(byte[] hash);
	}

	public class JsonItemFilterAttribute: Attribute{ }

	public interface IJsonLogParser
	{

		IEnumerable<T> ReadFile<T>(string filePath) where T: IJsonLogWithHash;

	}
}

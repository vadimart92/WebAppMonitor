using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.DataProcessing.Json
{
	public class JsonLogParser: IJsonLogParser
	{
		public IEnumerable<T> ReadFile<T>(string filePath) {
			using (var streamReader = new StreamReader(File.OpenRead(filePath)))
			using (var reader = new JsonTextReader(streamReader)) {
				reader.SupportMultipleContent = true;
				var serializer = new JsonSerializer();
				while (reader.Read()) {
					if (reader.TokenType == JsonToken.StartObject) {
						yield return serializer.Deserialize<T>(reader);
					}
				}
			}
		}

	}
}

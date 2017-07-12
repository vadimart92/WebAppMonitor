using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using WebAppMonitor.Core.Import;

namespace WebAppMonitor.DataProcessing.Json
{
	public class JsonLogParser: IJsonLogParser
	{
		public IEnumerable<T> ReadFile<T>(string filePath) where T: IJsonLogWithHash {
			var hashProvider = SHA512.Create();
			using (var reader = new StreamReader(File.OpenRead(filePath))) {
				while (reader.Peek() > -1) {
					var line = reader.ReadLine();
					if (line == null) {
						continue;
					}
					var hash = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(line));
					var result = JsonConvert.DeserializeObject<T>(line);
					result.SetSourceLogHash(hash);
					yield return result;
				}
			}
		}
	}
}

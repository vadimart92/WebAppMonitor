namespace WebAppMonitor.DataProcessing.Json
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Security.Cryptography;
	using System.Text;
	using Newtonsoft.Json;
	using WebAppMonitor.Core.Import;
	using System.Linq;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Newtonsoft.Json.Serialization;

	public class JsonLogParser: IJsonLogParser
	{

		private bool NeedContext<T>() {
			return typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic)
				.Any(mi => mi.GetCustomAttribute<OnDeserializingAttribute>() != null);
		}

		private Func<string, bool> GetFilter<T>() {
			MethodInfo method =  typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic)
				.FirstOrDefault(mi => mi.GetCustomAttribute<JsonItemFilterAttribute>() != null);
			if (method != null) {
				return (Func<string, bool>)Delegate.CreateDelegate(typeof(Func<string, bool>), method);
			}
			return null;
		}

		public IEnumerable<T> ReadFile<T>(string filePath) where T: IJsonLogWithHash {
			SHA512 hashProvider = SHA512.Create();
			var settings = new JsonSerializerSettings {
				MissingMemberHandling = MissingMemberHandling.Ignore
			};
			bool needContext = NeedContext<T>();
			var filter = GetFilter<T>();
			using (var reader = new StreamReader(File.OpenRead(filePath))) {
				while (reader.Peek() > -1) {
					string line = reader.ReadLine();
					if (string.IsNullOrWhiteSpace(line) || filter != null && !filter(line)) {
						continue;
					}
					int lineSizeInMegaBytes = Encoding.UTF8.GetByteCount(line) / 1000 / 1000;
					if(lineSizeInMegaBytes > 50) {
						throw new Exception($"line is too big: {lineSizeInMegaBytes}MB");
					}
					T result =  default(T);
					try {
						result = JsonConvert.DeserializeObject<T>(line, settings);
					} catch (Exception) {
						if(needContext) {
							settings.Context = new StreamingContext(StreamingContextStates.File, line);
							result = JsonConvert.DeserializeObject<T>(line, settings);
						} else {
							throw;
						}
					}
					if (result != null) {
						var hash = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(line));
						result.SetSourceLogHash(hash);
						yield return result;
					}
				}
			}
		}
	}
}

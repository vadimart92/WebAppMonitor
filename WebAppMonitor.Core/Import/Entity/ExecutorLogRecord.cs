using System;

namespace WebAppMonitor.Core.Import.Entity {
	using System.IO;
	using System.Runtime.Serialization;
	using System.Text.RegularExpressions;
	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;
	using WebAppMonitor.Core.Common;

	public class ExecutorLogRecord:IJsonLogWithHash
	{
		public DateTime Date { get; set; }
		public string Level { get; set; }
		public string Appname { get; set; }
		public string Logger { get; set; }
		public string Thread { get; set; }
		public string Ndc { get; set; }
		public string Exception { get; set; }
		public Messageobject MessageObject { get; set; }

		public class Messageobject {
			public int Duration { get; set; }
			public string Sql { get; set; }
			[JsonIgnore]
			public Parameter[] Parameters { get; set; }
			public string StackTrace { get; set; }
		}

		public class Parameter {
			public object Value { get; set; }
			public string Type { get; set; }
			public string Name { get; set; }
		}

		private byte[] _sourceLogHash;
		public void SetSourceLogHash(byte[] hash) {
			_sourceLogHash = hash;
		}

		public byte[] GetSourceLogHash() {
			return _sourceLogHash;
		}

		static readonly Regex _validLineRegex = new Regex("\"messageObject\"(\\s*):(\\s*){");

		[JsonItemFilter]
		public static bool CanRead(string value) {
			return _validLineRegex.IsMatch(value);
		}

	}

	

}

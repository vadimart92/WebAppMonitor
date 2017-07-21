using System;
using Newtonsoft.Json;

namespace WebAppMonitor.Core.Import.Entity {

	public class PerformanceLogRecord : IJsonLogWithHash {
		[JsonProperty("d")]
		public DateTime Date { get; set; }
		[JsonProperty("t")]
		public string Thread { get; set; }
		[JsonProperty("u")]
		public string UserName { get; set; }
		[JsonProperty("m")]
		public Messageobject MessageObject { get; set; }

		private byte[] _bytes;
		public void SetSourceLogHash(byte[] hash) {
			_bytes = hash;
		}

		public byte[] GetSourceLogHash() {
			return _bytes;
		}

		public class Messageobject {
			public Guid Id { get; set; }
			public Guid ParentId { get; set; }
			public long Duration { get; set; }
			public string Code { get; set; }
		}
	}
}

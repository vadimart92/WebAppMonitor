using System;

namespace WebAppMonitor.Core.Import.Entity {
	public class PerfomanceLogRecord : IJsonLogWithHash {
		public DateTime Date { get; set; }
		public string Level { get; set; }
		public string Appname { get; set; }
		public string Logger { get; set; }
		public string Thread { get; set; }
		public string Ndc { get; set; }
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

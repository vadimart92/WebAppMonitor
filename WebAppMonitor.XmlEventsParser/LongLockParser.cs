using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace WebAppMonitor.XmlEventsParser {
	public class LongLockParser {
		private readonly XmlSerializer _serializer;

		public LongLockParser() {
			_serializer = new XmlSerializer(typeof(@event));
		}

		public @event Parse(string xml) {
			using (var tr = new XmlTextReader(new StringReader(xml))) {
				return (@event) _serializer.Deserialize(tr);
			}
		}
	}
}
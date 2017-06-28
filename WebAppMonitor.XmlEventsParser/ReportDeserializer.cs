using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WebAppMonitor.XmlEventsParser {
	public class ReportDeserializer {
		public static @event Parse(string xml) {
			var xmlSerializer = new XmlSerializer(typeof(@event));
			var bytes = Encoding.UTF8.GetBytes(xml);
			using (var tr = new XmlTextReader(new MemoryStream(bytes))) {
				return (@event) xmlSerializer.Deserialize(tr);
			}
		}
	}
}
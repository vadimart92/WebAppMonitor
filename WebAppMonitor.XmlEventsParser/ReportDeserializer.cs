using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WebAppMonitor.XmlEventsParser {
	public class ReportDeserializer {
		public static @event Parse(string xml) {
			var s = new XmlSerializer(typeof(@event));
			var tr = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
			var data = (@event) s.Deserialize(tr);
			foreach (var item in data.data) {
				
			}
			return null;
		}
	}
}
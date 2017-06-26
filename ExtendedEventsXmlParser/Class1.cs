using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ExtendedEventsXmlParser
{
    public class ReportDeserializer
    {

	    public static void Parse(string xml) {
		    var s = new XmlSerializer(typeof(BlockedProcesses));
			var tr = new XmlTextReader(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
		    var data = (BlockedProcesses)s.Deserialize(tr);
		    foreach (var item in data.Items) {
			    if (item is BlockedProcesses) {
				    
			    }
		    }
	    }
    }
}

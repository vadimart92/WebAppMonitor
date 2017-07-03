using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace WebAppMonitor.XmlEventsParser
{
    public class DeadLockParser
    {
	    private readonly XmlSerializer _serializer;

	    public DeadLockParser() {
		    _serializer = new XmlSerializer(typeof(Deadlocks.@event));
	    }

	    public Deadlocks.@event Parse(string xml) {
		    using (var tr = new XmlTextReader(new StringReader(xml))) {
			    return (Deadlocks.@event)_serializer.Deserialize(tr);
		    }
	    }

    }
}

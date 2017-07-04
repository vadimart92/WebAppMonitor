#pragma warning disable IDE1006 // Стили именования
using System.Xml;

namespace WebAppMonitor.XmlEventsParser.Deadlocks
{

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]

	public class @event

	{

		private eventData dataField;

		private eventAction[] actionField;

		private string nameField;

		private string packageField;

		private System.DateTime timestampField;

		/// <remarks/>
		public eventData data {
			get {
				return dataField;
			}
			set {
				dataField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("action")]
		public eventAction[] action {
			get {
				return actionField;
			}
			set {
				actionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string name {
			get {
				return nameField;
			}
			set {
				nameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string package {
			get {
				return packageField;
			}
			set {
				packageField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public System.DateTime timestamp {
			get {
				return timestampField;
			}
			set {
				timestampField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventData
	{

		private eventDataValue valueField;

		private string nameField;

		/// <remarks/>
		public eventDataValue value {
			get {
				return valueField;
			}
			set {
				valueField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string name {
			get {
				return nameField;
			}
			set {
				nameField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValue
	{

		private eventDataValueDeadlock deadlockField;

		/// <remarks/>
		public eventDataValueDeadlock deadlock {
			get {
				return deadlockField;
			}
			set {
				deadlockField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlock
	{

		private eventDataValueDeadlockVictimlist victimlistField;

		private eventDataValueDeadlockProcess[] processlistField;

		private XmlNode resourcelistField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElement("victim-list")]
		public eventDataValueDeadlockVictimlist victimlist {
			get {
				return victimlistField;
			}
			set {
				victimlistField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArray("process-list")]
		[System.Xml.Serialization.XmlArrayItem("process", IsNullable = false)]
		public eventDataValueDeadlockProcess[] processlist {
			get {
				return processlistField;
			}
			set {
				processlistField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAnyElement("resource-list")]
		public XmlNode resourcelist {
			get {
				return resourcelistField;
			}
			set {
				resourcelistField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlockVictimlist
	{

		private eventDataValueDeadlockVictimlistVictimProcess victimProcessField;

		/// <remarks/>
		public eventDataValueDeadlockVictimlistVictimProcess victimProcess {
			get {
				return victimProcessField;
			}
			set {
				victimProcessField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlockVictimlistVictimProcess
	{

		private string idField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string id {
			get {
				return idField;
			}
			set {
				idField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlockProcess
	{

		private eventDataValueDeadlockProcessFrame[] executionStackField;

		private string inputbufField;

		private string idField;

		private long taskpriorityField;

		private long logusedField;

		private string waitresourceField;

		private long waittimeField;

		private long ownerIdField;

		private string transactionnameField;

		private System.DateTime lasttranstartedField;

		private string xDESField;

		private string lockModeField;

		private long scheduleridField;

		private long kpidField;

		private string statusField;

		private long spidField;

		private long sbidField;

		private long ecidField;

		private long priorityField;

		private long trancountField;

		private System.DateTime lastbatchstartedField;

		private System.DateTime lastbatchcompletedField;

		private System.DateTime lastattentionField;

		private string clientappField;

		private string hostnameField;

		private long hostpidField;

		private string loginnameField;

		private string isolationlevelField;

		private long xactidField;

		private long currentdbField;

		private long lockTimeoutField;

		private long clientoption1Field;

		private long clientoption2Field;

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayItem("frame", IsNullable = false)]
		public eventDataValueDeadlockProcessFrame[] executionStack {
			get {
				return executionStackField;
			}
			set {
				executionStackField = value;
			}
		}

		/// <remarks/>
		public string inputbuf {
			get {
				return inputbufField;
			}
			set {
				inputbufField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string id {
			get {
				return idField;
			}
			set {
				idField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long taskpriority {
			get {
				return taskpriorityField;
			}
			set {
				taskpriorityField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long logused {
			get {
				return logusedField;
			}
			set {
				logusedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string waitresource {
			get {
				return waitresourceField;
			}
			set {
				waitresourceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long waittime {
			get {
				return waittimeField;
			}
			set {
				waittimeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long ownerId {
			get {
				return ownerIdField;
			}
			set {
				ownerIdField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string transactionname {
			get {
				return transactionnameField;
			}
			set {
				transactionnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public System.DateTime lasttranstarted {
			get {
				return lasttranstartedField;
			}
			set {
				lasttranstartedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string XDES {
			get {
				return xDESField;
			}
			set {
				xDESField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string lockMode {
			get {
				return lockModeField;
			}
			set {
				lockModeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long schedulerid {
			get {
				return scheduleridField;
			}
			set {
				scheduleridField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long kpid {
			get {
				return kpidField;
			}
			set {
				kpidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string status {
			get {
				return statusField;
			}
			set {
				statusField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long spid {
			get {
				return spidField;
			}
			set {
				spidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long sbid {
			get {
				return sbidField;
			}
			set {
				sbidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long ecid {
			get {
				return ecidField;
			}
			set {
				ecidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long priority {
			get {
				return priorityField;
			}
			set {
				priorityField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long trancount {
			get {
				return trancountField;
			}
			set {
				trancountField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public System.DateTime lastbatchstarted {
			get {
				return lastbatchstartedField;
			}
			set {
				lastbatchstartedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public System.DateTime lastbatchcompleted {
			get {
				return lastbatchcompletedField;
			}
			set {
				lastbatchcompletedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public System.DateTime lastattention {
			get {
				return lastattentionField;
			}
			set {
				lastattentionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string clientapp {
			get {
				return clientappField;
			}
			set {
				clientappField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string hostname {
			get {
				return hostnameField;
			}
			set {
				hostnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long hostpid {
			get {
				return hostpidField;
			}
			set {
				hostpidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string loginname {
			get {
				return loginnameField;
			}
			set {
				loginnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string isolationlevel {
			get {
				return isolationlevelField;
			}
			set {
				isolationlevelField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long xactid {
			get {
				return xactidField;
			}
			set {
				xactidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long currentdb {
			get {
				return currentdbField;
			}
			set {
				currentdbField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long lockTimeout {
			get {
				return lockTimeoutField;
			}
			set {
				lockTimeoutField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long clientoption1 {
			get {
				return clientoption1Field;
			}
			set {
				clientoption1Field = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long clientoption2 {
			get {
				return clientoption2Field;
			}
			set {
				clientoption2Field = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlockProcessFrame
	{

		private string procnameField;

		private long lineField;

		private long stmtstartField;

		private bool stmtstartFieldSpecified;

		private long stmtendField;

		private bool stmtendFieldSpecified;

		private string sqlhandleField;

		private string valueField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string procname {
			get {
				return procnameField;
			}
			set {
				procnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long line {
			get {
				return lineField;
			}
			set {
				lineField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long stmtstart {
			get {
				return stmtstartField;
			}
			set {
				stmtstartField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute]
		public bool stmtstartSpecified {
			get {
				return stmtstartFieldSpecified;
			}
			set {
				stmtstartFieldSpecified = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public long stmtend {
			get {
				return stmtendField;
			}
			set {
				stmtendField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute]
		public bool stmtendSpecified {
			get {
				return stmtendFieldSpecified;
			}
			set {
				stmtendFieldSpecified = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string sqlhandle {
			get {
				return sqlhandleField;
			}
			set {
				sqlhandleField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute]
		public string Value {
			get {
				return valueField;
			}
			set {
				valueField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class lockResource
	{
		
		private string objectnameField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string objectname {
			get {
				return objectnameField;
			}
			set {
				objectnameField = value;
			}
		}

	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlockPagelockOwnerlist
	{

		private eventDataValueDeadlockPagelockOwnerlistOwner ownerField;

		/// <remarks/>
		public eventDataValueDeadlockPagelockOwnerlistOwner owner {
			get {
				return ownerField;
			}
			set {
				ownerField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlockPagelockOwnerlistOwner
	{

		private string idField;

		private string modeField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string id {
			get {
				return idField;
			}
			set {
				idField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string mode {
			get {
				return modeField;
			}
			set {
				modeField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlockPagelockWaiterlist
	{

		private eventDataValueDeadlockPagelockWaiterlistWaiter waiterField;

		/// <remarks/>
		public eventDataValueDeadlockPagelockWaiterlistWaiter waiter {
			get {
				return waiterField;
			}
			set {
				waiterField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventDataValueDeadlockPagelockWaiterlistWaiter
	{

		private string idField;

		private string modeField;

		private string requestTypeField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string id {
			get {
				return idField;
			}
			set {
				idField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string mode {
			get {
				return modeField;
			}
			set {
				modeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string requestType {
			get {
				return requestTypeField;
			}
			set {
				requestTypeField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlType(AnonymousType = true)]
	public class eventAction
	{

		private object valueField;

		private string nameField;

		private string packageField;

		/// <remarks/>
		public object value {
			get {
				return valueField;
			}
			set {
				valueField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string name {
			get {
				return nameField;
			}
			set {
				nameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttribute]
		public string package {
			get {
				return packageField;
			}
			set {
				packageField = value;
			}
		}
	}


}
#pragma warning restore IDE1006 // Стили именования
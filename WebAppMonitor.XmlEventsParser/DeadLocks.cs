using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAppMonitor.XmlEventsParser.Deadlocks
{

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
	public partial class @event
	{

		private eventData dataField;

		private eventAction[] actionField;

		private string nameField;

		private string packageField;

		private System.DateTime timestampField;

		/// <remarks/>
		public eventData data {
			get {
				return this.dataField;
			}
			set {
				this.dataField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("action")]
		public eventAction[] action {
			get {
				return this.actionField;
			}
			set {
				this.actionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string name {
			get {
				return this.nameField;
			}
			set {
				this.nameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string package {
			get {
				return this.packageField;
			}
			set {
				this.packageField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.DateTime timestamp {
			get {
				return this.timestampField;
			}
			set {
				this.timestampField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventData
	{

		private eventDataValue valueField;

		private string nameField;

		/// <remarks/>
		public eventDataValue value {
			get {
				return this.valueField;
			}
			set {
				this.valueField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string name {
			get {
				return this.nameField;
			}
			set {
				this.nameField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValue
	{

		private eventDataValueDeadlock deadlockField;

		/// <remarks/>
		public eventDataValueDeadlock deadlock {
			get {
				return this.deadlockField;
			}
			set {
				this.deadlockField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlock
	{

		private eventDataValueDeadlockVictimlist victimlistField;

		private eventDataValueDeadlockProcess[] processlistField;

		private eventDataValueDeadlockPagelock[] resourcelistField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("victim-list")]
		public eventDataValueDeadlockVictimlist victimlist {
			get {
				return this.victimlistField;
			}
			set {
				this.victimlistField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayAttribute("process-list")]
		[System.Xml.Serialization.XmlArrayItemAttribute("process", IsNullable = false)]
		public eventDataValueDeadlockProcess[] processlist {
			get {
				return this.processlistField;
			}
			set {
				this.processlistField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlArrayAttribute("resource-list")]
		[System.Xml.Serialization.XmlArrayItemAttribute("pagelock", IsNullable = false)]
		public eventDataValueDeadlockPagelock[] resourcelist {
			get {
				return this.resourcelistField;
			}
			set {
				this.resourcelistField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockVictimlist
	{

		private eventDataValueDeadlockVictimlistVictimProcess victimProcessField;

		/// <remarks/>
		public eventDataValueDeadlockVictimlistVictimProcess victimProcess {
			get {
				return this.victimProcessField;
			}
			set {
				this.victimProcessField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockVictimlistVictimProcess
	{

		private string idField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string id {
			get {
				return this.idField;
			}
			set {
				this.idField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockProcess
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
		[System.Xml.Serialization.XmlArrayItemAttribute("frame", IsNullable = false)]
		public eventDataValueDeadlockProcessFrame[] executionStack {
			get {
				return this.executionStackField;
			}
			set {
				this.executionStackField = value;
			}
		}

		/// <remarks/>
		public string inputbuf {
			get {
				return this.inputbufField;
			}
			set {
				this.inputbufField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string id {
			get {
				return this.idField;
			}
			set {
				this.idField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long taskpriority {
			get {
				return this.taskpriorityField;
			}
			set {
				this.taskpriorityField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long logused {
			get {
				return this.logusedField;
			}
			set {
				this.logusedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string waitresource {
			get {
				return this.waitresourceField;
			}
			set {
				this.waitresourceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long waittime {
			get {
				return this.waittimeField;
			}
			set {
				this.waittimeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long ownerId {
			get {
				return this.ownerIdField;
			}
			set {
				this.ownerIdField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string transactionname {
			get {
				return this.transactionnameField;
			}
			set {
				this.transactionnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.DateTime lasttranstarted {
			get {
				return this.lasttranstartedField;
			}
			set {
				this.lasttranstartedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string XDES {
			get {
				return this.xDESField;
			}
			set {
				this.xDESField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string lockMode {
			get {
				return this.lockModeField;
			}
			set {
				this.lockModeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long schedulerid {
			get {
				return this.scheduleridField;
			}
			set {
				this.scheduleridField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long kpid {
			get {
				return this.kpidField;
			}
			set {
				this.kpidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string status {
			get {
				return this.statusField;
			}
			set {
				this.statusField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long spid {
			get {
				return this.spidField;
			}
			set {
				this.spidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long sbid {
			get {
				return this.sbidField;
			}
			set {
				this.sbidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long ecid {
			get {
				return this.ecidField;
			}
			set {
				this.ecidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long priority {
			get {
				return this.priorityField;
			}
			set {
				this.priorityField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long trancount {
			get {
				return this.trancountField;
			}
			set {
				this.trancountField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.DateTime lastbatchstarted {
			get {
				return this.lastbatchstartedField;
			}
			set {
				this.lastbatchstartedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.DateTime lastbatchcompleted {
			get {
				return this.lastbatchcompletedField;
			}
			set {
				this.lastbatchcompletedField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public System.DateTime lastattention {
			get {
				return this.lastattentionField;
			}
			set {
				this.lastattentionField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string clientapp {
			get {
				return this.clientappField;
			}
			set {
				this.clientappField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string hostname {
			get {
				return this.hostnameField;
			}
			set {
				this.hostnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long hostpid {
			get {
				return this.hostpidField;
			}
			set {
				this.hostpidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string loginname {
			get {
				return this.loginnameField;
			}
			set {
				this.loginnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string isolationlevel {
			get {
				return this.isolationlevelField;
			}
			set {
				this.isolationlevelField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long xactid {
			get {
				return this.xactidField;
			}
			set {
				this.xactidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long currentdb {
			get {
				return this.currentdbField;
			}
			set {
				this.currentdbField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long lockTimeout {
			get {
				return this.lockTimeoutField;
			}
			set {
				this.lockTimeoutField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long clientoption1 {
			get {
				return this.clientoption1Field;
			}
			set {
				this.clientoption1Field = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long clientoption2 {
			get {
				return this.clientoption2Field;
			}
			set {
				this.clientoption2Field = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockProcessFrame
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
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string procname {
			get {
				return this.procnameField;
			}
			set {
				this.procnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long line {
			get {
				return this.lineField;
			}
			set {
				this.lineField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long stmtstart {
			get {
				return this.stmtstartField;
			}
			set {
				this.stmtstartField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute()]
		public bool stmtstartSpecified {
			get {
				return this.stmtstartFieldSpecified;
			}
			set {
				this.stmtstartFieldSpecified = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long stmtend {
			get {
				return this.stmtendField;
			}
			set {
				this.stmtendField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlIgnoreAttribute()]
		public bool stmtendSpecified {
			get {
				return this.stmtendFieldSpecified;
			}
			set {
				this.stmtendFieldSpecified = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string sqlhandle {
			get {
				return this.sqlhandleField;
			}
			set {
				this.sqlhandleField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlTextAttribute()]
		public string Value {
			get {
				return this.valueField;
			}
			set {
				this.valueField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockPagelock
	{

		private eventDataValueDeadlockPagelockOwnerlist ownerlistField;

		private eventDataValueDeadlockPagelockWaiterlist waiterlistField;

		private long fileidField;

		private long pageidField;

		private long dbidField;

		private string subresourceField;

		private string objectnameField;

		private string idField;

		private string modeField;

		private long associatedObjectIdField;

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("owner-list")]
		public eventDataValueDeadlockPagelockOwnerlist ownerlist {
			get {
				return this.ownerlistField;
			}
			set {
				this.ownerlistField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlElementAttribute("waiter-list")]
		public eventDataValueDeadlockPagelockWaiterlist waiterlist {
			get {
				return this.waiterlistField;
			}
			set {
				this.waiterlistField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long fileid {
			get {
				return this.fileidField;
			}
			set {
				this.fileidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long pageid {
			get {
				return this.pageidField;
			}
			set {
				this.pageidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long dbid {
			get {
				return this.dbidField;
			}
			set {
				this.dbidField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string subresource {
			get {
				return this.subresourceField;
			}
			set {
				this.subresourceField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string objectname {
			get {
				return this.objectnameField;
			}
			set {
				this.objectnameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string id {
			get {
				return this.idField;
			}
			set {
				this.idField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string mode {
			get {
				return this.modeField;
			}
			set {
				this.modeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public long associatedObjectId {
			get {
				return this.associatedObjectIdField;
			}
			set {
				this.associatedObjectIdField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockPagelockOwnerlist
	{

		private eventDataValueDeadlockPagelockOwnerlistOwner ownerField;

		/// <remarks/>
		public eventDataValueDeadlockPagelockOwnerlistOwner owner {
			get {
				return this.ownerField;
			}
			set {
				this.ownerField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockPagelockOwnerlistOwner
	{

		private string idField;

		private string modeField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string id {
			get {
				return this.idField;
			}
			set {
				this.idField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string mode {
			get {
				return this.modeField;
			}
			set {
				this.modeField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockPagelockWaiterlist
	{

		private eventDataValueDeadlockPagelockWaiterlistWaiter waiterField;

		/// <remarks/>
		public eventDataValueDeadlockPagelockWaiterlistWaiter waiter {
			get {
				return this.waiterField;
			}
			set {
				this.waiterField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventDataValueDeadlockPagelockWaiterlistWaiter
	{

		private string idField;

		private string modeField;

		private string requestTypeField;

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string id {
			get {
				return this.idField;
			}
			set {
				this.idField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string mode {
			get {
				return this.modeField;
			}
			set {
				this.modeField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string requestType {
			get {
				return this.requestTypeField;
			}
			set {
				this.requestTypeField = value;
			}
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
	public partial class eventAction
	{

		private object valueField;

		private string nameField;

		private string packageField;

		/// <remarks/>
		public object value {
			get {
				return this.valueField;
			}
			set {
				this.valueField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string name {
			get {
				return this.nameField;
			}
			set {
				this.nameField = value;
			}
		}

		/// <remarks/>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string package {
			get {
				return this.packageField;
			}
			set {
				this.packageField = value;
			}
		}
	}


}

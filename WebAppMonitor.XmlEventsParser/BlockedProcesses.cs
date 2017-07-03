﻿
/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class @event
{

	private eventData[] dataField;

	private string nameField;

	private string packageField;

	private System.DateTime timestampField;

	/// <remarks/>
	[System.Xml.Serialization.XmlElementAttribute("data")]
	public eventData[] data {
		get {
			return this.dataField;
		}
		set {
			this.dataField = value;
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

	private string textField;

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
	public string text {
		get {
			return this.textField;
		}
		set {
			this.textField = value;
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

	private eventDataValueBlockedprocessreport blockedprocessreportField;

	private string[] textField;

	/// <remarks/>
	[System.Xml.Serialization.XmlElementAttribute("blocked-process-report")]
	public eventDataValueBlockedprocessreport blockedprocessreport {
		get {
			return this.blockedprocessreportField;
		}
		set {
			this.blockedprocessreportField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlTextAttribute()]
	public string[] Text {
		get {
			return this.textField;
		}
		set {
			this.textField = value;
		}
	}
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class eventDataValueBlockedprocessreport
{

	private eventDataValueBlockedprocessreportBlockedprocess blockedprocessField;

	private eventDataValueBlockedprocessreportBlockingprocess blockingprocessField;

	private long monitorLoopField;

	/// <remarks/>
	[System.Xml.Serialization.XmlElementAttribute("blocked-process")]
	public eventDataValueBlockedprocessreportBlockedprocess blockedprocess {
		get {
			return this.blockedprocessField;
		}
		set {
			this.blockedprocessField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlElementAttribute("blocking-process")]
	public eventDataValueBlockedprocessreportBlockingprocess blockingprocess {
		get {
			return this.blockingprocessField;
		}
		set {
			this.blockingprocessField = value;
		}
	}

	/// <remarks/>
	[System.Xml.Serialization.XmlAttributeAttribute()]
	public long monitorLoop {
		get {
			return this.monitorLoopField;
		}
		set {
			this.monitorLoopField = value;
		}
	}
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class eventDataValueBlockedprocessreportBlockedprocess
{

	private eventDataValueBlockedprocessreportBlockedprocessProcess processField;

	/// <remarks/>
	public eventDataValueBlockedprocessreportBlockedprocessProcess process {
		get {
			return this.processField;
		}
		set {
			this.processField = value;
		}
	}
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class eventDataValueBlockedprocessreportBlockedprocessProcess
{

	private eventDataValueBlockedprocessreportBlockedprocessProcessFrame[] executionStackField;

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
	public eventDataValueBlockedprocessreportBlockedprocessProcessFrame[] executionStack {
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
public partial class eventDataValueBlockedprocessreportBlockedprocessProcessFrame
{

	private long lineField;

	private long stmtstartField;

	private bool stmtstartFieldSpecified;

	private long stmtendField;

	private bool stmtendFieldSpecified;

	private string sqlhandleField;

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
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class eventDataValueBlockedprocessreportBlockingprocess
{

	private BlockingProcess processField;

	/// <remarks/>
	public BlockingProcess process {
		get {
			return this.processField;
		}
		set {
			this.processField = value;
		}
	}
}

/// <remarks/>
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class BlockingProcess
{

	private object executionStackField;

	private string inputbufField;

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

	private string isolationlevelField;

	private long xactidField;

	private long currentdbField;

	private long lockTimeoutField;

	private long clientoption1Field;

	private long clientoption2Field;

	/// <remarks/>
	public object executionStack {
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


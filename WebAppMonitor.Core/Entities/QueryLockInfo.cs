﻿using System;

namespace WebAppMonitor.Core.Entities
{
	public class Proess
	{
		public string Text { get; set; }
	}

	public class BlockedProcess
	{

	}
	public class BlockerProcess
	{

	}

	public class QueryLockInfo
	{
		public string LockMode { get; set; }
		public DateTime TimeStamp { get; set; }
		public long Duration { get; set; }
		public Proess Blocker { get; set; }
		public Proess Blocked { get; set; }	
		public  string SourceXml { get; set; }
	}
}
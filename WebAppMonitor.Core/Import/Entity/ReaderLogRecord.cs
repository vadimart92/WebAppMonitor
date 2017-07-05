﻿using System;

namespace WebAppMonitor.Core.Import.Entity
{
	public class ReaderLogRecord:IJsonLogWithHash
	{
		public DateTime Date { get; set; }
		public string Level { get; set; }
		public string Appname { get; set; }
		public string Logger { get; set; }
		public string Thread { get; set; }
		public string Ndc { get; set; }
		public Messageobject MessageObject { get; set; }

		private byte[] _sourceLogHash;
		public void SetSourceLogHash(byte[] hash) {
			_sourceLogHash = hash;
		}

		public byte[] GetSourceLogHash() {
			return _sourceLogHash;
		}
	}

	public class Messageobject
	{
		public int[] RowsAffected { get; set; }
		public string Sql { get; set; }
		public Parameter[] Parameters { get; set; }
		public string StackTrace { get; set; }
	}

	public class Parameter
	{
		public string Value { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
	}

}

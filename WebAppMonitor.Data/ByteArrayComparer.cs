using System;
using System.Collections.Generic;
using System.Linq;

namespace WebAppMonitor.Data
{
	public class ByteArrayComparer : IEqualityComparer<byte[]>
	{
		public static IEqualityComparer<byte[]> Instance = new ByteArrayComparer();

		private ByteArrayComparer() {
			
		}
		public bool Equals(byte[] left, byte[] right)
		{
			if (left == null || right == null) {
				return left == right;
			}
			return left.SequenceEqual(right);
		}
		public int GetHashCode(byte[] key)
		{
			if (key == null) {
				return 0;
			}
			unchecked {
				int i = key.Length;
				int hc = i + 1;
				while (--i >= 0) {
					hc *= 31;
					hc ^= key[i];
				}
				return hc;
			}
		}
	}
}

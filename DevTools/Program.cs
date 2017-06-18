using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTools
{
	class Program
	{
		static void Main(string[] args) {
			if (args.Length > 2) {
				var cmd = args[0];
				if (cmd.Equals("start", StringComparison.OrdinalIgnoreCase)) {
					var exe = args[1];
					var arg = args[2];
					var psi = new ProcessStartInfo(exe) {
						UseShellExecute = true,
						Arguments = arg,
						WorkingDirectory = Environment.CurrentDirectory
					};
					Process.Start(psi);
					return;
				}
			}
		}
	}
}

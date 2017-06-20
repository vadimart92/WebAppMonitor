using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace DevTools
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 2)
            {
                var cmd = args[0];
                if (cmd.Equals("start", StringComparison.OrdinalIgnoreCase))
                {
                    var exe = args[1];
                    var arg = args[2];
                    if (Utils.FindApp(new []{arg,exe})) {
                        return;
                    }
                    var psi = new ProcessStartInfo(exe)
                    {
                        UseShellExecute = true,
                        Arguments = arg,
                        WorkingDirectory = Environment.CurrentDirectory
                    };
                    Process.Start(psi);
                }
            }
        }

        
       
    }

    public static class Utils
    {
        private static bool Contains(object source, string exe)
        {
            var s = source as string;
            if (s == null) { return false;}
            return s.IndexOf(Path.GetFileNameWithoutExtension(exe), StringComparison.OrdinalIgnoreCase) > -1;
        }

        public static bool FindApp(IEnumerable<string> processArgs)
        {
            var orFilter = processArgs.Aggregate($" {Process.GetCurrentProcess().Id} ", (s, s1) => $"{s} AND CommandLine LIKE '%{s1}%'");
            string queryString = $"SELECT CommandLine FROM Win32_Process WHERE ProcessId <> {orFilter}";
            using (var searcher = new ManagementObjectSearcher(ManagementPath.DefaultPath.Path, queryString, new EnumerationOptions(
                null, System.TimeSpan.MaxValue,
                1, true, false, true,
                true, false, true, true)))
            {
                var objects = searcher.Get().Cast<ManagementObject>().Select(o=> o["CommandLine"]).ToList();
                return objects.Any(o => processArgs.All(pa=> Contains(o,pa)));
            }
        }

    }
}

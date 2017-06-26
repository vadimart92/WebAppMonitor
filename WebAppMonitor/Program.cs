using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace WebAppMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
#if !DEBUG
				.UseUrls(@"http://tscore-dev-13:5000/")
#endif
				.UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace WebAppMonitor.Tests
{
	[TestFixture]
	public class ServerTests {
		private readonly TestServer _server;
		private readonly HttpClient _client;

		public ServerTests() {
			Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
			var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> {
				{"Environment", "Tests"}
			}).Build();
			_server = new TestServer(new WebHostBuilder().UseConfiguration(config).UseStartup<Startup>());
			_client = _server.CreateClient();
		}

		[Test]
		public async Task ImportExtendedEvents() {
			var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "collect_long_locks.xel");
			string requestUri = $"/api/Admin/importExtendedEvents?file={Uri.EscapeDataString(file)}";
			var response = await _client.GetAsync(requestUri);
			response.EnsureSuccessStatusCode();
		}

		[Test]
		public async Task PrimaryImport() {
			var files = Directory.EnumerateFiles(@"\\tscore-dev-13\WorkAnalisys\xevents", "collect_long_locks*.xel",
					SearchOption.AllDirectories)
				.Select(f=>new FileInfo(f))
				.OrderBy(f=>f.CreationTime)
				.ToList();
			foreach (FileInfo fileInfo in files) {
				var file = fileInfo.FullName;
				string requestUri = $"/api/Admin/importExtendedEvents?file={Uri.EscapeDataString(file)}";
				var response = await _client.GetAsync(requestUri);
				response.EnsureSuccessStatusCode();
			}
		}
	}
}
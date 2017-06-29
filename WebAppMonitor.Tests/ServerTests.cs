using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NSubstitute;
using NUnit.Framework;

namespace WebAppMonitor.Tests
{
	[TestFixture]
	public class ServerTests
	{
		private readonly TestServer _server;
		private readonly HttpClient _client;

		public ServerTests() {
			var s = new Startup(Substitute.For<IHostingEnvironment>());
			// Arrange
			_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
			//_client = _server.CreateClient();
		}

		[Test]
		public async Task ImportExtendedEvents()
		{
			var file = Path.Combine(TestContext.CurrentContext.TestDirectory, "collect_long_locks.xel");
			var response = await _client.GetAsync($"/api/Admin/ImportExtendedEvents?file={file}");
			response.EnsureSuccessStatusCode();
			var responseString = await response.Content.ReadAsStringAsync();
		}
	}
}
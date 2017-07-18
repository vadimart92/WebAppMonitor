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
			IConfigurationRoot config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string> {
				{"Environment", "Tests"}
			}).Build();
			_server = new TestServer(new WebHostBuilder().UseConfiguration(config).UseStartup<Startup>());
			_client = _server.CreateClient();
		}

		private string ShareFile(string file) {
			string sharedPath = @"\\tscore-dev-13\Share";
			if (!Directory.Exists(sharedPath)) {
				return file;
			}
			string tmpDir = Path.Combine(sharedPath, "tmp");
			if (Directory.Exists(tmpDir)) {
				Directory.Delete(tmpDir, true);
			}
			Directory.CreateDirectory(tmpDir);
			string fileName = Path.Combine(tmpDir, Path.GetFileName(file));
			File.Copy(file, fileName);
			return fileName;
		}

		[Test]
		public async Task ImportLongLocks() {
			string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "collect_long_locks.xel");
			file = ShareFile(file);
			string requestUri = $"/api/Admin/importLongLocks?file={Uri.EscapeDataString(file)}";
			HttpResponseMessage response = await _client.GetAsync(requestUri);
			response.EnsureSuccessStatusCode();
		}

		[Test]
		public async Task ImportDeadLocks() {
			string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "collect_deadlock_data.xel");
			file = ShareFile(file);
			string requestUri = $"/api/Admin/importDeadLocks?file={Uri.EscapeDataString(file)}";
			HttpResponseMessage response = await _client.GetAsync(requestUri);
			response.EnsureSuccessStatusCode();
		}
		[Test]
		public async Task ImportReaderLogs() {
			string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "LoggingDataReader.json.0.json");
			file = ShareFile(file);
			string requestUri = $"/api/Admin/importReaderLogs?file={Uri.EscapeDataString(file)}";
			HttpResponseMessage response = await _client.GetAsync(requestUri);
			response.EnsureSuccessStatusCode();
		}

		[Test]
		public async Task ImportDbExecutorLogs() {
			string file = Path.Combine(TestContext.CurrentContext.TestDirectory, "Sql.0.json");
			file = ShareFile(file);
			string requestUri = $"/api/Admin/importDbExecutorLogs?file={Uri.EscapeDataString(file)}";
			HttpResponseMessage response = await _client.GetAsync(requestUri);
			response.EnsureSuccessStatusCode();
		}

		[Test]
		public async Task PrimaryImport() {
			List<FileInfo> files = Directory.EnumerateFiles(@"\\tscore-dev-13\WorkAnalisys\xevents", "collect_long_locks*.xel",
					SearchOption.AllDirectories)
				.Select(f=>new FileInfo(f))
				.OrderBy(f=>f.CreationTime)
				.ToList();
			foreach (FileInfo fileInfo in files) {
				string file = fileInfo.FullName;
				string requestUri = $"/api/Admin/importExtendedEvents?file={Uri.EscapeDataString(file)}";
				HttpResponseMessage response = await _client.GetAsync(requestUri);
				response.EnsureSuccessStatusCode();
			}
		}

		[Test]
		public async Task DailyImport() {
			List<DateTime> dates = new List<DateTime> {
				//new DateTime(2017,7, 13),
				//new DateTime(2017,7, 14),
				//new DateTime(2017,7, 15),
				new DateTime(2017,7, 16),
				new DateTime(2017,7, 17),
				new DateTime(2017,7, 18)
			};
			string datesString = string.Join(",", dates.Select(d => d.ToString("yyyy-MM-dd")));
			string requestUri = $"/api/Admin/importAllByDates?dates={Uri.EscapeDataString(datesString)}";
			HttpResponseMessage response = await _client.GetAsync(requestUri);
			response.EnsureSuccessStatusCode();
		}
	}
}
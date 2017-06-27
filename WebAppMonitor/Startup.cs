using System.IO;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Common;
using WebAppMonitor.Data;

namespace WebAppMonitor
{
	public class Startup
	{
		public static IConfigurationRoot Configuration { get; set; }
		public Startup()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", reloadOnChange:true, optional:false);

			Configuration = builder.Build();
		}
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc()
				.AddJsonOptions(options =>
				{
					options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
				});

			string cs = Configuration.GetConnectionString("db");
			services.AddSingleton<IDbConnectionProvider>(new DbConnectionProviderImpl(cs));
			services.AddMemoryCache(options => options.CompactOnMemoryPressure = true);
			services.AddScoped(provider => new QueryStatsContext(cs));
			services.AddSingleton<IDataImporter, DataImporter>();
			services.AddHangfire(x => x.UseSqlServerStorage(cs));
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			loggerFactory.AddConsole();

			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.Use(async (context, next) => {
				await next();
				if (context.Response.StatusCode == 404 &&
				    !Path.HasExtension(context.Request.Path.Value) &&
				    !context.Request.Path.Value.StartsWith("/api/")) {
					context.Request.Path = "/index.html";
					await next();
				}
			});
			app.UseHangfireServer();
			app.UseHangfireDashboard(options: new DashboardOptions() {
				Authorization = new []{new HangfireAuthFilter()}
			});
			app.UseMvcWithDefaultRoute();
			app.UseDefaultFiles();
			app.UseStaticFiles();
		}
	}
}

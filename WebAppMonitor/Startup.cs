using System.IO;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using WebAppMonitor.Common;
using WebAppMonitor.Core;
using WebAppMonitor.Data;
using WebAppMonitor.XmlEventsParser;

namespace WebAppMonitor
{
	public class Startup
	{
		public static IConfigurationRoot Configuration { get; set; }
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddEnvironmentVariables()
				.AddJsonFile("appsettings.json", reloadOnChange:true, optional:false)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional:true);

			Configuration = builder.Build();
			env.ConfigureNLog("nlog.config");
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
			services.AddScoped<ISettingsRepository, SettingsRepository>();
			services.AddScoped<IExtendedEventParser, ExtendedEventParser>();
			services.AddScoped<IExtendedEventDataSaver, ExtendedEventDataSaver>();
			services.AddScoped<ISimpleDataProvider, SimpleDataProvider>();
			services.AddScoped<IExtendedEventLoader, ExtendedEventLoader>();
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			loggerFactory.AddConsole().AddNLog();
			app.AddNLogWeb();

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

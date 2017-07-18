using System;
using System.IO;
using AutoMapper;
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
using WebAppMonitor.Core.Import;
using WebAppMonitor.Data;
using WebAppMonitor.DataProcessing;
using WebAppMonitor.DataProcessing.Json;
using WebAppMonitor.XmlEventsParser;

namespace WebAppMonitor {
	using Autofac;
	using Autofac.Core;
	using Autofac.Core.Activators.Reflection;
	using Autofac.Extensions.DependencyInjection;

	public class Startup {
		public static IConfigurationRoot Configuration { get; set; }
		public IContainer ApplicationContainer { get; private set; }

		public Startup(IHostingEnvironment env) {
			IConfigurationBuilder builder = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
				.AddEnvironmentVariables("ASPNETCORE_")
				.AddJsonFile("appsettings.json", reloadOnChange: true, optional: false)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			Configuration = builder.Build();
			env.ConfigureNLog("nlog.config");
		}

		public IServiceProvider ConfigureServices(IServiceCollection services) {
			services.AddMvc().AddJsonOptions(options => {
				options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
			}).AddControllersAsServices();

			string connectionString = Configuration.GetConnectionString("db");
			services.AddAutoMapper(expression => { }, typeof(MappingProfile));
			services.AddMemoryCache(options => options.CompactOnMemoryPressure = true);
			services.AddScoped(provider => new QueryStatsContext(connectionString));
			services.AddHangfire(x => x.UseSqlServerStorage(connectionString));

			var builder = new ContainerBuilder();
			builder.Populate(services);

			var connectionProvider = new DbConnectionProviderImpl(connectionString);
			builder.RegisterInstance<IDbConnectionProvider>(connectionProvider).SingleInstance();

			RegisterTypes(builder);

			ApplicationContainer = builder.Build();
			return new AutofacServiceProvider(ApplicationContainer);
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			loggerFactory.AddNLog();
			app.AddNLogWeb();

			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.Use(async (context, next) => {
				await next();
				if (context.Response.StatusCode == 404 && !Path.HasExtension(context.Request.Path.Value) &&
				    !context.Request.Path.Value.StartsWith("/api/")) {
					context.Request.Path = "/index.html";
					await next();
				}
			});
			if(Configuration.GetValue("UseJobs", true)) {
				app.UseHangfireServer();
				app.UseHangfireDashboard(options: new DashboardOptions() {
					Authorization = new[] {new HangfireAuthFilter()}
				});
			}
			app.UseMvcWithDefaultRoute();
			app.UseDefaultFiles();
			app.UseStaticFiles();
		}


		private static void RegisterTypes(ContainerBuilder builder) {
			builder.RegisterType<DataLoader>().As<IDataLoader>().SingleInstance();
			builder.RegisterType<SettingsRepository>().As<ISettingsRepository>().SingleInstance();
			builder.RegisterType<Settings>().As<ISettings>().SingleInstance();
			builder.RegisterType<DateRepository>().As<IDateRepository>().SingleInstance();
			builder.RegisterType<QueryTextStoringService>().As<IQueryTextStoringService>().SingleInstance();
			builder.RegisterType<JsonLogStoringService>().As<IJsonLogStoringService>().SingleInstance();
			builder.RegisterType<StackStoringService>().As<IStackStoringService>().SingleInstance();
			builder.RegisterType<DataFilePathProvider>().As<IDataFilePathProvider>().SingleInstance();
			builder.RegisterType<CurrentDateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
			builder.RegisterType<PerfomanceItemCodeStoringService>().As<IPerfomanceItemCodeStoringService>()
				.SingleInstance();

			builder.RegisterType<ExtendedEventParser>().As<IExtendedEventParser>();
			builder.RegisterType<ExtendedEventDataSaver>().As<IExtendedEventDataSaver>();
			builder.RegisterType<SimpleDataProvider>().As<ISimpleDataProvider>();
			builder.RegisterType<ExtendedEventLoader>().As<IExtendedEventLoader>();
			builder.RegisterType<AppLogLoader>().As<IAppLogLoader>();
			builder.RegisterType<JsonLogParser>().Named<IJsonLogParser>("logParser").SingleInstance();
			builder.RegisterType<LoggingJsonLogParser>();
			builder.RegisterDecorator<IJsonLogParser>(
				(c, inner) => new LoggingJsonLogParser(c.Resolve<ILogger<LoggingJsonLogParser>>(), inner), "logParser");
		}

	}
}

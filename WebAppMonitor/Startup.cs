using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebAppMonitor.Common;

namespace WebAppMonitor
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc()
				.AddJsonOptions(options => {
					options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
				});
			
			services.AddSingleton<IDbConnectionProvider>(new DbConnectionProviderImpl(@"Data Source=tscore-dev-13\mssql2016; Initial Catalog=work_analisys; Persist Security Info=True; MultipleActiveResultSets=True; Integrated Security=SSPI; Pooling = true; Max Pool Size = 100; Async = true; Connection Timeout=500"));
			var memoryCache = new MemoryCache(new MemoryCacheOptions { CompactOnMemoryPressure = true });
			services.AddSingleton<IMemoryCache>(memoryCache);
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
			app.UseMvcWithDefaultRoute();
			app.UseDefaultFiles();
			app.UseStaticFiles();
		}
	}
}


using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using NLog;
using VixenModules.App.WebServer.Hubs;
using VixenModules.App.WebServer.Service;

namespace VixenModules.App.WebServer
{
	/// <summary>
	/// Startup config class for Owin server
	/// </summary>
	public class Startup
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private ShowHelperService _showHelperService = new ShowHelperService();

		public void ConfigureServices(IServiceCollection services)
		{
			//Maintain Pascal casing to match existing apis
			services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.PropertyNamingPolicy = null;
			});
			services.AddSignalR().AddJsonProtocol(options => {
				options.PayloadSerializerOptions.PropertyNamingPolicy = null;
			}); ;

			services.AddScoped<ContextBroadcaster>();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
		{

			appLifetime.ApplicationStarted.Register(() =>
			{
				Logging.Info("Webserver started.");
			});

			appLifetime.ApplicationStopping.Register(() =>
			{
				Logging.Info("Webaerver is stopping...");
			});

			appLifetime.ApplicationStopped.Register(() =>
			{
				Logging.Info("Webserver stopped.");
			});

			app.UseDefaultFiles();

			//	//Where our content lives
			string contentPath = Path.Combine(Environment.CurrentDirectory, @".\wwwroot");
			app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(contentPath),
				RequestPath = ""
			});

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<ContextHub>("hubs/contextStates");
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "DefaultApiWithAction",
					pattern: "api/{controller}/{action}/{id?}");
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "DefaultApi",
					pattern: "api/{controller}/{id?}");
			});

		}
	}

	
}

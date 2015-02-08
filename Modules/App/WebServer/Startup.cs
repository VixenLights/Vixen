using System;
using System.IO;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Owin;

[assembly: OwinStartup(typeof(VixenModules.App.WebServer.Startup))]

namespace VixenModules.App.WebServer
{
	/// <summary>
	/// Startup config class for Owin server
	/// </summary>
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			//logger for testing
			//app.Use(typeof (RequestLogger));

			app.MapSignalR();
			
			//Where our content lives
			string contentPath = Path.Combine(Environment.CurrentDirectory, @".\Modules\App\www");

			var physicalFileSystem = new PhysicalFileSystem(contentPath);
			var options = new FileServerOptions
			{
				EnableDefaultFiles = true,
				FileSystem = physicalFileSystem
			};
			options.StaticFileOptions.FileSystem = physicalFileSystem;
			options.StaticFileOptions.ServeUnknownFileTypes = true;
			options.DefaultFilesOptions.DefaultFileNames = new[] {"index.htm"}; //Default file to serve if none specified
			app.UseFileServer(options);

			var config = new HttpConfiguration();
			
			// JSON stuff
			
			//config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
			//config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
			

			//Route config
			config.MapHttpAttributeRoutes();
			config.Routes.MapHttpRoute(
				name: "DefaultApiWithAction",
				routeTemplate: "api/{controller}/{action}/{id}",
				defaults: new {id = RouteParameter.Optional}
				);

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new {id = RouteParameter.Optional}
				);

			app.UseWebApi(config);

			

		}

	}
}

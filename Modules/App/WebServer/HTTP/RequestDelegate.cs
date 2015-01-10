using System.IO;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Kayak;
using Kayak.Http;
using Newtonsoft.Json.Linq;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.App.WebServer.Actions;
using VixenModules.Effect.SetLevel;
using VixenModules.Property.Color;

namespace VixenModules.App.WebServer.HTTP
{
	class RequestDelegate : IHttpRequestDelegate
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public void OnRequest(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			if (request.Method.ToUpperInvariant() == "POST" && request.Uri.ToLower().StartsWith("/api")) 
			{
				ApiPost(request, requestBody, response);
			} else if (request.Uri.StartsWith("/api"))
			{
				ApiGet(request, response);
			} else if (request.Uri.StartsWith("/resx"))
			{
				GetResource(request, response);
			} else if (request.Uri.StartsWith("/"))
			{
				GetResourceByName("vixen.htm", response);
			} else
			{
				NotFoundResponse(request, response);
			}
		}

		
		private static HttpRequestHead EchoPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			var headers = new HttpResponseHead() {
				Status = "200 OK",
				Headers = new Dictionary<string, string>() 
						{
							{ "Content-Type", "text/plain" },
							{ "Connection", "close" }
						}
			};
			if (request.Headers.ContainsKey("Content-Length"))
				headers.Headers["Content-Length"] = request.Headers["Content-Length"];

			// if you call OnResponse before subscribing to the request body,
			// 100-continue will not be sent before the response is sent.
			// per rfc2616 this response must have a 'final' status code,
			// but the server does not enforce it.
			response.OnResponse(headers, requestBody);

			return request;
		}
		private static Assembly _assembly = null;

		private static void GetResource(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var contentRequest = request.Uri.ToString().ToLower().Replace("/resx/", string.Empty).Replace('/', '.');
			GetResourceByName(contentRequest, response);
		}

		private static void GetResourceByName(string contentRequest, IHttpResponseDelegate response)
		{

			var headers = new HttpResponseHead() {
				Status = "200 OK",
				Headers = new Dictionary<string, string>() 
						{
							{ "Content-Type", "text/plain" },
							{ "Connection", "close" }, 
							{ "Content-Length", "0" },
							{ "Cache-Control", "max-age=31536000"}
						}
			};

			BufferedProducer producer = new BufferedProducer("");

			if (_assembly == null)
				_assembly = Assembly.GetAssembly(typeof(VixenModules.App.WebServer.HTTP.RequestDelegate));


			try {
				var resources = _assembly.GetManifestResourceNames();
				var resourceItem = resources.FirstOrDefault(n => n.EndsWith(contentRequest, StringComparison.CurrentCultureIgnoreCase));

				if (resourceItem == null)
					throw new ApplicationException(string.Format("Requested Resource {0} does not exist.", contentRequest));

				using (var _Stream = _assembly.GetManifestResourceStream(resourceItem)) {
					var bytes = ReadFully(_Stream);

					headers.Headers["Content-Length"] = bytes.Length.ToString();
					headers.Headers["Content-Type"] = GetContentType(contentRequest);

					producer = new BufferedProducer(bytes);
				}

			} catch (Exception e) {
				Logging.Error(e.Message, e);
				headers.Status = "404 Not Found";
			}

			response.OnResponse(headers, producer);
		}

		private static string GetContentType(string fileName)
		{

			string contentType = "application/octetstream";

			var extension = Path.GetExtension(fileName);
			if (extension != null)
			{
				string ext = extension.ToLower();

				Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

				if (registryKey != null && registryKey.GetValue("Content Type") != null)
					contentType = registryKey.GetValue("Content Type").ToString();
			}

			return contentType;

		}

		public static byte[] ReadFully(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream()) {
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0) {
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}

		private static void ApiPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			const string pattern = @"^/api/([A-Za-z]+)/.*$";
			Match match = Regex.Match(request.Uri, pattern);
			if (match.Success)
			{
				IController controller = ControllerFactory.Get(match.Groups[1].Value);
				if (controller != null)
				{
					controller.ProcessPost(request, requestBody, response);
					return;
				}
			}

			NotFoundResponse(request, response);
			
		}

		private static void ApiGet(HttpRequestHead request, IHttpResponseDelegate response)
		{
			const string pattern = @"^/api/([A-Za-z]+)/.*$";
			Match match = Regex.Match(request.Uri, pattern);
			if (match.Success)
			{
				IController controller = ControllerFactory.Get(match.Groups[1].Value);
				if (controller != null)
				{
					controller.ProcessGet(request, response);
					return;
				}
			}

			NotFoundResponse(request, response);
			
		}

		private static void NotFoundResponse(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var responseBody = "The resource you requested ('" + request.Uri + "') could not be found.";
			var headers = new HttpResponseHead()
			{
				Status = "404 Not Found",
				Headers = new Dictionary<string, string>() 
					{
						{ "Content-Type", "text/html" },
						{ "Content-Length", responseBody.Length.ToString() },
					}
			};
			
			response.OnResponse(headers, new BufferedProducer(responseBody));
		}

	}
}

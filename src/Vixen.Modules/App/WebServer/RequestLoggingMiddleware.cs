using Microsoft.AspNetCore.Http;
using NLog;

namespace VixenModules.App.WebServer
{
	//Writes request info to the debug log for debugging purposes
	internal class RequestLoggingMiddleware
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private readonly RequestDelegate _next;

		public RequestLoggingMiddleware(RequestDelegate next)
		{
			_next = next ?? throw new ArgumentNullException(nameof(next));
		}

		public async Task Invoke(HttpContext context)
		{
			await _next(context); // Call the next middleware

			var address = context.Connection.RemoteIpAddress != null
				? context.Connection.RemoteIpAddress.ToString().Equals("::1")
					? "127.0.0.1"
					: context.Connection.RemoteIpAddress.ToString()
				: string.Empty;

			// Log Response Details
			Logging.Debug("{Ip} - \"{Method} {Path} {Protocol}\" {StatusCode} {Size}", address, 
				context.Request.Method, context.Request.Path, context.Request.Protocol, context.Response.StatusCode, context.Response.ContentLength);
		}

	}
}

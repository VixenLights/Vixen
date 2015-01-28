using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using NLog;

namespace VixenModules.App.WebServer.Filter
{
	public class ArgumentExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private static readonly NLog.Logger Logging = LogManager.GetCurrentClassLogger();
		public override void OnException(HttpActionExecutedContext context)
		{
			if (context.Exception is ArgumentException)
			{
				context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
				{
					ReasonPhrase = context.Exception.Message.Replace(Environment.NewLine, " ")
				};
			}

			if (context.Exception is ArgumentNullException)
			{
				context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
				{
					ReasonPhrase = context.Exception.Message.Replace(Environment.NewLine, " ")
				};
			}

			Logging.Error("Exception in web server", context.Exception);
			context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
			{
				ReasonPhrase = context.Exception.Message.Replace(Environment.NewLine, " ")
			};
		}		
	}
}

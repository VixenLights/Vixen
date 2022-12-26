using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NLog;

namespace VixenModules.App.WebServer.Filter
{
	public class ArgumentExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private static readonly NLog.Logger Logging = LogManager.GetCurrentClassLogger();

		public override void OnException(ExceptionContext context)
		{
			if (context.Exception is ArgumentException)
			{

				context.Result =
					new BadRequestObjectResult(context.Exception.Message.Replace(Environment.NewLine, " "));

			}
			else if (context.Exception is ArgumentNullException)
			{
				context.Result =
					new BadRequestObjectResult(context.Exception.Message.Replace(Environment.NewLine, " "));
			}
			else
			{
				Logging.Error(context.Exception, "Exception in web server");
				context.Result =
					new StatusCodeResult(StatusCodes.Status500InternalServerError);
			}
			
		}
	}		
}

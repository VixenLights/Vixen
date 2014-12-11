using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kayak;
using Kayak.Http;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Actions
{
	
	public class SchedulerController:BaseController
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public override void ProcessPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			if (request.Uri.StartsWith("/api/schedule"))
			{

				if (request.Uri.StartsWith("/api/schedule/status"))
				{
					return;
				}
			}

			UnsupportedOperation(request, response);
		}

		public override void ProcessGet(HttpRequestHead request, IHttpResponseDelegate response)
		{
			UnsupportedOperation(request, response);
		}

		private void Status(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var status = new Status {Message = "Not implemented."};


			SerializeResponse(status, response);
		}
	}
}

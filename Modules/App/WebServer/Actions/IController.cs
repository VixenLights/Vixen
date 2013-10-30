using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kayak;
using Kayak.Http;

namespace VixenModules.App.WebServer.Actions
{
	public interface IController
	{
		void ProcessPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response);

		void ProcessGet(HttpRequestHead request, IHttpResponseDelegate response);
	}
}

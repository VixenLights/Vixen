using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using Kayak;
using Kayak.Http;
using VixenModules.App.WebServer.HTTP;
using VixenModules.App.WebServer.Model;

namespace VixenModules.App.WebServer.Actions
{
	/// <summary>
	/// Base class for all action based controllers. Provides some basic functionality.
	/// </summary>
	public abstract class BaseController:IController
	{
		internal static HttpResponseHead GetOkHeaders(int contentSize)
		{
			var headers = new HttpResponseHead()
			{
				Status = "200 OK",
				Headers = new Dictionary<string, string>()
				{
					{"Content-Type", "application/json"},
					{"Content-Length", contentSize.ToString()},
				}
			};

			return headers;
		}

		internal static HttpResponseHead GetNotFoundHeaders(int contentSize)
		{
			var headers = new HttpResponseHead()
			{
				Status = "404 Not Found",
				Headers = new Dictionary<string, string>()
				{
					{"Content-Type", "application/json"},
					{"Content-Length", contentSize.ToString()},
				}
			};

			return headers;
		}

		internal static HttpResponseHead GetHeaders(int contentSize, string status)
		{
			var headers = new HttpResponseHead()
			{
				Status = status,
				Headers = new Dictionary<string, string>()
				{
					{"Content-Type", "application/json"},
					{"Content-Length", contentSize.ToString()},
				}
			};

			return headers;
		}

		internal static void SerializeResponse(Object o, IHttpResponseDelegate response)
		{
			var s = new JavaScriptSerializer();
			string json = s.Serialize(o);
			var headers = GetOkHeaders(json.Length);
			response.OnResponse(headers, new BufferedProducer(json));
		}

		internal static HttpRequestHead UnsupportedOperation(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var serializer = new JavaScriptSerializer();
			string json = serializer.Serialize(new Status
			{
				Message = "Unknown request"
			});
			HttpResponseHead headers = GetOkHeaders(json.Length);
			headers.Status = HttpStatusCode.BadRequest.ToString();
			response.OnResponse(headers, new BufferedProducer(json));
			return request;
		}

		internal static NameValueCollection GetParameters(HttpRequestHead request)
		{
			return HttpUtility.ParseQueryString(request.QueryString);
		}

		public abstract void ProcessPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response);
		public abstract void ProcessGet(HttpRequestHead request, IHttpResponseDelegate response);
	}
}

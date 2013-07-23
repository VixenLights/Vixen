using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kayak;
using Kayak.Http;

namespace VixenModules.App.WebServer.HTTP
{
	class RequestDelegate : IHttpRequestDelegate
	{
		public void OnRequest(HttpRequestHead request, IDataProducer requestBody,
			IHttpResponseDelegate response)
		{
			if (request.Method.ToUpperInvariant() == "POST" && request.Uri.StartsWith("/bufferedecho")) {
				request = BufferedEchoPost(request, requestBody, response);
			} else if (request.Method.ToUpperInvariant() == "POST" && request.Uri.StartsWith("/echo")) {
				request = EchoPost(request, requestBody, response);
			} else if (request.Uri.StartsWith("/Elements")) {
				request = ElementResponse(request, response);
			} else if (request.Uri.StartsWith("/")) {
				request = GenericResponse(request, response);
			} else {
				var responseBody = "The resource you requested ('" + request.Uri + "') could not be found.";
				var headers = new HttpResponseHead() {
					Status = "404 Not Found",
					Headers = new Dictionary<string, string>()
					{
						{ "Content-Type", "text/plain" },
						{ "Content-Length", responseBody.Length.ToString() }
					}
				};
				var body = new BufferedProducer(responseBody);

				response.OnResponse(headers, body);
			}
		}

		private static HttpRequestHead BufferedEchoPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			// when you subecribe to the request body before calling OnResponse,
			// the server will automatically send 100-continue if the client is 
			// expecting it.
			requestBody.Connect(new BufferedConsumer(bufferedBody => {
				var headers = new HttpResponseHead() {
					Status = "200 OK",
					Headers = new Dictionary<string, string>() 
								{
									{ "Content-Type", "text/plain" },
									{ "Content-Length", request.Headers["Content-Length"] },
									{ "Connection", "close" }
								}
				};
				response.OnResponse(headers, new BufferedProducer(bufferedBody));
			}, error => {
				// XXX
				// uh oh, what happens?
			}));
			return request;
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
		 
		private static HttpRequestHead ElementResponse(HttpRequestHead request, IHttpResponseDelegate response)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<html>");
			sb.AppendLine("<head>");
			sb.AppendLine("<script src=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.10.2/jquery.min.js\"></script>");
			sb.AppendLine("<script src=\"//ajax.googleapis.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js\"></script>");
			sb.AppendLine("<script src=\"//code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.js\"></script>");
			sb.AppendLine("<link rel=\"stylesheet\"  type=\"text/css\" media=\"all\" href=\"//code.jquery.com/mobile/1.3.1/jquery.mobile-1.3.1.min.css\" />");

			sb.AppendLine("</head>");
			sb.AppendLine("<body>");
			sb.AppendLine( string.Format(
							"Elements Hello world.<br/>Hello.<hr/><br/>Uri: {0}<br/>Path: {1}<br/>Query:{2}<br/>Fragment: {3}<br/>",
							request.Uri,
							request.Path,
							request.QueryString,
							request.Fragment));
			sb.AppendLine("</body>");
			sb.AppendLine("</html>");
	


			var headers = new HttpResponseHead() {
				Status = "200 OK",
				Headers = new Dictionary<string, string>() 
					{
						{ "Content-Type", "text/html" },
						{ "Content-Length", sb.Length.ToString() },
					}
			};
			
			response.OnResponse(headers, new BufferedProducer(sb.ToString()));
			return request;
		}
		private static HttpRequestHead GenericResponse(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var body = string.Format(
							"Hello world.\r\nHello.\r\n\r\nUri: {0}\r\nPath: {1}\r\nQuery:{2}\r\nFragment: {3}\r\n",
							request.Uri,
							request.Path,
							request.QueryString,
							request.Fragment);

			var headers = new HttpResponseHead() {
				Status = "200 OK",
				Headers = new Dictionary<string, string>() 
					{
						{ "Content-Type", "text/plain" },
						{ "Content-Length", body.Length.ToString() },
					}
			};
			response.OnResponse(headers, new BufferedProducer(body));
			return request;
		}
	}
}

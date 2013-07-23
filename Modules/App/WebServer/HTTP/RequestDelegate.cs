using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kayak;
using Kayak.Http;
using Newtonsoft.Json.Linq;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Services;
using Vixen.Sys;

namespace VixenModules.App.WebServer.HTTP {
	class RequestDelegate : IHttpRequestDelegate {
		public void OnRequest(HttpRequestHead request, IDataProducer requestBody,
			IHttpResponseDelegate response) {
			if (request.Method.ToUpperInvariant() == "POST" && request.Uri.StartsWith("/sequence", StringComparison.CurrentCultureIgnoreCase)) {
				request = SequencePost(request, requestBody, response);
			}
			else if (request.Method.ToUpperInvariant() == "POST" && request.Uri.StartsWith("/element")) {
				request = EchoPost(request, requestBody, response);
			}
			else if (request.Uri.StartsWith("/")) {
				request = GenericResponse(request, response);
			}
			else {
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

		static ISequenceContext _context = null;
		private static HttpRequestHead SequencePost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response) {


			// when you subecribe to the request body before calling OnResponse,
			// the server will automatically send 100-continue if the client is 
			// expecting it.
			var sequenceFile = request.Uri.ToLower().Remove(0, 10);
			var sequenceFileNames = SequenceService.Instance.GetAllSequenceFileNames();
			var sequenceFilePath = sequenceFileNames.Where(x => System.IO.Path.GetFileName(x).Equals(sequenceFile, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
			if (System.IO.File.Exists(sequenceFilePath)) {
				var sequence = SequenceService.Instance.Load(sequenceFilePath);
				if (_context != null && _context.IsRunning)
					_context.Stop();
				if (_context.Name != sequence.Name) {

					_context = VixenSystem.Contexts.CreateSequenceContext(new ContextFeatures(ContextCaching.NoCaching), sequence);
					 
				}


				_context.Play(TimeSpan.Zero, sequence.Length);

			}
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

		private static HttpRequestHead EchoPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response) {
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
		private static HttpRequestHead GenericResponse(HttpRequestHead request, IHttpResponseDelegate response) {

			//List<Nodes> nodes = new List<Nodes>();

			//foreach (ElementNode element in VixenSystem.Nodes.GetRootNodes()) {
			//	nodes.Add(AddNode(element));
			//}
			StringBuilder sb = new StringBuilder();
			sb.AppendLine("<html>");
			sb.AppendLine("<head>");
			sb.AppendLine("<title>Vixen 3 Web Interface</title>");
			sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");

			sb.AppendLine("<script src=\"http://code.jquery.com/jquery-1.9.1.min.js\"></script>");
			sb.AppendLine("<script src=\"http://code.jquery.com/mobile/1.3.2/jquery.mobile-1.3.2.min.js\"></script>");
			sb.AppendLine("<link rel=\"stylesheet\" href=\"http://code.jquery.com/mobile/1.3.2/jquery.mobile-1.3.2.min.css\" />");

			sb.AppendLine("</head>");
			sb.AppendLine("<body>");
			GenerateSequencesHtml(sb);
			GenerateElementsHtml(sb);
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

		private static void GenerateNavBar(StringBuilder sb) {
			sb.AppendLine("<div data-role=\"navbar\">");
			sb.AppendLine("<ul>");
			sb.AppendLine("<li><a href=\"#\">Home</a></li>");
			sb.AppendLine("<li><a href=\"#Elements\">Elements</a></li>");
			sb.AppendLine("<li><a href=\"#Sequences\">Sequences</a></li>");
			sb.AppendLine("</ul>");
			sb.AppendLine("</div>");
		}
		#region Elements

		private static void GenerateElementsHtml(StringBuilder sb) {
			sb.AppendLine("<div data-role=\"page\" id=\"Elements\">");

			sb.AppendLine("<script  type=\"text/javascript\">");
			sb.AppendLine("function turnElementOnOff(id, state) {");
			sb.AppendLine("alert('Element =' + id + ' State = ' + state);");
			sb.AppendLine("}");

			sb.AppendLine("</script>");
			sb.AppendLine("<div data-role=\"header\"><h1>Vixen 3 Elements</h1></div>");
			GenerateNavBar(sb);
			sb.AppendLine("<div data-role=\"content\">");

			sb.AppendLine("<p>Test Elements</p>");

			foreach (ElementNode element in VixenSystem.Nodes.GetRootNodes()) {
				AddNodeToTree(sb, element);
			}

			sb.AppendLine("</div>");
			sb.AppendLine("</div>");
		}


		private static void AddNodeToTree(StringBuilder sb, ElementNode elementNode) {

			sb.AppendLine("<div data-role=\"collapsible\">");

			sb.AppendFormat("<h4>{0}</h4>", elementNode.Name);
			sb.AppendFormat("<p>ElementID = {0}</p>", elementNode.Id);
			//sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"turnElementOnOff('{0}','{1}')\">Turn {1}</a>", elementNode.Id, "On");
			//sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"turnElementOnOff('{0}','{1}')\">Turn {1}</a>", elementNode.Id, "Off");

			foreach (ElementNode childNode in elementNode.Children) {
				AddNodeToTree(sb, childNode);
			}

			sb.AppendLine("</div>");
		}

		#endregion
		#region Sequences

		private static void GenerateSequencesHtml(StringBuilder sb) {
			sb.AppendLine("<div data-role=\"page\" id=\"Sequences\">");
			sb.AppendLine("<script  type=\"text/javascript\">");
			sb.AppendLine("function playSequence(id ) {");
			sb.AppendLine("$.post('/sequence/' + id);");

			sb.AppendLine("}");

			sb.AppendLine("</script>");
			sb.AppendLine("<div data-role=\"header\"><h1>Vixen 3 Sequences</h1></div>");
			GenerateNavBar(sb);

			sb.AppendLine("<div data-role=\"content\">");

			var sequences = SequenceService.Instance.GetAllSequenceFileNames().Select(x => System.IO.Path.GetFileName(x));
			sequences.ToList().ForEach(sequence => {
				sb.AppendLine("<div data-role=\"collapsible\">");

				sb.AppendFormat("<h4>{0}</h4>", sequence);
				//sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"playSequence('{0}')\">Play</a>", sequence);
				//sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"turnElementOnOff('{0}','{1}')\">Turn {1}</a>", elementNode.Id, "Off");

				sb.AppendLine("</div>");


			});

			sb.AppendLine("</div>");
			sb.AppendLine("</div>");
		}

		#endregion

	}
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Kayak;
using Kayak.Http;
using Newtonsoft.Json.Linq;
using Vixen.Execution;
using Vixen.Execution.Context;
using Vixen.Services;
using Vixen.Sys;
using VixenModules.Effect.SetLevel;

namespace VixenModules.App.WebServer.HTTP
{
	class RequestDelegate : IHttpRequestDelegate
	{
		public void OnRequest(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			if (request.Method.ToUpperInvariant() == "POST" && request.Uri.ToLower().StartsWith("/element")) {
				request = ElementPost(request, requestBody, response);
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

		private static HttpRequestHead ElementPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			var objs = request.Uri.Split('/');
			Guid elementID = Guid.Empty;
			bool allElements = false;
			int seconds = 30;
			Color elementColor = Color.White;


			//Look for the Element ID or if we are using ALL Elements
			foreach (var item in objs) {
				if (Guid.TryParse(item, out elementID))
					break;
				else if (item.Equals("all", StringComparison.CurrentCultureIgnoreCase)) {
					allElements = true;
					break;
				}
			}
			foreach (var item in objs) {
				if (int.TryParse(item, out seconds))
					break;
			}
			foreach (var item in objs) {
				try {
					var color = Color.FromName(item);
					if (color.Name.Equals(item, StringComparison.CurrentCultureIgnoreCase)) {
						elementColor = color;
						break;
					}
				} catch (Exception) { }
			}

			SetLevel effect = new SetLevel();
			effect.TimeSpan = TimeSpan.FromSeconds(seconds);
			effect.Color = elementColor;
			effect.IntensityLevel = 1;

			if (allElements) {
				effect.TargetNodes = VixenSystem.Nodes.GetRootNodes().ToArray();
			} else
				effect.TargetNodes = new[] { VixenSystem.Nodes.GetElementNode(elementID) };

			Module.LiveSystemContext.Execute(new EffectNode(effect, TimeSpan.Zero));

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

		private static HttpRequestHead GenericResponse(HttpRequestHead request, IHttpResponseDelegate response)
		{

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
			//GenerateSequencesHtml(sb);
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

		private static void GenerateNavBar(StringBuilder sb)
		{
			sb.AppendLine("<div data-role=\"navbar\">");
			sb.AppendLine("<ul>");
			sb.AppendLine("<li><a href=\"#\">Home</a></li>");
			sb.AppendLine("<li><a href=\"#Elements\">Elements</a></li>");
			//sb.AppendLine("<li><a href=\"#Sequences\">Sequences</a></li>");
			sb.AppendLine("</ul>");
			sb.AppendLine("</div>");
		}
		#region Elements

		private static void GenerateElementsHtml(StringBuilder sb)
		{
			sb.AppendLine("<div data-role=\"page\" id=\"Elements\">");

			sb.AppendLine("<script  type=\"text/javascript\">");
			sb.AppendLine("var txtValue='30';");
			sb.AppendLine("function setValue(x){  txtValue=x;}");

			sb.AppendLine("function turnElementOn(id,color) {");
			
			sb.AppendLine("$.post('/element/' + id + '/' + txtValue + '/' + color);");
			/*
			 $.post('/Elements/' + id);
			 
			 */
			sb.AppendLine("}");

			sb.AppendLine("</script>");
			sb.AppendLine("<div data-role=\"header\" data-position=\"fixed\"><h1>Vixen 3 Elements</h1></div>");
			GenerateNavBar(sb);
			sb.AppendLine("<div data-role=\"content\">");



			foreach (ElementNode element in VixenSystem.Nodes.GetRootNodes()) {
				AddNodeToTree(sb, element);
			}
			if (VixenSystem.Nodes.GetRootNodes().Count() > 0) {
				sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"turnElementOn('{0}')\">Turn All Elements On</a>", "All");
			}
			sb.AppendLine("<p>Element Test Time in Seconds <input id=\"txtDefaultTime\" type=\"number\" value=\"30\" /></p>");

			sb.AppendLine("</div>");

			sb.AppendLine("<div data-role=\"footer\" data-position=\"fixed\">");

			sb.AppendLine("<p>Element Test Time in Seconds <input id=\"txtDefaultTime\" type=\"number\" value=\"30\" onchange=\"setValue($(this).val())\" /></p>");

			sb.AppendLine("</div>");


			sb.AppendLine("</div>");
		}


		private static void AddNodeToTree(StringBuilder sb, ElementNode elementNode)
		{

			sb.AppendLine("<div data-role=\"collapsible\">");
			string[] colors = { "White", "Blue" };
			sb.AppendFormat("<h4>{0}</h4>", elementNode.Name);
			//sb.AppendFormat("<p>ElementID = {0}</p>", elementNode.Id);
			//sb.Append("<p>Turn Element on:</p>");
			//sb.AppendLine("<div data-type=\"horizontal\" data-role=\"controlgroup\">");
			//foreach (var item in colors) {
				
				sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"turnElementOn('{0}', 'White')\">Turn On: {1}</a>", elementNode.Id, elementNode.Name);
			//}
			//sb.AppendLine("</div>");
			foreach (ElementNode childNode in elementNode.Children) {
				AddNodeToTree(sb, childNode);
			}

			sb.AppendLine("</div>");
		}

		#endregion
		#region Sequences

		private static void GenerateSequencesHtml(StringBuilder sb)
		{
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

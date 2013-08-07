using System.IO;
using System.Reflection;
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
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public void OnRequest(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			if (request.Method.ToUpperInvariant() == "POST" && request.Uri.ToLower().StartsWith("/element")) {
				request = ElementPost(request, requestBody, response);
			} else if (request.Uri.StartsWith("/resx")) {
				request = GetResource(request, response);
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
		private static Assembly _assembly = null;
		private static HttpRequestHead GetResource(HttpRequestHead request, IHttpResponseDelegate response)
		{


			Console.WriteLine(request.Uri.ToString());
			var contentRequest = request.Uri.ToString().ToLower().Replace("/resx/", string.Empty).Replace('/', '.');
			Console.WriteLine(contentRequest);

			var headers = new HttpResponseHead() {
				Status = "200 OK",
				Headers = new Dictionary<string, string>() 
						{
							{ "Content-Type", "text/plain" },
							{ "Connection", "close" }, 
							{ "Content-Length", "0" }
						}
			};

			BufferedProducer producer = new BufferedProducer("");

			if (_assembly == null)
				_assembly = Assembly.GetAssembly(typeof(VixenModules.App.WebServer.HTTP.RequestDelegate));


			try {
				var resources = _assembly.GetManifestResourceNames();
				var resourceItem = resources.Where(n => n.EndsWith(contentRequest, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();

				if (resourceItem == null)
					throw new ApplicationException(string.Format("Requested Resource {0} does not exist.", contentRequest));

				using (var _Stream = _assembly.GetManifestResourceStream(resourceItem)) {
					var bytes = ReadFully(_Stream);

					headers.Headers["Content-Length"] = bytes.Length.ToString();
					headers.Headers["Content-Type"] = GetContentType(contentRequest);

					producer = new BufferedProducer(bytes);
				}

			} catch (Exception e) {
				Logging.ErrorException(e.Message, e);
				headers.Status = "404 Not Found";
			}



			response.OnResponse(headers, producer);
			return request;
		}

		private static string GetContentType(string fileName)
		{

			string contentType = "application/octetstream";

			string ext = System.IO.Path.GetExtension(fileName).ToLower();

			Microsoft.Win32.RegistryKey registryKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);

			if (registryKey != null && registryKey.GetValue("Content Type") != null)
				contentType = registryKey.GetValue("Content Type").ToString();

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
			sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");

			sb.AppendLine("<script src=\"resx/jquery-1.10.2.min.js\"></script>");
			sb.AppendLine("<script src=\"resx/jquery.mobile-1.3.2.min.js\"></script>");
			sb.AppendLine("<script src=\"resx/jquery.fittext.js\"></script>");
			sb.AppendLine("<link rel=\"stylesheet\" href=\"resx/jquery.mobile-1.3.2.min.css\" />");

			sb.AppendLine("<script src=\"resx/vixen.js\"></script>");
			sb.AppendLine("<link rel=\"stylesheet\" href=\"resx/vixen.css\" />");

			sb.AppendLine("</head>");
			sb.AppendLine("<body>");
			//GenerateSequencesHtml(sb);
			GenerateHomeHtml(sb);
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

		private static void GenerateNavBar(StringBuilder sb, string title)
		{
			sb.AppendFormat("<div class=\"navBarClass\">");
			sb.AppendLine("<div class=\"headerLogo\">");
			sb.AppendLine("<img class=\"logo\" src=\"resx/images/v3logo.png\" />");
			sb.AppendFormat("<div id=\"headerLogoTitle\">{0}</div>", title);
			
			sb.AppendLine("</div>");
			sb.AppendLine("<div data-role=\"navbar\">");
			sb.AppendLine("<ul>");
			sb.AppendLine("<li><a href=\"#Home\">Home</a></li>");
			sb.AppendLine("<li><a href=\"#Elements\">Elements</a></li>");
			//sb.AppendLine("<li><a href=\"#Sequences\">Sequences</a></li>");
			sb.AppendLine("</ul>");
			sb.AppendFormat("<h2>{0}</h2>", title);
			sb.AppendLine("</div></div>");

		}


		private static void GenerateHomeHtml(StringBuilder sb)
		{
			sb.AppendLine("<div data-role=\"page\" id=\"Home\" >");
			GenerateNavBar(sb, "");
			sb.AppendLine("<div data-role=\"content\">");
		
			sb.AppendLine("<h1>Home!</h1>");
			sb.AppendLine("</div>");
			sb.AppendLine("</div>");
		}
		private static void GenerateElementsHtml(StringBuilder sb)
		{
			sb.AppendLine("<div data-role=\"page\" id=\"Elements\">");
			GenerateNavBar(sb, "Elements");
			sb.AppendLine("<div data-role=\"content\">");

			foreach (ElementNode element in VixenSystem.Nodes.GetRootNodes()) {
				AddNodeToTree(sb, element);
			}

			if (VixenSystem.Nodes.GetRootNodes().Count() > 0) {
				sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"turnElementOn('{0}')\">Turn All Elements On</a>", "All");
			}

			sb.AppendLine("<p id=\"colorpickerHolder\">");
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


			sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"turnElementOn('{0}', 'White')\">Turn On: {1}</a>", elementNode.Id, elementNode.Name);

			foreach (ElementNode childNode in elementNode.Children) {
				AddNodeToTree(sb, childNode);
			}

			sb.AppendLine("</div>");
		}


		//private static void GenerateSequencesHtml(StringBuilder sb)
		//{
		//	sb.AppendLine("<div data-role=\"page\" id=\"Sequences\">");
		//	sb.AppendLine("<script  type=\"text/javascript\">");
		//	sb.AppendLine("function playSequence(id ) {");
		//	sb.AppendLine("$.post('/sequence/' + id);");

		//	sb.AppendLine("}");

		//	sb.AppendLine("</script>");
		//	GenerateNavBar(sb, "Vixen 3 Sequences");

		//	sb.AppendLine("<div data-role=\"content\">");

		//	var sequences = SequenceService.Instance.GetAllSequenceFileNames().Select(x => System.IO.Path.GetFileName(x));
		//	sequences.ToList().ForEach(sequence => {
		//		sb.AppendLine("<div data-role=\"collapsible\">");

		//		sb.AppendFormat("<h4>{0}</h4>", sequence);
		//		//sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"playSequence('{0}')\">Play</a>", sequence);
		//		//sb.AppendFormat("<a href=\"#\" data-role=\"button\" onclick=\"turnElementOnOff('{0}','{1}')\">Turn {1}</a>", elementNode.Id, "Off");

		//		sb.AppendLine("</div>");


		//	});

		//	sb.AppendLine("</div>");
		//	sb.AppendLine("</div>");
		//}

	}
}

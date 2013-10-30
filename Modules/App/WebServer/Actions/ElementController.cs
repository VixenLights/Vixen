using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Kayak;
using Kayak.Http;
using Vixen.Sys;
using VixenModules.App.WebServer.HTTP;
using VixenModules.App.WebServer.Model;
using VixenModules.Effect.SetLevel;
using VixenModules.Property.Color;
using Element = VixenModules.App.WebServer.Model.Element;

namespace VixenModules.App.WebServer.Actions
{
	public class ElementController:BaseController
	{
		public override void ProcessPost(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
		{
			throw new NotImplementedException();
		}

		public override void ProcessGet(HttpRequestHead request, IHttpResponseDelegate response)
		{
			if (request.Uri.StartsWith("/api/element"))
			{
				if (request.Uri.StartsWith("/api/element/getElements"))
				{
					GetElements(request, response);
					return;
				}
				if (request.Uri.StartsWith("/api/element/on"))
				{
					TurnOnElement(request, response);
					return;
				}
			}

			UnsupportedOperation(request, response);
		}

		private void GetElements(HttpRequestHead request, IHttpResponseDelegate response)
		{
			IEnumerable<ElementNode> elementNodes = VixenSystem.Nodes.GetRootNodes();
			var elements = new List<Element>();
			foreach (var elementNode in elementNodes)
			{
				AddNodes(elements,elementNode);
			}
			
			SerializeResponse(elements,response);
		}

		private void AddNodes(List<Element> elements, ElementNode elementNode)
		{
			var element = new Element
			{
				Id = elementNode.Id,
				Name = elementNode.Name
			};
			
			element.Colors = ColorModule.getValidColorsForElementNode(elementNode, true).Select(ColorTranslator.ToHtml).ToList();
			
			elements.Add(element);
			if (!elementNode.IsLeaf)
			{
				var children = new List<Element>();
				element.Children = children;
				foreach (var childNode in elementNode.Children)
				{
					AddNodes(children, childNode);
				}
			}	
		}

		private static void TurnOnElement(HttpRequestHead request, IHttpResponseDelegate response)
		{
			var status = new Status();
			NameValueCollection parms = GetParameters(request);

			if (!parms.HasKeys() && parms["id"] != null && parms["time"] != null && parms["color"]!=null)
			{
				HttpResponseHead headers = GetHeaders(0, HttpStatusCode.BadRequest.ToString());
				response.OnResponse(headers, new BufferedProducer(""));
				return;
			}

			if (parms["color"].Length != 7 || !parms["color"].StartsWith("#"))
			{
				status.Message = "Invalid color. Must be Hex.";
				SerializeResponse(status,response);
				return;	
			}

			Guid elementId = Guid.Empty;
			bool allElements = false;
			int seconds;

			if ("all".Equals(parms["id"]))
			{
				allElements = true;
			} else
			{
				Guid.TryParse(parms["id"], out elementId);	
			}
			if (!int.TryParse(parms["time"], out seconds))
			{
				status.Message = "Time must be numeric.";
				SerializeResponse(status,response);
				return;
			}
			
			Color elementColor = ColorTranslator.FromHtml(parms["color"]);

			//TODO the following logic for all does not properly deal with discrete color elements when turning all on
			//TODO they will not respond to turning on white if they are set up with a filter.
			//TODO enhance this to figure out what colors there are and turn them all on when we are turning all elements on.

			var effect = new SetLevel
			{
				TimeSpan = TimeSpan.FromSeconds(seconds),
				Color = elementColor,
				IntensityLevel = 1,
				TargetNodes =
					allElements ? VixenSystem.Nodes.GetRootNodes().ToArray() : new[] {VixenSystem.Nodes.GetElementNode(elementId)}
			};

			Module.LiveSystemContext.Execute(new EffectNode(effect, TimeSpan.Zero));
			status.Message = string.Format("{0} element(s) turned on for {1} seconds at 100% intensity.",
				allElements?"All":VixenSystem.Nodes.GetElementNode(elementId).Name, seconds);

			SerializeResponse(status,response);
		}

		
	}
}

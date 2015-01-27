using System.Collections.Generic;
using System.Web.Http;
using Microsoft.SqlServer.Server;
using VixenModules.App.WebServer.Filter;
using VixenModules.App.WebServer.Model;
using VixenModules.App.WebServer.Service;

namespace VixenModules.App.WebServer.Controllers
{
	[ArgumentExceptionFilter]
	public class ElementController: ApiController
	{

		public IEnumerable<Element> Get()
		{
			var helper = new ElementsHelper();
			return helper.GetElements();
		}
 
		public IEnumerable<Element> GetElements()
		{
			var helper = new ElementsHelper();
			return helper.GetElements();
		}

		[HttpGet]
		public IEnumerable<Element> SearchElements(string q)
		{
			var helper = new ElementsHelper();
			return helper.SearchElements(q);
		}

		public IEnumerable<Element> GetChildElements(string id)
		{
			var helper = new ElementsHelper();
			return helper.GetChildElements(id);
		}

		public IEnumerable<Element> GetParentElements(string id)
		{
			var helper = new ElementsHelper();
			return helper.GetParentElements(id);
		}

		[HttpGet]
		public Status On(string id, int time, string color)
		{
			return ElementsHelper.TurnOnElement(id, time, color);
		}

		[HttpPost]
		public Status Effect(ElementEffect effect)
		{
			return ElementsHelper.Effect(effect);
		}

	}
}

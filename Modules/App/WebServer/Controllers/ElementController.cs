using System.Collections.Generic;
using System.Web.Http;
using VixenModules.App.WebServer.Filter;
using VixenModules.App.WebServer.Model;
using VixenModules.App.WebServer.Service;

namespace VixenModules.App.WebServer.Controllers
{
	[ArgumentExceptionFilter]
	public class ElementController: BaseApiController
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

		[HttpPost]
		public Status On(ElementState elementState)
		{
			return ElementsHelper.TurnOnElement(elementState.Id, elementState.Duration, elementState.Intensity, elementState.Color);
		}

		[HttpPost]
		public Status GroupOn([FromBody] List<ElementState> elementState)
		{
			return ElementsHelper.TurnOnElements(elementState);
		}

		[HttpPost]
		public Status GroupOff([FromBody] List<ElementState> elementState)
		{
			return ElementsHelper.TurnOffElements(elementState);
		}

		[HttpPost]
		public Status Off(Element element)
		{
			return ElementsHelper.TurnOffElement(element.Id);
		}

		[HttpPost]
		public Status ClearAll()
		{
			return ElementsHelper.ClearActiveEffects();
		}

		[HttpPost]
		public Status Effect(ElementEffect effect)
		{
			return ElementsHelper.Effect(effect);
		}

	}
}

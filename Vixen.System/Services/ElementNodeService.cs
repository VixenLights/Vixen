using System.Linq;
using Vixen.Sys;

namespace Vixen.Services
{
	public class ElementNodeService
	{
		private static ElementNodeService _instance;

		private ElementNodeService()
		{
		}

		public static ElementNodeService Instance
		{
			get { return _instance ?? (_instance = new ElementNodeService()); }
		}

		public ElementNode CreateSingle(ElementNode parentNode, string name = null, bool createElement = true,
		                                bool uniquifyName = true)
		{
			name = name ?? "Unnamed";

			ElementNode elementNode = VixenSystem.Nodes.AddNode(name, parentNode, uniquifyName);

			Element element = createElement ? _CreateElement(name) : null;
			elementNode.Element = element;
			VixenSystem.Elements.AddElement(element);

			return elementNode;
		}

		public ElementNode[] CreateMultiple(ElementNode parentNode, int count, bool createElement = true,
		                                    bool uniquifyNames = true)
		{
			return Enumerable.Range(0, count).Select(x => CreateSingle(parentNode, null, createElement, uniquifyNames)).ToArray();
		}

		public ElementNode ImportTemplateOnce(string templateFileName, ElementNode parentNode)
		{
			ElementNodeTemplate elementNodeTemplate = FileService.Instance.LoadElementNodeTemplateFile(templateFileName);
			if (elementNodeTemplate == null) return null;

			VixenSystem.Nodes.AddChildToParent(elementNodeTemplate.ElementNode, parentNode);

			return elementNodeTemplate.ElementNode;
		}

		public ElementNode[] ImportTemplateMany(string templateFileName, ElementNode parentNode, int count)
		{
			return
				Enumerable.Range(0, count).Select(x => ImportTemplateOnce(templateFileName, parentNode)).Where(x => x != null).
					ToArray();
		}

		public void Rename(ElementNode elementNode, string name)
		{
			elementNode.Name = name;
		}

		private Element _CreateElement(string name)
		{
			return new Element(name);
		}
	}
}
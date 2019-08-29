using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys
{
	[Serializable]
	public class ElementNodeProxy
	{
		public ElementNodeProxy()
		{
			
		}
		public ElementNodeProxy(ElementNode node)
		{
			Id = node.Id;
			Name = node.Name;
			Element = node.Element!=null?new ElementProxy(node.Element):null;
			Children = node.Children.Select(x => new ElementNodeProxy(x)).ToList();
		}
		public Guid Id { get; set; }

		public string Name { get; set; }

		public ElementProxy Element { get; set; }

		public List<ElementNodeProxy> Children { get; set; }
	}
}

using System;

namespace Vixen.Sys
{
	public class ElementProxy
	{
		public ElementProxy()
		{
			
		}

		public ElementProxy(Element element)
		{
			Id = element.Id;
			Name = element.Name;
		}

		public Guid Id { get; set; }

		public string Name { get; set; }
	}
}

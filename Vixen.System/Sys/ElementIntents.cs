using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys
{
	public class ElementIntents : Dictionary<Guid, IIntentNode[]>
	{
		// For better storytelling.
		public IEnumerable<Guid> ElementIds
		{
			get { return Keys; }
		}

		public void AddIntentNodeToElement(Guid elementId, IIntentNode[] intentNodes)
		{
			if (intentNodes == null || intentNodes.Length == 0) return;
			this[elementId] = intentNodes;
			
		}

		public void AddIntentNodesToElements(ElementIntents elementIntents)
		{
			foreach (KeyValuePair<Guid, IIntentNode[]> elementIntent in elementIntents)
			{
				AddIntentNodeToElement(elementIntent.Key,elementIntent.Value);
			}	
		}

	}
}
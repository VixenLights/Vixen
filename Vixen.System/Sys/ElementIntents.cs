using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public class ElementIntents : Dictionary<Guid,IIntentNode[]> {
		// For better storytelling.
		public IEnumerable<Guid> ElementIds {
			get { return Keys; }
		}

		public void AddIntentNodeToElement(Guid elementId, IIntentNode[] intentNodes) {
			if(intentNodes == null || intentNodes.Length == 0) return;

			if(!ContainsKey(elementId)) {
				_AddIntentNode(elementId, intentNodes);
			}
		}

		public void AddIntentNodesToElements(ElementIntents elementIntents) {
			foreach(Guid elementId in elementIntents.ElementIds) {
				IIntentNode[] intentNodes = elementIntents[elementId];
				AddIntentNodeToElement(elementId, intentNodes);
			}
		}

		private void _AddIntentNode(Guid elementId, IIntentNode[] intentNodes) {
			this[elementId] = intentNodes;
		}
	}
}

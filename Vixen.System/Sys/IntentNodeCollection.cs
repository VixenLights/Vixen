using System.Collections.Generic;

namespace Vixen.Sys {
	public class IntentNodeCollection : List<IIntentNode> {
		public IntentNodeCollection() {
		}

		public IntentNodeCollection(IEnumerable<IIntentNode> intentNodes) {
			AddRange(intentNodes);
		}
	}
}
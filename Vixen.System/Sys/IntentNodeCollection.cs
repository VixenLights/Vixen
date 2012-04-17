using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class IntentNodeCollection : List<IntentNode> {
		public IntentNodeCollection() {
		}

		public IntentNodeCollection(IEnumerable<IntentNode> intentNodes) {
			AddRange(intentNodes);
		}
	}
}

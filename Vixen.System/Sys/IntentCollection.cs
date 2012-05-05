using System.Collections.Generic;

namespace Vixen.Sys {
	public class IntentCollection : List<IIntent> {
		public IntentCollection() {
		}

		public IntentCollection(IEnumerable<IIntent> intents) {
			AddRange(intents);
		}
	}
}

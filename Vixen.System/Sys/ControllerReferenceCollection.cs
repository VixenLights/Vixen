using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class ControllerReferenceCollection : List<ControllerReference> {
		public ControllerReferenceCollection() {
		}

		public ControllerReferenceCollection(IEnumerable<ControllerReference> items)
			: base(items) {
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class ElementCommandsPerSecondValue : RateValue {
		private Element _element;

		public ElementCommandsPerSecondValue(Element element)
			: base("Element Commands Per Second - " + element.Name) {
			_element = element;
		}
	}
}

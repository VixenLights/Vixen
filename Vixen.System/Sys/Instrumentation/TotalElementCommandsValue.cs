using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class TotalElementCommandsValue : CountValue {
		private Element _element;

		public TotalElementCommandsValue(Element element)
			: base("Total Element Commands - " + element.Name) {
			_element = element;
		}
	}
}

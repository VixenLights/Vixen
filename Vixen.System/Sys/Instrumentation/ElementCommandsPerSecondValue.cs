using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	internal class ElementCommandsPerSecondValue : RateValue
	{
		private Element _element;

		public ElementCommandsPerSecondValue(Element element)
			: base(string.Format("Element Commands Per Second - {0}", element.Name))
		{
			_element = element;
		}
	}
}
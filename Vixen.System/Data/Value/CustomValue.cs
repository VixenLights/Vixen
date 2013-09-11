using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Data.Value
{
	public struct CustomValue : IIntentDataType
	{
		public CustomValue(object value) {
			Value = value;
		}
		public object Value;

	}
}

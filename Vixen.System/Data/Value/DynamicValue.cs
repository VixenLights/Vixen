using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Data.Value
{
	public struct DynamicValue : IIntentDataType
	{
		public DynamicValue(dynamic value) {
			Value = value;
		}
		public dynamic Value;

	}
}

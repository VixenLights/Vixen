using System;
using System.Collections.Generic;

namespace Vixen.Sys
{
	public class ElementIntentStates : Dictionary<Guid, IIntentStates>
	{
		public ElementIntentStates()
		{
		}

		public ElementIntentStates(IDictionary<Guid, IIntentStates> values)
			: base(values)
		{
		}
	}
}
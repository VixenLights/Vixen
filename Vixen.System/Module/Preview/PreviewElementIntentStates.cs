using System;
using System.Collections.Generic;

namespace Vixen.Preview
{
	public class PreviewElementIntentStates : Dictionary<Vixen.Sys.Element, Vixen.Sys.IIntentStates>
	{
		public PreviewElementIntentStates()
		{
		}

		public PreviewElementIntentStates(IDictionary<Vixen.Sys.Element, Vixen.Sys.IIntentStates> values)
			: base(values)
		{
		}
	}
}
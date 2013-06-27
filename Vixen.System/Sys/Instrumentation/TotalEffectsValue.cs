using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation
{
	internal class TotalEffectsValue : CountValue
	{
		public TotalEffectsValue()
			: base("Total Effects Written")
		{
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timeline
{
	public static class ExtensionMethods
	{
		public static TimeSpan Scale(this TimeSpan ts, double scale)
		{
			return TimeSpan.FromTicks((long)(ts.Ticks * scale));
		}

		public static TimeSpan Scale(this TimeSpan ts, long scale)
		{
			return TimeSpan.FromTicks(ts.Ticks * scale);
		}

	}
}

using System;
using Vixen.Data.Value;

namespace Vixen.Intent
{
	public class StaticDiscreteLightingIntent: StaticIntent<DiscreteValue>
	{
		/// <inheritdoc />
		public StaticDiscreteLightingIntent(DiscreteValue value, TimeSpan timeSpan) : base(value, timeSpan)
		{
		}
	}
}

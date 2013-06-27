using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Trigger
{
	public enum AnalogInputTriggerCondition
	{
		ExceedsThreshold = 1,
		SubcedesThreshold,
		WithinRange,
		WithoutRange
	};
}
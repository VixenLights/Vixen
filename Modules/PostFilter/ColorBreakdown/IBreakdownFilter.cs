using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Sys;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	internal interface IBreakdownFilter
	{
		double GetIntensityForState(IIntentState intentValue);
	}
}

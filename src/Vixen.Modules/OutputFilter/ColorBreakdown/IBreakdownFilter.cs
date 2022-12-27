using Vixen.Sys;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	internal interface IBreakdownFilter
	{
		double GetIntensityForState(IIntentState intentValue);
	}
}

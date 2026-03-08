using System.ComponentModel;

namespace Vixen.Sys.Props
{
	public enum StringTypes
	{
		[Description("All Lights are a single color")] 
		SingleColor,

		[Description("String is made up of multiple independant colors")]
		MultiColor,

		[Description("Full RGB color mixing")]
		ColorMixingRGB
	}
}
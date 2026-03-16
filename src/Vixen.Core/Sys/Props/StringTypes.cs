using System.ComponentModel;

namespace Vixen.Sys.Props
{
	public enum StringTypes
	{
		[Description("All Lights are a single color")] 
		SingleColor,

		[Description("Multiple Independent Color Strings")]
		MultiColor,

		[Description("Full RGB color mixing")]
		ColorMixingRGB
	}
}
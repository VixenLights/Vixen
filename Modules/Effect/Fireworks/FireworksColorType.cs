using System;
using System.ComponentModel;

namespace VixenModules.Effect.Fireworks
{
	public enum FireworksColorType
	{
		[Description("Standard")]
		Standard,
		[Description("RainBow")]
		RainBow,
		[Description("Range")]
		Range,
		[Description("Palette")]
		Palette,
		[Description("Random")]
		Random
	}
}

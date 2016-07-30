using System;
using System.ComponentModel;

namespace VixenModules.Effect.Fireworks
{
	public enum FireworksColorType
	{
		[Description("Standard Bursts")]
		Standard,
		[Description("RainBow Particles")]
		RainBow,
		[Description("Range Particles")]
		Range,
		[Description("Palette Particles")]
		Palette,
		[Description("Random Bursts")]
		Random
	}
}

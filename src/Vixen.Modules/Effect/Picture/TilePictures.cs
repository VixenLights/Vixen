﻿using System.ComponentModel;

namespace VixenModules.Effect.Picture
{
	public enum TilePictures
	{
		//Did this as None is no longer required however users may have it saved in a sequence.
		[Browsable(false)]
		[Description("None")]
		None,
		[Description("Blue Glow Dots")]
		BlueGlowDots,
		[Description("Bubbles")]
		Bubbles,
		[Description("Checkers")]
		Checkers,
		[Description("Rain")]
		Rain,
		[Description("Damask")]
		Damask,
		[Description("VintageDamask")]
		VintageDamask,
		[Description("Diamonds")]
		Diamonds,
		[Description("Snowflakes1")]
		Snowflakes1,
		[Description("Snowflakes2")]
		Snowflakes2,
		[Description("Stripes1")]
		Stripes1,
		[Description("Stripes2")]
		Stripes2,
		[Description("TuttiFruitti")]
		TuttiFruitti
	}
}
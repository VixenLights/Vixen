﻿using System.ComponentModel;

namespace VixenModules.Effect.Picture
{
	public enum EffectType
	{
		[Description("None")]
		RenderPictureNone,
		[Description("Tiles")]
		RenderPictureTiles,
		[Description("Left")]
		RenderPictureLeft,
		[Description("Right")]
		RenderPictureRight,
		[Description("Up")]
		RenderPictureUp,
		[Description("Down")]
		RenderPictureDown,
		[Description("Up & Left")]
		RenderPictureUpleft,
		[Description("Down & Left")]
		RenderPictureDownleft,
		[Description("Up & Right")]
		RenderPictureUpright,
		[Description("Down & Right")]
		RenderPictureDownright,
		[Description("Peekaboo 0")]
		RenderPicturePeekaboo0,
		[Description("Peekaboo 90")]
		RenderPicturePeekaboo90,
		[Description("Peekaboo 180")]
		RenderPicturePeekaboo180,
		[Description("Peekaboo 270")]
		RenderPicturePeekaboo270,
		[Description("Wiggle")]
		RenderPictureWiggle
		
	}
}
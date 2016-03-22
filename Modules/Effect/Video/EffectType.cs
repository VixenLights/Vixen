using System.ComponentModel;

namespace VixenModules.Effect.Video
{
	public enum EffectType
	{
		[Description("Left")]
		RenderPictureLeft,
		[Description("Right")]
		RenderPictureRight,
		[Description("Up")]
		RenderPictureUp,
		[Description("Down")]
		RenderPictureDown,
		[Description("None")]
		RenderPictureNone,
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
using System.ComponentModel;

namespace VixenModules.Effect.Dissolve
{
	public enum DissolveMarkType
	{
		[Description("Per Mark")]
		PerMark,
		[Description("Per Mark Fill")]
		PerMarkFill,
		[Description("Per Mark Dissolve")]
		PerMarkDissolve,
		[Description("Mark Label Value - %")]
		MarkLabelValue,
		[Description("Mark Label Value - Pixels")]
		MarkLabelPixels
	}
}

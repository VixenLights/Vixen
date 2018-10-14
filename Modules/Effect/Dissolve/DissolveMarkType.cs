using System.ComponentModel;

namespace VixenModules.Effect.Dissolve
{
	public enum DissolveMarkType
	{
		[Description("Per Mark")]
		PerMark,
		[Description("Mark Label Value - %")]
		MarkLabelValue,
		[Description("Mark Label Value - Pixels")]
		MarkLabelPixels
	}
}

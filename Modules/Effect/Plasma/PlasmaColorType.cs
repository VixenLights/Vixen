using System.ComponentModel;

namespace VixenModules.Effect.Plasma
{
	public enum PlasmaColorType
	{
		[Description("Custom")]
		Normal,
		[Description("Red & Green")]
		Preset1,
		[Description("Blue & Green")]
		Preset2,
		[Description("Rainbow")]
		Preset3,
		[Description("Black & White")]
		Preset4
	}
}
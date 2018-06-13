using System.ComponentModel;

namespace VixenModules.App.LipSyncApp
{
	public enum FaceComponent
	{
		[Description("Mouth")]
		Mouth,
		[Description("Outlines")]
		Outlines,
		[Description("Eyes Open")]
		EyesOpen,
		[Description("Eyes Closed")]
		EyesClosed
	}
}

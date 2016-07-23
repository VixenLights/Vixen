using System.ComponentModel;

namespace VixenModules.Effect.Curtain
{
	public enum CurtainDirection
	{
		[Description("Open")]
		CurtainOpen,
		[Description("Close")]
		CurtainClose,
		[Description("Open - Close")]
		CurtainOpenClose,
		[Description("Close - Open")]
		CurtainCloseOpen
	}
}
using System.ComponentModel;
using Vixen.TypeConverters;

namespace VixenModules.Effect.Wipe {
	
	public enum WipeDirection {
		[Description("Wipe Vertical")]
		Vertical,
		[Description("Wipe Horizontal")]
		Horizontal,
		[Description("Wipe Down/Right")]
		DownRight,
		[Description("Wipe Up/Right")]
		UpRight,
		[Description("Burst")]
		Burst,
		[Description("Circle Burst")]
		Circle,
		[Description("Diagonal Burst")]
		Diagonal
	}
}

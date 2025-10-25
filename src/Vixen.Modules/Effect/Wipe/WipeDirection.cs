using System.ComponentModel;

namespace VixenModules.Effect.Wipe {
	
	public enum WipeDirection {
		[Description("Wipe Vertical")]
		Vertical,
		[Description("Wipe Horizontal")]
		Horizontal,
		[Description("Wipe Diagonal Up")]
		DiagonalUp,
		[Description("Wipe Diagonal Down")]
		DiagonalDown,
		[Description("Burst")]
		Burst,
		[Description("Circle Burst")]
		Circle,
		[Description("Diamond Burst")]
		Dimaond
	}
}

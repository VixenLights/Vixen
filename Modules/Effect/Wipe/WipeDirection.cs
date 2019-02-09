using System.ComponentModel;
using Vixen.TypeConverters;

namespace VixenModules.Effect.Wipe {
	
	public enum WipeDirection {
		[Description("Wipe Up")]
		Up = 0,
		[Description("Wipe Down")]
		Down = 1,
		[Description("Wipe Right")]
		Right = 2,
		[Description("Wipe Left")]
		Left = 3,
		[Description("Burst In")]
		In = 4,
		[Description("Burst Out")]
		Out = 5,
		[Description("Circle Burst In")]
		CircleIn = 6,
		[Description("Circle Burst Out")]
		CircleOut = 7,
		[Description("Diagonal Burst In")]
		DiagonalIn = 8,
		[Description("Diagonal Burst Out")]
		DiagonalOut = 9,
		[Description("Wipe Down/Right")]
		DownRight = 10,
		[Description("Wipe Down/Left")]
		DownLeft = 11,
		[Description("Wipe Up/Right")]
		UpRight = 12,
		[Description("Wipe Up/Left")]
		UpLeft = 13
	}
}

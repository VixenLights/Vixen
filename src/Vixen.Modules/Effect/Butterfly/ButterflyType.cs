using System.ComponentModel;

namespace VixenModules.Effect.Butterfly
{
	public enum ButterflyType
	{
		[Description("Pulsing Circle")]
		Type2,
		[Description("Pulsing Grid of Circles")]
		Type3,
		[Description("Diagonal Boxes")]
		Type1,
		[Description("Alternating Inverted Boxes")]
		Type4,
		[Description("Flapping Wings")]
		Type5


//		1 - Pulsing Circle (formerly 2)
//2 - #1 within Diagonally Scrolling Boxes (formerly 1)
//3 - #2 with alternating inverted gradient (formerly 4)
//4 - Pulsing Grid of Circles (formerly 3)
//5 - Flapping wings
	}
}
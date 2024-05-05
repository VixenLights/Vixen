using VixenModules.App.ColorGradients;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Maintains the properties of a whirl.
	/// </summary>
	public interface IWhirl : ICloneable
	{
		/// <summary>
		/// Enables drawing of the whirl.
		/// This property is useful to determine which whirl is which in the Whirl collection.
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// Determines the position of the whirl in the x-axis.
		/// </summary>
		int X { get; set; }

		/// <summary>
		/// Determines the position of the whirl in the y-axis.
		/// </summary>
		int Y { get; set; }

		/// <summary>
		/// Width of the whirl.
		/// </summary>
		int Width { get; set; }

		/// <summary>
		/// Height of the whirl.
		/// </summary>
		int Height { get; set; }

		/// <summary>
		/// Determines the type of whirl.
		/// </summary>
		WhirlpoolMode WhirlMode { get; set; }

		/// <summary>
		/// Length of the meteor tail.
		/// </summary>
		int TailLength { get; set; }

		/// <summary>
		/// Determines which corner the whirl starts from.
		/// </summary>
		WhirlpoolStartLocation StartLocation { get; set; }

		/// <summary>
		/// Determines if the whirl moves in or out.
		/// </summary>
		WhirlpoolDirection WhirlDirection { get; set; }

		/// <summary>
		/// Determines if the whirl moves clockwise or counter-clockwise.
		/// </summary>
		WhirlpoolRotation Rotation { get; set; }

		/// <summary>
		/// Determines if the whirl is drawn in reverse.
		/// </summary>
		bool ReverseDraw { get; set; }

		/// <summary>
		/// Determines the spacing between the whirls.
		/// </summary>
		int Spacing { get; set; }

		/// <summary>
		/// Determines the thickness of the whirl.
		/// </summary>
		int Thickness { get; set; }

		/// <summary>
		/// Determines if the whirls are drawn in 3-D.
		/// </summary>
		bool Show3D { get; set; }

		/// <summary>
		/// Offset position of the whirl in the x-axis.
		/// </summary>
		int XOffset { get; set; }

		/// <summary>
		/// Offset position of the whirl in the y-axis.
		/// </summary>
		int YOffset { get; set; }

		/// <summary>
		/// Determines how the whirl is colored.
		/// </summary>
		WhirlpoolColorMode ColorMode { get; set; }

		/// <summary>
		/// Determines the length of the color bands when in color band mode.
		/// </summary>
		int BandLength { get; set; }

		/// <summary>
		/// Color of the left side of the whirl.  Applicable when color mode is LegColors.  
		/// </summary>
		ColorGradient LeftColor { get; set; }

		/// <summary>
		/// Color of the right side of the whirl.  Applicable when color mode is LegColors.  
		/// </summary>
		ColorGradient RightColor { get; set; }

		/// <summary>
		/// Color of the top side of the whirl.  Applicable when color mode is LegColors.  
		/// </summary>
		ColorGradient TopColor { get; set; }

		/// <summary>
		/// Color of the bottom side of the whirl.  Applicable when color mode is LegColors.  
		/// </summary>
		ColorGradient BottomColor { get; set; }

		/// <summary>
		/// Color of the whirl.  Applicable when color mode is GradientOverTime. 
		/// </summary>
		ColorGradient SingleColor { get; set; }

		/// <summary>
		/// Collection of colors to use in either Band or RectangularRings color mode.
		/// </summary>
		List<ColorGradient> Colors { get; set; }

		/// <summary>
		/// Brightness of the whirl.
		/// </summary>
		double Intensity { get; set; }

		/// <summary>
		/// Determines the percentage of effect duration that the completed whirl is shown.
		/// </summary>
		int PauseAtEnd { get; set; }
	}
}

using System.ComponentModel;

namespace VixenModules.Effect.Shapes
{
	public enum ShapeType
	{
		[Description("None")]
		None,
		[Description("Bounce")]
		Bounce,
		[Description("Wrap")]
		Wrap
	}

	public enum GeometricShapes
	{
		[Description("Random")]
		Random,
		[Description("Square")]
		A,
		[Description("Triangle")]
		B,
		[Description("Circle")]
		C,
		[Description("Circle")]
		D,
		[Description("Circle")]
		E,
		[Description("Circle")]
		F,
		[Description("Circle")]
		G,
		[Description("Circle")]
		H,
		[Description("Circle")]
		I
	}

	public enum ChristmasShapes
	{
		[Description("Random")]
		Random,
		[Description("A")]
		A,
		[Description("B")]
		B,
		[Description("C")]
		C,
		[Description("D")]
		D,
		[Description("E")]
		E,
		[Description("F")]
		F,
		[Description("G")]
		G,
		[Description("H")]
		H,
		[Description("I")]
		I
	}

	public enum FontShapes
	{
		[Description("Random")]
		Random,
		[Description("A")]
		A,
		[Description("B")]
		B,
		[Description("C")]
		C,
		[Description("D")]
		D,
		[Description("E")]
		E,
		[Description("F")]
		F,
		[Description("G")]
		G,
		[Description("H")]
		H,
		[Description("I")]
		I
	}

	public enum ShapeList
	{
		[Description("Geometric Shapes")]
		GeometricShapes,
		[Description("Christmas Shapes")]
		ChristmasShapes,
		[Description("Font")]
		FontShapes
	}
}
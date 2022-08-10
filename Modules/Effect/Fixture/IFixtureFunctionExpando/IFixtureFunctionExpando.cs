using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Data.Value;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Maintains the properties of a fixture function for the catch-all fixture effect.
	/// Some properties on this interface are only applicable for certain function types.
	/// Example: ColorIndex only applies to color wheel functions.
	/// </summary>
	public interface IFixtureFunctionExpando: ICloneable
	{		
		/// <summary>
		/// Name of the fixture function.
		/// </summary>
		string FunctionName { get; set; }
		
		/// <summary>
		/// Type of the fixture function.
		/// </summary>
		FunctionIdentity FunctionIdentity { get; set; }

		/// <summary>
		/// Type of function.
		/// </summary>
		FixtureFunctionType FunctionType { get; set; }

		/// <summary>
		/// Fixture functions associated with the fixture specification.
		/// </summary>
		List<FixtureFunction> FixtureFunctions { get; set; }

		/// <summary>
		/// Selected index value.
		/// This property is applicable to index functions.
		/// </summary>
		string IndexValue { get; set; }

		/// <summary>
		/// Range of the fixture setting.
		/// This property is applicable for indexes that support a range of values (curve).
		/// </summary>
		Curve Range { get; set; }

		/// <summary>
		/// Selected color wheel index value.
		/// This property is applicable to color wheel functions.
		/// </summary>
		string ColorIndexValue { get; set; }
		
		/// <summary>
		/// Selected color.
		/// This property is applicable to RGB and RGBW functions.
		/// </summary>
		ColorGradient Color { get; set; }		

		/// <summary>
		/// Color to represent the function on the timeline for certain effects.
		/// </summary>
		Color TimelineColor { get; set; }
	}
}

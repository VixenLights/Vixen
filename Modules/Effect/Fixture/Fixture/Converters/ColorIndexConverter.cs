using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Provides the color wheel names associated with a target node.
	/// </summary>
	public class ColorIndexConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Gets a collection of color wheel names associated with the node(s).
		/// </summary>
		/// <param name="context">Effects associated with the request</param>
		/// <returns>Collection of color wheel names</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Create the return collection
			List<string> values = new List<string>();

			// Cast the context to a fixture function expando object
			FixtureFunctionExpando fixtureFunction = (FixtureFunctionExpando)context.Instance;

			// Add the color wheel names to the return collection
			values.AddRange(fixtureFunction.ColorWheelIndexData.Select(colorWheelItem => colorWheelItem.Name));
									
			return new TypeConverter.StandardValuesCollection(values.ToArray());
		}

		#endregion
	}
}

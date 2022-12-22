using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Provides the color wheel names associated with the target nodes.
	/// </summary>
	public class ColorIndexConverter : EffectListTypeConverterBase<FixtureFunctionExpando>
	{
		#region Protected Methods

		/// <summary>
		/// Gets a collection of color wheel names associated with the specified fixture function expando object.
		/// </summary>
		/// <param name="fixtureFunction"></param>
		/// <returns>Collection of color wheel names</returns>
		protected override List<string> GetStandardValuesInternal(FixtureFunctionExpando fixtureFunction)
		{
			// Add the color wheel names to the return collection
			return fixtureFunction.ColorWheelIndexData.Select(colorWheelItem => colorWheelItem.Name).ToList();
		}

		#endregion
	}
}

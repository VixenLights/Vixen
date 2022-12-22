using VixenModules.Effect.Effect;

namespace VixenModules.Effect.SpinColorWheel
{
	/// <summary>
	/// Provides the spin color wheel index value names associated with the target node(s).
	/// </summary>
	public class SpinColorWheelFixtureIndexCollectionNameConverter : EffectListTypeConverterBase<SpinColorWheelModule>
	{
		#region Protected Methods

		/// <summary>
		/// Retrieves the collection of available spin color wheel index value names.
		/// </summary>
		/// <param name="context">Spin Color Wheel effect instance</param>
		/// <returns>Collection of available Spin Color Wheel index values</returns>
		protected override List<string> GetStandardValuesInternal(SpinColorWheelModule effect)
		{
			// Return the collection of Spin Color Wheel index names
			return effect.IndexValues;
		}

		#endregion
	}
}

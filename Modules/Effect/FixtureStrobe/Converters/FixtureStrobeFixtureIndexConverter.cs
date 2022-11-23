using System.Collections.Generic;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.FixtureStrobe
{
	/// <summary>
	/// Provides the fixture strobe index names associated with the target node(s).
	/// </summary>
	public class FixtureStrobeFixtureIndexCollectionNameConverter : EffectListTypeConverterBase<FixtureStrobeModule>
	{
		#region Protected Methods

		/// <summary>
		/// Retrieves the collection of available fixture strobe index value names.
		/// </summary>
		/// <param name="context">Fixture strobe effect instance</param>
		/// <returns>Collection of available fixture strobe index values</returns>
		protected override List<string> GetStandardValuesInternal(FixtureStrobeModule effect)
		{
			// Return the collection of fixture strobe index names
			return effect.IndexValues;
		}

		#endregion
	}
}

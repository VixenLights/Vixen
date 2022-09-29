using System.Collections.Generic;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Prism
{
	/// <summary>
	/// Provides the prism value names associated with a target node.
	/// </summary>
	public class PrismFixtureIndexCollectionNameConverter : EffectListTypeConverterBase<PrismModule>
	{
		#region Protected Methods

		/// <summary>
		/// Retrieves the collection of available prism index value names.
		/// </summary>
		/// <param name="context">Prism effect instance</param>
		/// <returns>Collection of available prism index names</returns>
		protected override List<string> GetStandardValuesInternal(PrismModule prismEffect)
		{
			// Return the collection of prism index names
			return prismEffect.IndexValues;
		}

		#endregion
	}
}

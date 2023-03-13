using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Gobo
{
	/// <summary>
	/// Provides the gobo value names associated with a target node.
	/// </summary>
	public class GoboFixtureIndexCollectionNameConverter : EffectListTypeConverterBase<GoboModule>
	{
		#region protected Methods

		/// <summary>
		/// Retrieves the collection of available gobo index value names.
		/// </summary>
		/// <param name="context">Gobo effect instance</param>
		/// <returns>Collection of available gobo index names</returns>
		protected override List<string> GetStandardValuesInternal(GoboModule theGoboEffect)
		{
			// Return the collection of gobo index names
			return theGoboEffect.IndexValues;
		}

		#endregion
	}
}

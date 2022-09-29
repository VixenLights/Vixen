using System.Collections.Generic;
using System.Linq;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Provides the fixture function index names associated with the target node(s).
	/// </summary>
	public class FixtureIndexCollectionNameConverter : EffectListTypeConverterBase<FixtureFunctionExpando>
	{
		#region Protected Methods

		/// <summary>
		/// Gets a collection of index names associated with the specified fixture function expando object.
		/// </summary>
		/// <param name="fixtureFunction">Fixture function to retrieve index names from</param>
		/// <returns>Collection of fixture function index names</returns>
		protected override List<string> GetStandardValuesInternal(FixtureFunctionExpando fixtureFunction)
		{
			// Add the index names to the return collection
			return fixtureFunction.IndexData.Select(idx => idx.Name).ToList();
		}

		#endregion
	}
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Provides the fixture function index names associated with a target node.
	/// </summary>
	public class FixtureIndexCollectionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Gets a collection of fixture functions associated with the node(s).
		/// </summary>
		/// <param name="context">Effects associated with the request</param>
		/// <returns>Collection of fixture function names</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Create the return collection
			List<string> values = new List<string>();

			// Cast the context to a fixture function expando object
			FixtureFunctionExpando fixtureFunction = (FixtureFunctionExpando)context.Instance;

			// Add the index names to the return collection
			values.AddRange(fixtureFunction.IndexData.Select(idx => idx.Name));
									
			return new TypeConverter.StandardValuesCollection(values.ToArray());
		}

		#endregion
	}
}

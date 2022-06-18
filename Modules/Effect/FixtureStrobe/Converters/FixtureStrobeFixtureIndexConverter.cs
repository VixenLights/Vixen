using System.ComponentModel;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.FixtureStrobe
{
	/// <summary>
	/// Provides the fixture strobe index names associated with a target node.
	/// </summary>
	public class FixtureStrobeFixtureIndexCollectionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available fixture strobe index value names.
		/// </summary>
		/// <param name="context">Fixture strobe effect instance</param>
		/// <returns>Collection of available fixture strobe index values</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to the fixture strobe effect module
			FixtureStrobeModule fixtureStrobeEffect = (FixtureStrobeModule)context.Instance;
			
			// Return the collection of fixture strobe index names
			return new TypeConverter.StandardValuesCollection(fixtureStrobeEffect.IndexValues.ToArray());
		}

		#endregion
	}
}

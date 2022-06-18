using System.ComponentModel;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Gobo
{
	/// <summary>
	/// Provides the gobo value names associated with a target node.
	/// </summary>
	public class GoboFixtureIndexCollectionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available gobo index value names.
		/// </summary>
		/// <param name="context">Gobo effect instance</param>
		/// <returns>Collection of available gobo index values</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to a Gobo effect module
			GoboModule theGoboEffect = (GoboModule)context.Instance;
			
			// Return the collection of gobo index names
			return new TypeConverter.StandardValuesCollection(theGoboEffect.IndexValues.ToArray());
		}

		#endregion
	}
}

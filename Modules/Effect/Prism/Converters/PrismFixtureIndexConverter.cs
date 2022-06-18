using System.ComponentModel;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Prism
{
	/// <summary>
	/// Provides the prism value names associated with a target node.
	/// </summary>
	public class PrismFixtureIndexCollectionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available prism index value names.
		/// </summary>
		/// <param name="context">Prism effect instance</param>
		/// <returns>Collection of available prism index values</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to the prism effect module
			PrismModule prismEffect = (PrismModule)context.Instance;
			
			// Return the collection of prism index names
			return new TypeConverter.StandardValuesCollection(prismEffect.IndexValues.ToArray());
		}

		#endregion
	}
}

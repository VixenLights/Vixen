using System.ComponentModel;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.SpinColorWheel
{
	/// <summary>
	/// Provides the spin color wheel index value names associated with a target node.
	/// </summary>
	public class SpinColorWheelFixtureIndexCollectionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available spin color wheel index value names.
		/// </summary>
		/// <param name="context">Spin Color Wheel effect instance</param>
		/// <returns>Collection of available Spin Color Wheel index values</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to the Spin Color Wheel effect module
			SpinColorWheelModule effect = (SpinColorWheelModule)context.Instance;
						
			// Return the collection of Spin Color Wheel index names
			return new TypeConverter.StandardValuesCollection(effect.IndexValues.ToArray());
		}

		#endregion
	}
}

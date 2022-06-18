using System.ComponentModel;
using Vixen.Data.Value;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.SpinColorWheel
{
	/// <summary>
	/// Provides the Spin Color Wheel function names associated with a target node.
	/// </summary>
	public class SpinColorWheelFunctionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available color wheel function names.
		/// </summary>
		/// <param name="context">Spin Color wheel effect instance</param>
		/// <returns>Collection of available spin color wheel function names</returns>		
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to a Spin Color Wheel effect module
			SpinColorWheelModule effect = (SpinColorWheelModule)context.Instance;

			// Return the Spin Color Wheel function names associated with effect 
			return new TypeConverter.StandardValuesCollection(effect.GetMatchingFunctionNames(FunctionIdentity.SpinColorWheel).ToArray());
		}

		#endregion
	}
}

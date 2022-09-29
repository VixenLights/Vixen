using System.Collections.Generic;
using Vixen.Data.Value;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.SpinColorWheel
{
	/// <summary>
	/// Provides the Spin Color Wheel function names associated with the target node(s).
	/// </summary>
	public class SpinColorWheelFunctionNameConverter : EffectListTypeConverterBase<SpinColorWheelModule>
	{
		#region Protected Methods

		/// <summary>
		/// Retrieves the collection of available color wheel function names.
		/// </summary>
		/// <param name="context">Spin Color wheel effect instance</param>
		/// <returns>Collection of available spin color wheel function names</returns>		
		protected override List<string> GetStandardValuesInternal(SpinColorWheelModule effect)
		{
			// Return the Spin Color Wheel function names associated with effect 
			return effect.GetMatchingFunctionNames(FunctionIdentity.SpinColorWheel);
		}

		#endregion
	}
}

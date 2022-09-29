using System.Collections.Generic;
using Vixen.Data.Value;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Gobo
{
	/// <summary>
	/// Provides the gobo function names associated with the target node(s).
	/// </summary>
	public class GoboFunctionNameConverter : EffectListTypeConverterBase<GoboModule>
	{
		#region Protected Methods

		/// <summary>
		/// Retrieves the collection of available gobo function names.
		/// </summary>
		/// <param name="context">Gobo effect instance</param>
		/// <returns>Collection of available gobo function names</returns>		
		protected override List<string> GetStandardValuesInternal(GoboModule effect)
		{
			// Return the gobo function names associated with effect 
			return effect.GetMatchingFunctionNames(FunctionIdentity.Gobo);
		}

		#endregion
	}
}

using System.Collections.Generic;
using System.ComponentModel;
using Vixen.Data.Value;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Frost;

namespace VixenModules.Effect.Frost
{
	/// <summary>
	/// Provides the frost function names associated with target node(s).
	/// </summary>
	public class FrostFunctionNameConverter : EffectListTypeConverterBase<FrostModule>
	{
		#region Protected Methods

		/// <summary>
		/// Retrieves the collection of available frost function names.
		/// </summary>
		/// <param name="context">Frost effect instance</param>
		/// <returns>Collection of available frost function names</returns>		
		protected override List<string> GetStandardValuesInternal(FrostModule effect)
		{
			// Return the frost function names associated with effect 
			return effect.GetMatchingFunctionNames(FunctionIdentity.Frost);
		}

		#endregion
	}
}

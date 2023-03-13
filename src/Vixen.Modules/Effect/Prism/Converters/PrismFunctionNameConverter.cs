using Vixen.Data.Value;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Prism
{
	/// <summary>
	/// Provides the prism function names associated with the target node(s).
	/// </summary>
	public class PrismFunctionNameConverter : EffectListTypeConverterBase<PrismModule>
	{
		#region Protected Methods

		/// <summary>
		/// Retrieves the collection of available prism function names.
		/// </summary>
		/// <param name="context">Prism effect instance</param>
		/// <returns>Collection of available prism function names</returns>		
		protected override List<string> GetStandardValuesInternal(PrismModule effect)
		{
			// Return the prism function names associated with effect 
			return effect.GetMatchingFunctionNames(FunctionIdentity.Prism);
		}

		#endregion
	}
}

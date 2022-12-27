using Vixen.Data.Value;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.FixtureStrobe
{
	/// <summary>
	/// Provides the prism function names associated with the target node(s).
	/// </summary>
	public class FixtureStrobeFunctionNameConverter : EffectListTypeConverterBase<FixtureStrobeModule>
	{
		#region Protected Methods

		/// <summary>
		/// Retrieves the collection of available prism function names.
		/// </summary>
		/// <param name="context">Prism effect instance</param>
		/// <returns>Collection of available prism function names</returns>		
		protected override List<string> GetStandardValuesInternal(FixtureStrobeModule effect)
		{
			// Return the fixture strobe function names associated with effect 
			return effect.GetMatchingFunctionNames(FunctionIdentity.Shutter);
		}

		#endregion
	}
}

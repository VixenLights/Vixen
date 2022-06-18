using System.ComponentModel;
using Vixen.Data.Value;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.FixtureStrobe
{
	/// <summary>
	/// Provides the prism function names associated with a target node.
	/// </summary>
	public class FixtureStrobeFunctionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available prism function names.
		/// </summary>
		/// <param name="context">Prism effect instance</param>
		/// <returns>Collection of available prism function names</returns>		
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to a fixture strobe effect module
			FixtureStrobeModule fixtureStrobeEffect = (FixtureStrobeModule)context.Instance;

			// Return the fixture strobe function names associated with effect 
			return new TypeConverter.StandardValuesCollection(fixtureStrobeEffect.GetMatchingFunctionNames(FunctionIdentity.Shutter).ToArray());
		}

		#endregion
	}
}

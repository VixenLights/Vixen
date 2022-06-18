using System.ComponentModel;
using Vixen.Data.Value;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Frost;

namespace VixenModules.Effect.Frost
{
	/// <summary>
	/// Provides the frost function names associated with a target node.
	/// </summary>
	public class FrostFunctionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available frost function names.
		/// </summary>
		/// <param name="context">Frost effect instance</param>
		/// <returns>Collection of available frsot function names</returns>		
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to a Frost effect module
			FrostModule theFrostEffect = (FrostModule)context.Instance;

			// Return the frost function names associated with effect 
			return new TypeConverter.StandardValuesCollection(theFrostEffect.GetMatchingFunctionNames(FunctionIdentity.Frost).ToArray());
		}

		#endregion
	}
}

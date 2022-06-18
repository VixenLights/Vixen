using System.ComponentModel;
using Vixen.Data.Value;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Prism
{
	/// <summary>
	/// Provides the prism function names associated with a target node.
	/// </summary>
	public class PrismFunctionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available prism function names.
		/// </summary>
		/// <param name="context">Prism effect instance</param>
		/// <returns>Collection of available prism function names</returns>		
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to a Prism effect module
			PrismModule prismEffect = (PrismModule)context.Instance;

			// Return the prism function names associated with effect 
			return new TypeConverter.StandardValuesCollection(prismEffect.GetMatchingFunctionNames(FunctionIdentity.Prism).ToArray());
		}

		#endregion
	}
}

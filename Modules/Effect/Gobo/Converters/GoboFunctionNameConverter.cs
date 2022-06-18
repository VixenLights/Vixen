using System.ComponentModel;
using Vixen.Data.Value;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Gobo
{
	/// <summary>
	/// Provides the gobo function names associated with a target node.
	/// </summary>
	public class GoboFunctionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Retrieves the collection of available gobo function names.
		/// </summary>
		/// <param name="context">Gobo effect instance</param>
		/// <returns>Collection of available gobo function names</returns>		
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Convert the context to a Gobo effect module
			GoboModule theGoboEffect = (GoboModule)context.Instance;

			// Return the gobo function names associated with effect 
			return new TypeConverter.StandardValuesCollection(theGoboEffect.GetMatchingFunctionNames(FunctionIdentity.Gobo).ToArray());
		}

		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Vixen.Marks;
using Vixen.Module.Effect;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Fixture
{
	/// <summary>
	/// Provides the fixture function names associated with a target node(s).
	/// </summary>
	public class FunctionCollectionNameConverter : EffectListTypeConverterBase
	{
		#region Public Methods

		/// <summary>
		/// Gets a collection of fixture functions associated with the node(s).
		/// </summary>
		/// <param name="context">Effects associated with the request</param>
		/// <returns>Collection of fixture function names</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Create the return collection
			List<string> functionNames = new List<string>();

			// If the context is a single fixture effect then...
			if (context.Instance is FixtureModule)
			{
				// Cast the context to the fixture effect
				FixtureModule fixtureEffect = (FixtureModule)context.Instance;

				// Retrieve the functions from the fixture effect
				functionNames = fixtureEffect.GetFunctionNames();
			}
			// Otherwise the context is a collection of effects
			else if (context.Instance is IEffectModuleInstance[])
			{
				// Loop over the effects
				foreach(IEffectModuleInstance effect in ((IEffectModuleInstance[])context.Instance))
				{
					// If the effect is a fixture effect then...
					if (effect is FixtureModule)
					{
						// Cast the effect to the fixture effect
						FixtureModule fixtureEffect = (FixtureModule)effect;

						// Loop over the fixture functions supported by the effect
						foreach(string functionName in fixtureEffect.GetFunctionNames())
						{
							// If the function name is NOT already in the collection then...
							if (!functionNames.Contains(functionName))
							{
								// Add the function name to the collection
								functionNames.Add(functionName);
							}
						}
					}	
				}
			}
			
			return new TypeConverter.StandardValuesCollection(functionNames.ToArray());
		}

		#endregion
	}
}

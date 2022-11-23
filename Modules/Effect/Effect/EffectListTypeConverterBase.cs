using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Vixen.Module.Effect;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Base class for an effect converter that provides a list of available items.
	/// </summary>
	/// <typeparam name="TEffectModule">Type of effect module</typeparam>
	public abstract class EffectListTypeConverterBase<TEffectModule> : TypeConverter
		where TEffectModule : class
	{
		#region Public Methods

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return true;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return value?.ToString();
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
			Type destinationType)
		{
			return value.ToString();
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		protected abstract List<string> GetStandardValuesInternal(TEffectModule effectModule);


		/// <summary>
		/// Gets the standard values associated with the specified effect context.
		/// </summary>
		/// <param name="context">Single effect or a collection of effects</param>
		/// <returns>Collection of string names</returns>
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			// Create the return collection
			List<string> values = new List<string>();

			// If the context is a single effect module then...
			if (context.Instance is TEffectModule)
			{
				// Cast the context to the effect
				TEffectModule effectModule = (TEffectModule)context.Instance;

				// Retrieve the standard values from the effect
				values = GetStandardValuesInternal(effectModule);
			}
			// Otherwise the context is a collection of effects
			else if (context.Instance is IEffectModuleInstance[])
			{
				// Loop over the effects
				foreach (IEffectModuleInstance effect in ((IEffectModuleInstance[])context.Instance))
				{
					// If the effect is the effect of interest then...
					if (effect is TEffectModule)
					{
						// Cast the effect to the fixture effect
						TEffectModule effectModule = (TEffectModule)effect;

						// Loop over the fixture functions supported by the effect
						foreach (string value in GetStandardValuesInternal(effectModule))
						{
							// If the string value is NOT already in the collection then...
							if (!values.Contains(value))
							{
								// Add the string value to the collection
								values.Add(value);
							}
						}
					}
				}
			}

			return new TypeConverter.StandardValuesCollection(values.ToArray());

		}

		#endregion
	}
}

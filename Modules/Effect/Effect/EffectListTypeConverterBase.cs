using System;
using System.ComponentModel;
using System.Globalization;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Base class for an effect converter that provides a list of available items.
	/// </summary>
	public abstract class EffectListTypeConverterBase : TypeConverter
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

		#endregion
	}
}

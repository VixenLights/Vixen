using System;
using System.ComponentModel;
using System.Globalization;

namespace VixenModules.EffectEditor.TypeConverters
{
	/// <summary>
	/// Ensures Percent type values are in range.
	/// </summary>
	public class PercentTypeConverter:TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		// Overrides the ConvertFrom method of TypeConverter.
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				double d = Convert.ToDouble(value);
				if (d >= 0 && d <= 100) return d;
				return 100;
			}
			return base.ConvertFrom(context, culture, value);
		}
		// Overrides the ConvertTo method of TypeConverter.
		public override object ConvertTo(ITypeDescriptorContext context,
		   CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return value.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

	}
}

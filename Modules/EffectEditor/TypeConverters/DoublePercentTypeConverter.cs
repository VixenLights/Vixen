using System;
using System.CodeDom;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Schema;

namespace VixenModules.EffectEditor.TypeConverters
{
	/// <summary>
	/// Ensures Percent type values are in range.
	/// </summary>
	public class DoublePercentTypeConverter:TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(Double) || sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if(destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		// Overrides the ConvertFrom method of TypeConverter.
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is Double)
			{
				double d = (double) value;
				return d*100;
			}
			if(value is string)
			{
				double d = Convert.ToDouble(value);
				d = d/100;
				if (d < 0 || d > 1) return 1;
				return d;
			}
			
			return base.ConvertFrom(context, culture, value);
		}
		// Overrides the ConvertTo method of TypeConverter.
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value is double)
				{
					double d = (double)value;
					if (d < 0 || d > 1) return "100";
					return(d * 100).ToString(culture);	
				}
				
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

	}
}

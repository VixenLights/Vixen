using System;
using System.ComponentModel;
using System.Globalization;

namespace VixenModules.EffectEditor.TypeConverters
{
	public class LevelTypeConverter: TypeConverter
	{
		// Overrides the CanConvertFrom method of TypeConverter.
		// The ITypeDescriptorContext interface provides the context for the
		// conversion. Typically, this interface is used at design time to 
		// provide information about the design-time container.
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
		// Overrides the ConvertFrom method of TypeConverter.
		public override object ConvertFrom(ITypeDescriptorContext context,CultureInfo culture, object value)
		{
			if (value is string)
			{
				double d = Convert.ToDouble(value)/100;
				if (d >= 0 && d <= 1) return d;
				return 1;
			}
			return base.ConvertFrom(context, culture, value);
		}
		// Overrides the ConvertTo method of TypeConverter.
		public override object ConvertTo(ITypeDescriptorContext context,
		   CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				double d = (double) value*100;
				return d.ToString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			double d = Convert.ToDouble(value);
			if(d <= 1 || d >= 0 )
			return true;
			return base.IsValid(context, value);
		}
	}
}

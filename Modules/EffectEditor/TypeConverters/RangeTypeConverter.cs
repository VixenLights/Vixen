using System;
using System.ComponentModel;
using System.Globalization;

namespace VixenModules.EffectEditor.TypeConverters
{
	public class RangeTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string) || sourceType == typeof(Int16) || sourceType == typeof(Int32) || sourceType == typeof(double) || sourceType == typeof(float))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		// Overrides the ConvertFrom method of TypeConverter.
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string || value is Int16 || value is Int32 || value is Single || value is Double)
			{
				int intValue = Convert.ToInt32(value);
				//Do range checking
				if (context != null && context.PropertyDescriptor != null)
				{
					AttributeCollection attributes = context.PropertyDescriptor.Attributes;
					RangeAttribute attribute = (RangeAttribute) attributes[typeof (RangeAttribute)];
					if (attribute != null)
					{
						if (intValue >= attribute.Lower && intValue <= attribute.Upper)
						{
							return intValue;
						}

						return attribute.Upper;

					}
				}
				return intValue;
			}

			return base.ConvertFrom(context, culture, value);
		}

		//public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		//{
		//	if (destinationType == typeof(string))
		//	{
		//		return true;
		//	}
		//	return base.CanConvertTo(context, destinationType);
		//}

		//// Overrides the ConvertTo method of TypeConverter.
		//public override object ConvertTo(ITypeDescriptorContext context,
		//	CultureInfo culture, object value, Type destinationType)
		//{
		//	if (destinationType == typeof(string))
		//	{
		//		return value.ToString();
		//	}
		//	return base.ConvertTo(context, culture, value, destinationType);
		//}

	}
}

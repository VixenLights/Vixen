using System;
using System.ComponentModel;
using System.Globalization;
using Vixen.Attributes;

namespace Vixen.TypeConverters
{
	public class BooleanStringTypeConverter:TypeConverter
	{
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				if (context != null)
				{
					PropertyDescriptor propertyDescriptor = context.PropertyDescriptor;
					var attribute = propertyDescriptor.Attributes[typeof (BoolDescriptionAttribute)] as BoolDescriptionAttribute;
					if (attribute != null)
					{
						if (((string) value).Equals(attribute.TrueValue))
							return true;
						if (((string) value).Equals(attribute.FalseValue))
							return false;
						throw new Exception(String.Format("Values must be \"{0}\" or \"{1}\"", attribute.TrueValue, attribute.FalseValue));
					}
				}


			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (context != null)
				{
					PropertyDescriptor propertyDescriptor = context.PropertyDescriptor;
					var attribute = propertyDescriptor.Attributes[typeof(BoolDescriptionAttribute)] as BoolDescriptionAttribute;
					if (attribute != null)
					{
						return (((bool)value) ? attribute.TrueValue : attribute.FalseValue);	
					}
					
				}

				return value.ToString();
				
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (context != null)
			{
				PropertyDescriptor propertyDescriptor = context.PropertyDescriptor;
				var attribute = propertyDescriptor.Attributes[typeof(BoolDescriptionAttribute)] as BoolDescriptionAttribute;
				if (attribute != null)
				{
					string[] bools = { attribute.TrueValue, attribute.FalseValue };
					return new StandardValuesCollection(bools);
					
				}
					
			}
			
			return new StandardValuesCollection(new []{"True", "False" });
		}
	}
}

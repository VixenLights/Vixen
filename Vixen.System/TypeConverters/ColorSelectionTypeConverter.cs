using System;
using System.ComponentModel;
using System.Globalization;

namespace Vixen.TypeConverters
{
	public class ColorSelectionTypeConverter : TypeConverter
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
				if (((string)value).ToLower() == "static")
					return true;
				if (((string)value).ToLower() == "gradient")
					return false;
				throw new Exception("Values must be \"Static\" or \"Gradient\"");
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
				return (((bool)value) ? "Static" : "Gradient");
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			string[] bools = { "Static", "Gradient" };
			StandardValuesCollection svc = new StandardValuesCollection(bools);
			return svc;
		}
	}
}

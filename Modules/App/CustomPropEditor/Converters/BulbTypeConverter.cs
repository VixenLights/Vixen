using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace VixenModules.App.CustomPropEditor.Converters
{
	public class BulbTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}

			return false;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}

			return false;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return value.ToString();
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
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

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			List<string> values = new List<string>();
			values.Add(@"12mm Bullet");
			values.Add(@"12mm Square");
			values.Add(@"1x3 Rectangle");
			values.Add(@"PixaBulb");
			values.Add(@"Strip");
			values.Add(@"Generic Pixel");
			values.Add(@"M5");
			values.Add(@"M6");
			values.Add(@"C6");
			values.Add(@"C7");
			values.Add(@"C9");
			values.Add(@"G40");

			return new StandardValuesCollection(values.ToArray());
		}
	}
}

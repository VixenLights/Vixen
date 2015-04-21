using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.ValueTypes;

namespace VixenModules.EffectEditor.TypeConverters
{
	public class PercentageTypeConverter : TypeConverter
	{
		
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string) || sourceType == typeof(Int16) || sourceType == typeof(Int32) || sourceType == typeof(Single) || sourceType == typeof(Double))
			{
				return true;
			}

			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return true;
			}

			return base.CanConvertTo(context, destinationType);
		}
		
		// Overrides the ConvertFrom method of TypeConverter.
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			
			float f = Convert.ToSingle(value) / 100;
			if (f < 0 || f > 1)
			{
				f = 1;
			}
			return new Percentage(f);
			
			//return base.ConvertFrom(context, culture, value);
		}

		// Overrides the ConvertTo method of TypeConverter.
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value is Percentage)
				{
					Percentage p = (Percentage)value;
					return (p.Value * 100).ToString(culture);
				}

			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}

}

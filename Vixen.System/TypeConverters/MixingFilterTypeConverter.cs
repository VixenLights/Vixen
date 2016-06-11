using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Vixen.Module.MixingFilter;
using Vixen.Services;

namespace Vixen.TypeConverters
{
	public class MixingFilterTypeConverter : TypeConverter
	{
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
			var filter = ApplicationServices.GetModuleDescriptors<ILayerMixingFilterInstance>().FirstOrDefault(x => x.TypeName.Equals(value));
			if (filter != null)
			{
				return filter.TypeId;
			}
			return null;
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

		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			var descriptors = ApplicationServices.GetModuleDescriptors<ILayerMixingFilterInstance>();
			List<string> values = new List<string>();
			for (int i = 0; i < descriptors.Length; i++)
			{
				values.Add(descriptors[i].TypeName);
			}

			return new StandardValuesCollection(values.ToArray());
		}
	}
}

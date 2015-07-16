using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Vixen.Attributes;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace Vixen.TypeConverters
{
	public class TargetElementDepthConverter : TypeConverter
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
			return Convert.ToInt32(value);
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
			int depth = 0;
			if (context != null)
			{
				IEffect effect = (IEffect) context.Instance;

				if (effect.TargetNodes.FirstOrDefault() != null)
				{
					//IEnumerable<ElementNode> leafs = effect.TargetNodes.SelectMany(x => x.GetLeafEnumerator());
					depth = effect.TargetNodes.FirstOrDefault().GetMaxChildDepth();
				}
			}

			PropertyDescriptor propertyDescriptor = context.PropertyDescriptor;
			var attribute = propertyDescriptor.Attributes[typeof (OffsetAttribute)] as OffsetAttribute;
			int offset = 0;
			if (attribute != null)
			{
				offset = attribute.Offset;
			}
			List<string> values = new List<string>();
			for (int i = 0; i < depth; i++)
			{
				values.Add((offset + i).ToString());
			}

			return new StandardValuesCollection(values.ToArray());
		}
	}
}
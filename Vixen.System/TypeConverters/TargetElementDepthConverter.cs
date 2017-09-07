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
				if (context.Instance is Array)
				{
					IEffect[] effects = context.Instance as IEffect[];

					if (effects != null)
					{
						int tempDepth = Int32.MaxValue;
						foreach (var effect in effects)
						{
							var node = effect.TargetNodes.FirstOrDefault();
							if (node != null)
							{
								tempDepth = Math.Min(node.GetMaxChildDepth(), tempDepth);
							}
						}
						if (tempDepth < Int32.MaxValue)
						{
							depth = tempDepth;
						}
					}
					
				}
				else
				{
					IEffect effect = (IEffect)context.Instance;
					var node = effect.TargetNodes.FirstOrDefault();
					if (node != null)
					{
						depth = node.GetMaxChildDepth();
					}
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
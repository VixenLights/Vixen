﻿using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace VixenModules.App.ColorGradients
{
	public class GradientTypeConverter : TypeConverter
	{

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return false;
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");

			if (destinationType == typeof(InstanceDescriptor))
			{
				ConstructorInfo ci = typeof(ColorGradient).GetConstructor(new[] { typeof(ColorGradient) });
				ColorGradient gradient = (ColorGradient)value;
				return new InstanceDescriptor(ci, new object[] { gradient });
			}

			if (destinationType == typeof (string))
			{
				if (value is ColorGradient)
				{
					ColorGradient cg = (ColorGradient) value;
					if (cg.IsLibraryReference)
					{
						return string.Format("Library: {0}", cg.LibraryReferenceName);
					}
					return "";
				}

			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

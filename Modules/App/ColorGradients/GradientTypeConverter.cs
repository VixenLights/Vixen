using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

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
			if (value is ColorGradient)
			{
				ColorGradient cg = (ColorGradient) value;
				if (cg.IsLibraryReference)
				{
					return cg.LibraryReferenceName; 
				}
				return "Gradient";
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

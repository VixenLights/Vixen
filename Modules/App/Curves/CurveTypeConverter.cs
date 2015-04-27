using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace VixenModules.App.Curves
{
	public class CurveTypeConverter: TypeConverter
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
			if (destinationType == typeof (string))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");

			if (destinationType == typeof(InstanceDescriptor))
			{
				ConstructorInfo ci = typeof(Curve).GetConstructor(new []{typeof(Curve)});
				Curve curve = (Curve)value;
				return new InstanceDescriptor(ci, new object[] { curve });
			}

			if (destinationType == typeof (string))
			{
				if (value is Curve)
				{
					Curve cg = (Curve)value;
					if (cg.IsLibraryReference)
					{
						return cg.LibraryReferenceName;
					}
					return "Curve";
				}
			}
			

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}

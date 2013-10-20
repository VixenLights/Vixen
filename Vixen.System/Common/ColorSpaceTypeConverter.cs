using System.ComponentModel;
using System.Globalization;
using System;
using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace Common.Controls.ColorManagement.ColorModels
{
	/// <summary>
	/// type converter for xyz color
	/// </summary>
	public class XYZTypeConverter : TypeConverter
	{
		//conversion
		public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
		{
			return sourceType == typeof (string) ||
			       base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
		{
			return destinationType == typeof (string) ||
			       base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
		                                   object value)
		{
			if (value is string) {
				if (culture == null)
					culture = CultureInfo.CurrentCulture;
				//
				string text = (string) value;
				string[] parts = text.Split(culture.TextInfo.ListSeparator.ToCharArray());
				if (parts.Length != 3)
					throw new ArgumentException("string invalid");
				//
				TypeConverter conv =
					TypeDescriptor.GetConverter(typeof (double));
				return new XYZ((double) conv.ConvertFromString(context, culture, parts[0]),
				               (double) conv.ConvertFromString(context, culture, parts[1]),
				               (double) conv.ConvertFromString(context, culture, parts[2]));
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
		                                 object value, System.Type destinationType)
		{
			if (value is XYZ) {
				if (culture == null)
					culture = CultureInfo.CurrentCulture;

				XYZ xyz = (XYZ) value;
				if (destinationType == null)
					//exception
					throw new ArgumentNullException("destinationType");
				else if (destinationType == typeof (String)) {
					//to string
					TypeConverter conv =
						TypeDescriptor.GetConverter(typeof (double));
					return string.Join(string.Format("{0} ",culture.TextInfo.ListSeparator),
					                   new string[]
					                   	{
					                   		conv.ConvertToString(context, culture, xyz.X),
					                   		conv.ConvertToString(context, culture, xyz.Y),
					                   		conv.ConvertToString(context, culture, xyz.Z)
					                   	});
				}
				else if (destinationType == typeof (InstanceDescriptor)) {
					//new instance
					ConstructorInfo ctor = typeof (XYZ).GetConstructor(new Type[]
					                                                   	{
					                                                   		typeof (double), typeof (double), typeof (double)
					                                                   	});
					if (ctor != null)
						return new InstanceDescriptor(ctor,
						                              new object[] {xyz.X, xyz.Y, xyz.Z});
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		//get properties
		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
		                                                           System.Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof (XYZ)).Sort();
		}

		//create instance
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override object CreateInstance(ITypeDescriptorContext context, System.Collections.IDictionary propertyValues)
		{
			if (propertyValues == null)
				throw new ArgumentNullException("propertyValues");
			//
			object x = propertyValues["X"],
			       y = propertyValues["Y"],
			       z = propertyValues["Z"];
			if (x == null || !(x is double) ||
			    y == null || !(y is double) ||
			    z == null || !(z is double))
				throw new ArgumentException("properties invalid");
			//
			return new XYZ((double) x, (double) y, (double) z);
		}
	}
}
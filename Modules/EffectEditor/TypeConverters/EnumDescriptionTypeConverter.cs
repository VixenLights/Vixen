using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace VixenModules.EffectEditor.TypeConverters
{
	public class EnumDescriptionTypeConverter : EnumConverter
	{
		public EnumDescriptionTypeConverter(Type type)
			: base(type)
		{
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || TypeDescriptor.GetConverter(typeof(Enum)).CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
				return GetEnumValue(EnumType, (string)value);
			if (value is Enum)
				return GetEnumDescription((Enum)value);
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			return value is Enum && destinationType == typeof(string)
				? GetEnumDescription((Enum)value)
				: (value is string && destinationType == typeof(string)
				  ? GetEnumDescription(EnumType, (string)value)
				  : base.ConvertTo(context, culture, value, destinationType));
		}

		public static string GetEnumDescription(Enum value)
		{
			var fieldInfo = value.GetType().GetField(value.ToString());
			var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
		}

		public static string GetEnumDescription(Type value, string name)
		{
			var fieldInfo = value.GetField(name);
			var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return (attributes.Length > 0) ? attributes[0].Description : name;
		}

		public static object GetEnumValue(Type value, string description)
		{
			var fields = value.GetFields();
			foreach (var fieldInfo in fields)
			{
				var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (attributes.Length > 0 && attributes[0].Description == description)
					return fieldInfo.GetValue(fieldInfo.Name);
				if (fieldInfo.Name == description)
					return fieldInfo.GetValue(fieldInfo.Name);
			}
			return description;
		}


	}
}

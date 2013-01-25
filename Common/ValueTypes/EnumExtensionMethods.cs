using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace Common.ValueTypes
{
	public static class EnumExtensionMethods
	{
		public static string GetDescription(this Enum value)
		{
			string description = value.ToString();
			FieldInfo fieldInfo = value.GetType().GetField(description);
			DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (attributes != null && attributes.Length > 0)
				description = attributes[0].Description;
			return description;
		}

		public static IList<KeyValuePair<Enum, string>> ToKeyValuePairs(this Type enumType)
		{
			return (from Enum v in Enum.GetValues(enumType)
					select new KeyValuePair<Enum, string>(v, v.GetDescription())).ToList();
		}
	}
}

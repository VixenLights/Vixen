using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace Common.WPFCommon.Converters
{
	public class EnumValueTypeConverter : IValueConverter
	{
		public static string GetDescription(Enum value)
		{
			if (value == null) return string.Empty;
			var field = value.GetType().GetField(value.ToString());
			var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
			return attribute?.Description ?? value.ToString();
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return string.Empty;

			// Get the description attribute
			FieldInfo fi = value.GetType().GetField(value.ToString());
			if (fi != null)
			{
				var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
				return attributes.Length > 0 ? attributes[0].Description : value.ToString();
			}

			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// We don't usually need to convert back if using an ItemTemplate
			return Binding.DoNothing;
		}
	}
}
using System.Diagnostics;
using System.Globalization;
using Catel.MVVM.Converters;
using Vixen.Sys.Props;

namespace Common.WPFCommon.Converters
{
	public class EnumValueTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter is Type { IsEnum: true } t)
			{
				if (value is Enum enumValue)
				{
					var name = value.ToString() ?? String.Empty;
					return Enum.Parse(t, name);
				}
			}

			throw new ArgumentException("Invalid Enum conversion");
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (targetType.IsEnum && value is Enum enumValue)
			{
				var name = value.ToString() ?? String.Empty;
				return Enum.Parse(targetType, name);
			}

			throw new ArgumentException("Invalid Enum conversion");
		}
	}
}
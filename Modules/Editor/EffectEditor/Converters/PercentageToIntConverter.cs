using System;
using System.Globalization;
using System.Windows.Data;
using Common.ValueTypes;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class PercentageToIntConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Percentage)
			{
				Percentage p = (Percentage)value;
				return System.Convert.ToInt32(p.Value * 100);
			}
			return System.Convert.ToInt32(value) * 100;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			float f = System.Convert.ToSingle(value) / 100;
			if (f < 0 || f > 1)
			{
				f = 1;
			}
			return new Percentage(f);
		}
	}
}

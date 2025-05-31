using System.Globalization;
using System.Windows.Data;

namespace VixenApplication.SetupDisplay.Converter
{
    public class TranslateTransformConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2)
            {
                return Math.Round( (double)values[1] * (double)values[0]);
            }

            return 1d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
﻿using System.Windows.Data;

namespace Common.WPFCommon.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool) && targetType != typeof(bool?))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool) && targetType != typeof(bool?))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        #endregion
    }
}

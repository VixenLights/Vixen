using System;
using System.Globalization;
using Catel.MVVM.Converters;

namespace Common.WPFCommon.Converters
{
	public class NullConverter<T>:IValueConverter
	{
		public NullConverter(T nullValue, T notNullValue)
		{
			NotNullValue = notNullValue;
			NullValue = nullValue;
		}

		public T NotNullValue { get; }

		public T NullValue { get; }

		#region Implementation of IValueConverter

		/// <inheritdoc />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value == null ? NullValue : NotNullValue;
		}

		/// <inheritdoc />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

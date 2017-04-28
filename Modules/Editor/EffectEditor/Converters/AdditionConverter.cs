using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class AdditionConverter : MarkupExtension, IValueConverter
	{
		private static AdditionConverter _instance;

		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var newValue = System.Convert.ToDouble(value) + System.Convert.ToDouble(parameter);
			if (newValue < 0) newValue = 0.0;
			return newValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _instance ?? (_instance = new AdditionConverter());
		}
	}
}

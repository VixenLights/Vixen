using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Ink;

namespace VixenModules.Preview.DisplayPreview.WPF
{
	[ValueConversion(typeof (Brush), typeof (DrawingAttributes))]
	public class BrushToDrawingAttributesConverter : IValueConverter
	{
		#region IValueConverter Members

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Brush colorBrush = (Brush) value;
			SolidColorBrush newBrush = (SolidColorBrush) colorBrush;
			DrawingAttributes attr = new DrawingAttributes()
			                         	{
			                         		Color = newBrush.Color
			                         	};
			return attr;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
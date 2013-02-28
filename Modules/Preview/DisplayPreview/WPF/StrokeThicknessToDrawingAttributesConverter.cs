using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Ink;
using System.Windows.Data;
using System.Windows.Media;

namespace VixenModules.Preview.DisplayPreview.WPF
{
	[ValueConversion(typeof(double), typeof(DrawingAttributes))]
	public class StrokeThicknessToDrawingAttributesConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double thickness = (double)value;
			DrawingAttributes attr = new DrawingAttributes {Width = thickness, Height = thickness, FitToCurve = false, Color = Colors.White};
			return attr;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

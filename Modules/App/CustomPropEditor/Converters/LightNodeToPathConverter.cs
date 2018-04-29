using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Converters
{
	public class LightNodeToPathConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Light ln = value as Light;
			Geometry g = null;
			if (ln != null)
			{
				g = new EllipseGeometry(new Point(ln.X, ln.Y), ln.Size, ln.Size);
			}

			return g;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

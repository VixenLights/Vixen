using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class CurveIsLibraryToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Curve curve = value as Curve;
			if (curve != null)
			{
				return curve.IsLibraryReference ? Visibility.Hidden : Visibility.Visible;
			}
			else
			{
				var glp = value as GradientLevelPair;
				if (glp != null)
				{
					return glp.Curve.IsLibraryReference ? Visibility.Hidden : Visibility.Visible;
				}
			}

			return Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

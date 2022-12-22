﻿using System.Globalization;
using System.Windows;
using System.Windows.Data;
using VixenModules.App.ColorGradients;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class ColorGradientIsLibraryToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ColorGradient cg = value as ColorGradient;
			if (cg != null)
			{
				return cg.IsLibraryReference ? Visibility.Hidden : Visibility.Visible;
			}
			else
			{
				var glp = value as GradientLevelPair;
				if (glp != null)
				{
					return glp.ColorGradient.IsLibraryReference ? Visibility.Hidden : Visibility.Visible;
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

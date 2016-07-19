using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Vixen.Sys.LayerMixing;

namespace VixenModules.Editor.LayerEditor.Converters
{
	public class DefaultLayerVisibilityConverter:MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ILayer layer = value as ILayer;
			if (layer != null)
			{
				return layer.Type == LayerType.Default ? Visibility.Collapsed : Visibility.Visible;
			}

			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return this;
		}
	}
}

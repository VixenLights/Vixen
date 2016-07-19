using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Vixen.Module.MixingFilter;
using Vixen.Sys.LayerMixing;

namespace VixenModules.Editor.LayerEditor.Converters
{
	public class HasSetupVisibilityConverter:MarkupExtension, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ILayerMixingFilterInstance layerMixingFilterInstance = value as ILayerMixingFilterInstance;
			if (layerMixingFilterInstance !=null)
			{
				return layerMixingFilterInstance.HasSetup ? Visibility.Visible : Visibility.Collapsed;
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

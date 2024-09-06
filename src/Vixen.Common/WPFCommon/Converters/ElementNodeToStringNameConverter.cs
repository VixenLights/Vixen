using System.Globalization;
using System.Windows.Data;
using Vixen.Sys;

namespace Common.WPFCommon.Converters
{
    public class ElementNodeToStringNameConverter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	    {
			if (value is ElementNode node)
			{
				return node.Name;
			}

			return string.Empty;
		}

	    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	    {
		    throw new NotImplementedException();
	    }
    }
}

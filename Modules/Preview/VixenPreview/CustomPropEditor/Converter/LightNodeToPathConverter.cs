using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Model;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Converter
{
    public class LightNodeToPathConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LightNode ln = value as LightNode;
            Geometry g = null;
            if (ln != null)
            {
                g = new EllipseGeometry(ln.Center, ln.Size, ln.Size);
            }

            return g;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

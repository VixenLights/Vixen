namespace VixenModules.Preview.DisplayPreview.WPF
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using VixenModules.Preview.DisplayPreview.Model.Shapes;

    [ValueConversion(typeof(IShape), typeof(ShapeType))]
    public class ShapeToShapeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var shape = value as IShape;
            if (shape != null)
            {
                return shape.ShapeType;
            }

            return ShapeType.OutlinedCircle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is ShapeType
                && targetType == typeof(IShape))
            {
                var shapeType = (ShapeType)value;
                switch (shapeType)
                {
                    case ShapeType.OutlinedCircle:
                        return new OutlinedCircle();
                    case ShapeType.SolidCircle:
                        return new SolidCircle();
                    case ShapeType.Line:
                        return new Line();
                    case ShapeType.OutlinedTriangle:
                        return new OutlinedTriangle();
                    case ShapeType.SolidTriangle:
                        return new SolidTriangle();
                    case ShapeType.OutlinedRectangle:
                        return new OutlinedRectangle();
                    case ShapeType.SolidRectangle:
                        return new SolidRectangle();
                    case ShapeType.OutlinedStar:
                        return new OutlinedStar();
                    case ShapeType.SolidStar:
                        return new SolidStar();
                    case ShapeType.Arc:
                        return new Arc();
					case ShapeType.UserDefinedShape:
						return new UserDefinedShape();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
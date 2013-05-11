namespace VixenModules.Preview.DisplayPreview
{
    using System.Windows.Media;

    public static class ColorManager
    {
        static ColorManager()
        {
            var brush = new LinearGradientBrush();
            brush.GradientStops.Add(new GradientStop(Colors.Red, 0));
            brush.GradientStops.Add(new GradientStop(Colors.Green, .5));
            brush.GradientStops.Add(new GradientStop(Colors.Blue, 1));
            brush.Freeze();
            RgbBrush = brush;
        }

        public static Brush RgbBrush { get; private set; }

        public static Brush AsBrush(this Color color)
        {
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }
    }
}
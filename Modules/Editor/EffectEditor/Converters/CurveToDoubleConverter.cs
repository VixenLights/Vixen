using System;
using System.Globalization;
using System.Windows.Data;
using VixenModules.App.Curves;
using ZedGraph;

namespace VixenModules.Editor.EffectEditor.Converters
{
	public class CurveToDoubleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var curve = value as Curve;
			if (curve != null)
			{
				return curve.GetValue(0);
			}
			return 0.0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var curveValue = System.Convert.ToDouble(value);
			return new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { curveValue, curveValue })); ;
		}
	}
}

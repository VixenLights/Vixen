using System.Globalization;
using System.Xml.Linq;
using NLog;

namespace VixenModules.App.CustomPropEditor.Import.XLights
{
	internal readonly record struct XModelCoordinateScale(double X, double Y)
	{
		public static int GetDefaultScale(int x, int y)
		{
			if (x < 100 && y < 100)
			{
				return 4;
			}

			if (x < 200 && y < 200)
			{
				return 2;
			}

			return 1;
		}

		public static XModelCoordinateScale FromModelElement(
			XElement modelElement,
			double defaultScale,
			Logger logging)
		{
			return new XModelCoordinateScale(
				GetScale(modelElement, "ScaleX", defaultScale, logging),
				GetScale(modelElement, "ScaleY", defaultScale, logging));
		}

		public int ApplyX(double x)
		{
			return Apply(x, X);
		}

		public int ApplyY(double y)
		{
			return Apply(y, Y);
		}

		private static int Apply(double value, double scale)
		{
			return (int)Math.Round(value * scale, MidpointRounding.AwayFromZero);
		}

		private static double GetScale(
			XElement modelElement,
			string attributeName,
			double defaultScale,
			Logger logging)
		{
			var scaleValue = XModelElementMetadata.GetAttributeValue(modelElement, attributeName);
			if (double.TryParse(scaleValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var scale) && scale > 0)
			{
				return scale;
			}

			if (!string.IsNullOrWhiteSpace(scaleValue))
			{
				logging.Warn(
					"xModel {ModelName} has invalid {ScaleAttribute} value {Scale}; using {DefaultScale}.",
					XModelElementMetadata.GetAttributeValue(modelElement, "name"),
					attributeName,
					scaleValue,
					defaultScale);
			}

			return defaultScale;
		}
	}
}

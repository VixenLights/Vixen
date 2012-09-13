using Vixen.Data.Value;

namespace VixenModules.OutputFilter.Color.Filter {
	class YellowFilter : ColorComponentFilter {
		override public string FilterName {
			get { return "Yellow"; }
		}

		protected override ColorValue FilterColorValue(ColorValue colorValue) {
			int yellow = _GetYellowValue(colorValue.Color);
			colorValue.Color = _CreateYellowColor(yellow);
			return colorValue;
		}

		protected override LightingValue FilterLightingValue(LightingValue lightingValue) {
			int yellow = _GetYellowValue(lightingValue.Color);
			yellow = (int)(yellow * lightingValue.Intensity);
			lightingValue.Color = _CreateYellowColor(yellow);
			return lightingValue;
		}

		private int _GetYellowValue(System.Drawing.Color color) {
			return (color.R + color.G) / 2;
		}

		private System.Drawing.Color _CreateYellowColor(int yellowValue) {
			return System.Drawing.Color.FromArgb(yellowValue, yellowValue, 0);
		}
	}
}

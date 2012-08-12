using Vixen.Data.Value;

namespace VixenModules.OutputFilter.Color.Filter {
	class GreenFilter : ColorComponentFilter {
		override public string FilterName {
			get { return "Green"; }
		}

		protected override ColorValue FilterColorValue(ColorValue colorValue) {
			int green = _GetGreenValue(colorValue.Color);
			colorValue.Color = _CreateGreenColor(green);
			return colorValue;
		}

		protected override LightingValue FilterLightingValue(LightingValue lightingValue) {
			int green = _GetGreenValue(lightingValue.Color);
			green = (int)(green * lightingValue.Intensity);
			lightingValue.Color = _CreateGreenColor(green);
			return lightingValue;
		}

		private int _GetGreenValue(System.Drawing.Color color) {
			return color.G;
		}

		private System.Drawing.Color _CreateGreenColor(int greenValue) {
			return System.Drawing.Color.FromArgb(0, greenValue, 0);
		}
	}
}

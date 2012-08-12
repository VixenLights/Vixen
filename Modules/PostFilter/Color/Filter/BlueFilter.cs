using Vixen.Data.Value;

namespace VixenModules.OutputFilter.Color.Filter {
	class BlueFilter : ColorComponentFilter {
		protected override ColorValue FilterColorValue(ColorValue colorValue) {
			int blue = _GetBlueValue(colorValue.Color);
			colorValue.Color = _CreateBlueColor(blue);
			return colorValue;
		}

		protected override LightingValue FilterLightingValue(LightingValue lightingValue) {
			int blue = _GetBlueValue(lightingValue.Color);
			blue = (int)(blue * lightingValue.Intensity);
			lightingValue.Color = _CreateBlueColor(blue);
			return lightingValue;
		}

		private int _GetBlueValue(System.Drawing.Color color) {
			return color.B;
		}

		private System.Drawing.Color _CreateBlueColor(int blueValue) {
			return System.Drawing.Color.FromArgb(0, 0, blueValue);
		}
	}
}

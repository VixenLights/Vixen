using Vixen.Data.Value;

namespace VixenModules.OutputFilter.Color.Filter {
	class WhiteFilter : ColorComponentFilter {
		override public string FilterName {
			get { return "White"; }
		}

		protected override ColorValue FilterColorValue(ColorValue colorValue) {
			int white = _GetWhiteValue(colorValue.Color);
			colorValue.Color = _CreateWhiteColor(white);
			return colorValue;
		}

		protected override LightingValue FilterLightingValue(LightingValue lightingValue) {
			int white = _GetWhiteValue(lightingValue.Color);
			white = (int)(white * lightingValue.Intensity);
			lightingValue.Color = _CreateWhiteColor(white);
			return lightingValue;
		}

		private int _GetWhiteValue(System.Drawing.Color color) {
			return (color.R + color.G + color.B) / 3;
		}

		private System.Drawing.Color _CreateWhiteColor(int whiteValue) {
			return System.Drawing.Color.FromArgb(whiteValue, whiteValue, whiteValue);
		}
	}
}

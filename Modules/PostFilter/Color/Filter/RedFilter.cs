using Vixen.Data.Value;

namespace VixenModules.OutputFilter.Color.Filter {
	class RedFilter : ColorComponentFilter {
		override public string FilterName {
			get { return "Red"; }
		}

		protected override ColorValue FilterColorValue(ColorValue colorValue) {
			int red = _GetRedValue(colorValue.Color);
			colorValue.Color = _CreateRedColor(red);
			return colorValue;
		}

		protected override LightingValue FilterLightingValue(LightingValue lightingValue) {
			int red = _GetRedValue(lightingValue.Color);
			red = (int)(red * lightingValue.Intensity);
			lightingValue.Color = _CreateRedColor(red);
			return lightingValue;
		}

		private int _GetRedValue(System.Drawing.Color color) {
			return color.R;
		}

		private System.Drawing.Color _CreateRedColor(int redValue) {
			return System.Drawing.Color.FromArgb(redValue, 0, 0);
		}
	}
}

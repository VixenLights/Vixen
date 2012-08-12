using Vixen.Data.Value;

namespace VixenModules.OutputFilter.Color.Filter {
	class NoFilter : ColorComponentFilter {
		protected override ColorValue FilterColorValue(ColorValue colorValue) {
			return colorValue;
		}

		protected override LightingValue FilterLightingValue(LightingValue lightingValue) {
			return lightingValue;
		}
	}
}

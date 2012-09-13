using Vixen.Data.Value;

namespace VixenModules.OutputFilter.Color.Filter {
	class NoFilter : ColorComponentFilter {
		override public string FilterName {
			get { return "Unfiltered"; }
		}

		protected override ColorValue FilterColorValue(ColorValue colorValue) {
			return colorValue;
		}

		protected override LightingValue FilterLightingValue(LightingValue lightingValue) {
			return lightingValue;
		}
	}
}

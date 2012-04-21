using System.Drawing;

namespace Vixen.Intent {
	struct LightingValue {
		public LightingValue(Color color, double intensity) {
			Color = color;
			Intensity = intensity;
		}

		public Color Color;
		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public double Intensity;
	}
}

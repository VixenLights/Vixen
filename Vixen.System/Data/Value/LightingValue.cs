using System.Drawing;

namespace Vixen.Data.Value {
	public struct LightingValue : IIntentDataType {
		public LightingValue(Color color, float intensity) {
			Color = color;
			Intensity = intensity;
		}

		public Color Color;

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		public float Intensity;

		public Color GetIntensityAffectedColor() {
			// as this is a lighting value, the lower the intensity (brightness), the more transparent it should be.
			return Color.FromArgb((int)(Intensity * byte.MaxValue), (int)(Color.R * Intensity), (int)(Color.G * Intensity), (int)(Color.B * Intensity));
		}
	}
}

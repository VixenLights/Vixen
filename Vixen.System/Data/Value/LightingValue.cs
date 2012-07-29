using System.Drawing;
using Vixen.Sys;

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
			return Color.FromArgb(0xff, (int)(Color.R * Intensity), (int)(Color.G * Intensity), (int)(Color.B * Intensity));
		}
	}
}

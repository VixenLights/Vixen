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

		/// <summary>
		/// Gets the lighting value as a color with the intensity value applied. Results in an opaque color,
		/// between black (0,0,0) for a lighting value with an intensity of 0 and the solid color with an intensity of 100%.
		/// </summary>
		public Color GetOpaqueIntensityAffectedColor() {
			return Color.FromArgb((int)(Color.R * Intensity), (int)(Color.G * Intensity), (int)(Color.B * Intensity));
		}

		/// <summary>
		/// Gets the lighting value as a color with the intensity value applied. Results in a color of variable transparancy:
		/// the intensity value is mapped to the alpha channel of the resulting color.
		/// </summary>
		public Color GetAlphaChannelIntensityAffectedColor()
		{
			// as this is a lighting value, the lower the intensity (brightness), the more transparent it should be.
			return Color.FromArgb((int)(Intensity * byte.MaxValue), Color.R, Color.G, Color.B);
		}
	}
}

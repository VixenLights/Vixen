using System.Drawing;
using System.Runtime.Serialization;

namespace Vixen.Data.Value {
	[DataContract]
	public struct LightingValue : IIntentDataType {
		public LightingValue(Color color, float intensity) {
			Color = color;
			Intensity = intensity;
		}

		[DataMember]
		public Color Color;

		/// <summary>
		/// Percentage value between 0 and 1.
		/// </summary>
		[DataMember]
		public float Intensity;

		public Color GetIntensityAffectedColor() {
			return Color.FromArgb(0xff, (int)(Color.R * Intensity), (int)(Color.G * Intensity), (int)(Color.B * Intensity));
		}
	}
}

using System.Drawing;
using Vixen.Module.PreFilter;

namespace FadeOut {
	public class FadeOutModule : PreFilterModuleInstanceBase {
		public override float Affect(float value, float percentIntoFilter) {
			return value - value * percentIntoFilter;
		}

		public override Color Affect(Color value, float percentIntoFilter) {
			float percentOn = 1f - percentIntoFilter;
			return Color.FromArgb((int)(value.R * percentOn), (int)(value.G * percentOn), (int)(value.B * percentOn));
		}

		public override double Affect(double value, float percentIntoFilter) {
			return value - value * percentIntoFilter;
		}

		public override long Affect(long value, float percentIntoFilter) {
			return (long)(value - value * percentIntoFilter);
		}
	}
}

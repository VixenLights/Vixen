using System;
using System.Drawing;
using Vixen.Sys;

namespace FadeOut {
	class IntentHandler : Vixen.Sys.Dispatch.IntentSegmentDispatch {
		// StartTime to EndTime represent the time span within the filter's whole TimeSpan
		// that apply to the next operation.
		public TimeSpan TimeSpan;
		public TimeSpan StartTime;
		public TimeSpan EndTime;

		public override void Handle(IIntentSegment<double> obj) {
			obj.StartValue = _Reduce(obj.StartValue, _StartPercentIntoFilter());
			obj.EndValue = _Reduce(obj.EndValue, _EndPercentIntoFilter());
		}

		public override void Handle(IIntentSegment<float> obj) {
			obj.StartValue = _Reduce(obj.StartValue, _StartPercentIntoFilter());
			obj.EndValue = _Reduce(obj.EndValue, _EndPercentIntoFilter());
		}

		public override void Handle(IIntentSegment<long> obj) {
			obj.StartValue = _Reduce(obj.StartValue, _StartPercentIntoFilter());
			obj.EndValue = _Reduce(obj.EndValue, _EndPercentIntoFilter());
		}

		public override void Handle(IIntentSegment<Color> obj) {
			obj.StartValue = _Reduce(obj.StartValue, _StartPercentIntoFilter());
			obj.EndValue = _Reduce(obj.EndValue, _EndPercentIntoFilter());
		}

		public override void Handle(IIntentSegment<LightingValue> obj) {
			obj.StartValue = _Reduce(obj.StartValue, _StartPercentIntoFilter());
			obj.EndValue = _Reduce(obj.EndValue, _EndPercentIntoFilter());
		}

		private double _StartPercentIntoFilter() {
			return StartTime.TotalMilliseconds / TimeSpan.TotalMilliseconds;
		}

		private double _EndPercentIntoFilter() {
			return EndTime.TotalMilliseconds / TimeSpan.TotalMilliseconds;
		}

		private double _Reduce(double value, double reductionPercent) {
			return value - value * reductionPercent;
		}

		private float _Reduce(float value, double reductionPercent) {
			return value - (float)(value * reductionPercent);
		}

		private long _Reduce(long value, double reductionPercent) {
			return value - (long)(value * reductionPercent);
		}

		private Color _Reduce(Color value, double reductionPercent) {
			reductionPercent = 1d - reductionPercent;
			return Color.FromArgb((int)(value.R * reductionPercent), (int)(value.G * reductionPercent), (int)(value.B * reductionPercent));
		}

		private LightingValue _Reduce(LightingValue value, double reductionPercent) {
			return new LightingValue(
				value.Color,
				_Reduce(value.Intensity, reductionPercent));
		}
	}
}

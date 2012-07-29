using System;
using Vixen.Interpolator;

namespace Vixen.Sys {
	class LinearInterpolatedSegmentCreator<T> : ISegmentCreator<T> {
		private Interpolator<T> _interpolator; 

		public LinearInterpolatedSegmentCreator(Interpolator<T> interpolator) {
			if(interpolator == null) throw new ArgumentNullException("interpolator");

			_interpolator = interpolator;
		}

		public IIntentSegment<T> CreateSegment(T startValue, T endValue, TimeSpan timeSpan) {
			return new IntentSegment<T>(startValue, endValue, timeSpan, _interpolator);
		}
	}
}

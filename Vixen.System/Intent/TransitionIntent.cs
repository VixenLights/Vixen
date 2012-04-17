using System;
using Vixen.Interpolator;
using Vixen.Sys;

namespace Vixen.Intent {
	public abstract class TransitionIntent<T> : Dispatchable<TransitionIntent<T>>, IIntent<T>
		where T : struct {

		private Interpolator<T> _interpolator;

		protected TransitionIntent(T startValue, T endValue, TimeSpan timeSpan, Interpolator<T> interpolator) {
			StartValue = startValue;
			EndValue = endValue;
			TimeSpan = timeSpan;
			_interpolator = interpolator;
		}

		abstract public IIntentState CreateIntentState(TimeSpan intentRelativeTime);

		public T StartValue { get; private set; }

		public T EndValue { get; private set; }

		public TimeSpan TimeSpan { get; private set; }

		public T GetStateAt(TimeSpan timeOffset) {
			T value;
			_interpolator.Interpolate(timeOffset, TimeSpan, StartValue, EndValue, out value);
			return value;
		}

		public IntentNodeSegment CreateSegment(TimeSpan offset, TimeSpan timeSpan) {
			offset = offset > TimeSpan.Zero ? offset : TimeSpan.Zero;
			offset = offset < TimeSpan ? offset : TimeSpan;

			if(offset + timeSpan > TimeSpan) {
				timeSpan = TimeSpan - offset;
			}

			// Get this intent to create a segment of itself.
			TimeNode segmentTimeNode = new TimeNode(offset, timeSpan);
			T startValue = GetStateAt(segmentTimeNode.StartTime);
			T endValue = GetStateAt(segmentTimeNode.EndTime);
			var newIntent = _CreateSegment(startValue, endValue, timeSpan);
			return new IntentNodeSegment(newIntent, segmentTimeNode);
		}

		protected abstract TransitionIntent<T> _CreateSegment(T startValue, T endValue, TimeSpan timeSpan);
	}
}

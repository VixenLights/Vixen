using System;
using Vixen.Interpolator;

namespace Vixen.Sys {
	// Times are relative to the start of the owning Intent.
	class IntentSegment<T> : Dispatchable<IntentSegment<T>>, IIntentSegment<T> {
		private Interpolator<T> _interpolator;

		public IntentSegment(T startValue, T endValue, TimeSpan timeSpan, Interpolator<T> interpolator) {
			if(interpolator == null) throw new ArgumentNullException("interpolator");
			StartValue = startValue;
			EndValue = endValue;
			TimeSpan = timeSpan;
			_interpolator = interpolator;
		}

		public bool Filtered { get; set; }

		public T StartValue { get; set; }

		public T EndValue { get; set; }

		public TimeSpan TimeSpan { get; private set; }

		virtual public T GetStateAt(TimeSpan segmentRelativeTime) {
			T value;
			if(_interpolator.Interpolate(segmentRelativeTime, TimeSpan, StartValue, EndValue, out value)) {
				return value;
			}
			return default(T);
		}

		object IIntentSegment.StartValue {
			get { return StartValue; }
			set { StartValue = (T)value; }
		}

		object IIntentSegment.EndValue {
			get { return EndValue; }
			set { EndValue = (T)value; }
		}
		//public IntentNodeSegment[] SplitAt(TimeSpan segmentRelativeTime) {
		//    if(Intent != null && TimeNode.Intersects(segmentRelativeTime)) {
		//        IntentNodeSegment[] segments = new IntentNodeSegment[2];
		//        segments[0] = Intent.CreateSegment(StartTime, segmentRelativeTime - StartTime);
		//        segments[1] = Intent.CreateSegment(StartTime + segmentRelativeTime, TimeSpan - segmentRelativeTime);
		//        return segments;
		//    }
		//    // Can't split at this time point.
		//    return null;
		//}

		//public int CompareTo(IDataNode other) {
		//    return TimeNode.CompareTo(other.TimeNode);
		//}
	}
}

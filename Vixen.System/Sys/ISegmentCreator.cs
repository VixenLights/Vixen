using System;

namespace Vixen.Sys {
	interface ISegmentCreator<T> {
		IIntentSegment<T> CreateSegment(T startValue, T endValue, TimeSpan timeSpan);
	}
}

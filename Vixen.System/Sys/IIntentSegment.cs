using System;

namespace Vixen.Sys {
	public interface IIntentSegment {
		TimeSpan TimeSpan { get; }
	}

	interface IIntentSegment<T> : IIntentSegment {
		T StartValue { get; set; }
		T EndValue { get; set; }
		T GetStateAt(TimeSpan segmentRelativeTime);
	}
}

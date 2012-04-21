using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IIntent : IDispatchable {
		TimeSpan TimeSpan { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime);
		void SplitAt(TimeSpan intentRelativeTime);
		void SplitAt(IEnumerable<TimeSpan> intentRelativeTimes);
		//void SplitAt(params TimeSpan[] intentRelativeTimes);
		void SplitAt(ITimeNode intentRelativeTime);
		void ApplyFilter(IPreFilterNode preFilterNode, TimeSpan contextAbsoluteIntentStartTime);
	}
	public interface IIntent<out T> : IIntent {
		T GetStateAt(TimeSpan timeOffset);
	}
}

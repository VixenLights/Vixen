using System;

namespace Vixen.Sys {
	public interface IIntent : IDispatchable {
		TimeSpan TimeSpan { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime);
		IntentNodeSegment CreateSegment(TimeSpan offset, TimeSpan timeSpan); 
	}
	public interface IIntent<out T> : IIntent {
		T GetStateAt(TimeSpan timeOffset);
	}
}

using System;

namespace Vixen.Sys {
	public interface IIntent : IDispatchable {
		TimeSpan TimeSpan { get; }
		IIntentState CreateIntentState(TimeSpan intentRelativeTime);
	}
	public interface IIntent<out T> : IIntent {
		T GetCurrentState(TimeSpan timeOffset);
	}
}

using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IIntentState : IDispatchable {
		IIntent Intent { get; }
		TimeSpan RelativeTime { get; }
		IIntentState Clone();
		List<SubordinateIntentState> SubordinateIntentStates { get; }
		object GetValue();
	}

	public interface IIntentState<out T> : IIntentState {
		IIntent<T> Intent { get; }
		T GetValue();
	}
}

using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IIntentState : IDispatchable {
		TimeSpan RelativeTime { get; }
		IIntentState Clone();
		List<SubordinateIntentState> SubordinateIntentStates { get; }
		object GetValue();
	}

	public interface IIntentState<out T> : IIntentState {
		T GetValue();
	}
}

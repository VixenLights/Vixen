using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public interface IIntentState : IDispatchable {
		TimeSpan RelativeTime { get; }
		List<IFilterState> FilterStates { get; }
		IIntentState Clone();
	}

	interface IIntentState<out T> : IIntentState {
		T GetValue();
	}
}

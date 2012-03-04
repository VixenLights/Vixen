using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	abstract class StateAggregator<V> : IStateSource<V> {
		//private IEnumerable<IStateSource<V>> _inputStateSources;

		//protected StateAggregator() {
		//}

		//protected StateAggregator(IEnumerable<IStateSource<V>> inputStateSources) {
		//    _inputStateSources = inputStateSources;
		//}

		//public void Aggregate() {
		//    Aggregate(_inputStateSources);
		//}

		public void Aggregate(IEnumerable<IStateSource<V>> inputStateSources) {
			if(inputStateSources == null) throw new ArgumentNullException("inputStateSources");

			IStateSource<V>[] inputStateSourcesArray = inputStateSources.ToArray();
			if(inputStateSourcesArray.Length == 0) {
				State = default(V);
				return;
			}
			IStateSource<V> firstCommand = inputStateSourcesArray[0];
			if(inputStateSourcesArray.Length == 1) {
				State = _GetValueFromState(firstCommand);
				// This is part of two things: context and channel
				//Context: Getting the channel states set in the context; effect is being
				//aggregated into it.
				//Channel: Getting its state set from its context sources.
				return;
			}

			IStateSource<V> state = inputStateSourcesArray.Aggregate(_Combinator);
			State = _GetValueFromState(state);
		}

		public V State { get; private set; }

		abstract protected IStateSource<V> _Combinator(IStateSource<V> left, IStateSource<V> right);

		private V _GetValueFromState(IStateSource<V> state) {
			return (state != null) ? state.State : default(V);
		}
	}
}

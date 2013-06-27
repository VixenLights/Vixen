using System;
using System.Collections.Generic;

namespace Vixen.Sys
{
	//A collection of element sources, one source per element.
	internal class ElementStateSourceCollection : IStateSourceCollection<Guid, IIntentStates>
	{
		private Dictionary<Guid, ElementStateSource> _states;

		public ElementStateSourceCollection()
		{
			_states = new Dictionary<Guid, ElementStateSource>();
		}

		public void SetValue(Guid key, IIntentStates value)
		{
			ElementStateSource state;
			if (!_states.TryGetValue(key, out state)) {
				state = new ElementStateSource();
				_states[key] = state;
			}

			state.State = value;
		}

		public IStateSource<IIntentStates> GetState(Guid key)
		{
			ElementStateSource state;
			_states.TryGetValue(key, out state);
			return state;
		}

		public IEnumerable<Guid> ElementsInCollection
		{
			get { return _states.Keys; }
		}
	}
}
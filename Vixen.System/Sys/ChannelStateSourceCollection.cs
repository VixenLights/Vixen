using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	//A collection of channel sources, one source per channel.
	internal class ChannelStateSourceCollection : IStateSourceCollection<Guid, IIntentStateList> {
		private Dictionary<Guid, ChannelStateSource> _states;

		public ChannelStateSourceCollection() {
			_states = new Dictionary<Guid, ChannelStateSource>();
		}

		public void SetValue(Guid key, IIntentStateList value) {
			ChannelStateSource state;
			if(!_states.TryGetValue(key, out state)) {
				state = new ChannelStateSource();
				_states[key] = state;
			}
			state.State = value;
		}

		public IStateSource<IIntentStateList> GetState(Guid key) {
			ChannelStateSource state;
			_states.TryGetValue(key, out state);
			return state;
		}

		public IEnumerable<Guid> ChannelsInCollection {
			get { return _states.Keys; }
		}
	}
}

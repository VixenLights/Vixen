using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	internal class ChannelContextSource : IStateSourceCollectionAdapter<Guid, IEnumerable<IIntentState>> {
		public ChannelContextSource(Guid channelId) {
			Key = channelId;
		}

		public Guid Key { get; set; }

		public IEnumerator<IStateSource<IEnumerable<IIntentState>>> GetEnumerator() {
			return VixenSystem.Contexts.Select(x => x.GetState(Key)).NotNull().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}

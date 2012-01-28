using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Sys {
	internal class ChannelContextSource : IStateSourceCollectionAdapter<Guid, Command> {
		public ChannelContextSource(Guid channelId) {
			Key = channelId;
		}

		public Guid Key { get; set; }

		public IEnumerator<IStateSource<Command>> GetEnumerator() {
			return VixenSystem.Contexts.Select(x => x.GetValue(Key)).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}

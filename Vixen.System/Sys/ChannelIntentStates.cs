using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public class ChannelIntentStates : Dictionary<Guid, IIntentStates> {
		public ChannelIntentStates() {
		}

		public ChannelIntentStates(IDictionary<Guid, IIntentStates> values)
			: base(values) {
		}
	}
}

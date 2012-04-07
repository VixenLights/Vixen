using System;
using System.Collections.Generic;

namespace Vixen.Commands {
	public class ChannelCommands : Dictionary<Guid,ICommand> {
		public ChannelCommands() {
		}

		public ChannelCommands(IDictionary<Guid, ICommand> values)
			: base(values) {
		}
	}
}

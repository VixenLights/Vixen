using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class TotalChannelCommands : CountValue {
		private Channel _channel;

		public TotalChannelCommands(Channel channel)
			: base("Total Channel Commands - " + channel.Name) {
			_channel = channel;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Instrumentation;

namespace Vixen.Sys.Instrumentation {
	class ChannelCommandsPerSecondValue : RateValue {
		private Channel _channel;

		public ChannelCommandsPerSecondValue(Channel channel)
			: base("Channel Commands Per Second - " + channel.Name) {
			_channel = channel;
		}
	}
}

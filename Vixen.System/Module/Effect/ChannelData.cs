using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	public class ChannelData : Dictionary<Guid, Command[]> {
		public ChannelData() {
		}

		public ChannelData(IDictionary<Guid, Command[]> data)
			: base(data) {
		}

		public void AddCommandsForChannel(Guid channel, Command[] commands)
		{
			if (ContainsKey(channel)) {
				this[channel] = this[channel].Concat(commands).ToArray();
			} else {
				this[channel] = commands;
			}
		}

		public void AddCommandForChannel(Guid channel, Command command)
		{
			AddCommandsForChannel(channel, new Command[] {command});
		}

		static public ChannelData Restrict(ChannelData channelData, TimeSpan startTime, TimeSpan endTime)
		{
			return new ChannelData(channelData.ToDictionary(
				x => x.Key,
				x => x.Value.Where(y => !(y.StartTime >= endTime || y.EndTime < startTime)).ToArray()));
		}
	}
}

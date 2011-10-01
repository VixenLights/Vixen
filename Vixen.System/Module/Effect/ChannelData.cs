using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;

namespace Vixen.Module.Effect {
	// Operates against CommandNodes because the effect needs to generate data that is
	// timed relative to the effect's start.
	public class ChannelData : Dictionary<Guid, CommandNode[]> {
		public ChannelData() {
		}

		public ChannelData(IDictionary<Guid, CommandNode[]> data)
			: base(data) {
		}

		public void AddCommandsForChannel(Guid channel, CommandNode[] commands)
		{
			if (ContainsKey(channel)) {
				this[channel] = this[channel].Concat(commands).ToArray();
			} else {
				this[channel] = commands;
			}
		}

		public void AddCommandForChannel(Guid channel, CommandNode command)
		{
			AddCommandsForChannel(channel, new CommandNode[] { command });
		}

		static public ChannelData Restrict(ChannelData channelData, TimeSpan startTime, TimeSpan endTime)
		{
			return new ChannelData(channelData.ToDictionary(
				x => x.Key,
				x => x.Value.Where(y => !(y.StartTime >= endTime || y.EndTime < startTime)).ToArray()));
		}
	}
}

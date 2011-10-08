using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;

namespace Vixen.Module.Effect
{
	public class ChannelCommands : Dictionary<Guid, Command[]>
	{
		public ChannelCommands()
		{
		}

		public ChannelCommands(IDictionary<Guid, Command[]> data)
			: base(data)
		{
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
			AddCommandsForChannel(channel, new Command[] { command });
		}
	}
}

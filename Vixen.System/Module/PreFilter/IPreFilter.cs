using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;
using Vixen.Sys;

namespace Vixen.Module.PreFilter {
	public interface IPreFilter {
		Command Affect(Command command, TimeSpan filterRelativeTime);
		TimeSpan TimeSpan { get; set; }
		ChannelNode[] TargetNodes { get; set; }
	}
}

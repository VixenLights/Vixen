using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Effect {
	public class ChannelData : Dictionary<Guid, CommandData[]> {
		public ChannelData() {
		}

		public ChannelData(IDictionary<Guid, CommandData[]> data)
			: base(data) {
		}

		static public ChannelData Restrict(ChannelData channelData, TimeSpan startTime, TimeSpan endTime) {
			return new ChannelData(channelData.ToDictionary(
				x => x.Key,
				x => x.Value.Where(y => !(y.StartTime >= endTime || y.EndTime < startTime)).ToArray()));
		}
	}
}

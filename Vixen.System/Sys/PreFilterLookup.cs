using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class PreFilterLookup : Dictionary<Guid, IPreFilterNode[]> {
		public void AddChannelFilters(Channel channel, IEnumerable<IPreFilterNode> preFilterNodes) {
			this[channel.Id] = preFilterNodes.ToArray();
		}

		public IEnumerable<IPreFilterNode> GetFiltersForChannel(Guid channelId, TimeSpan currentTime) {
			return _GetQualifiedFilters(channelId, x => _PreFilterQualifies(x, currentTime));
		}

		public IEnumerable<IPreFilterNode> GetFiltersForChannel(Guid channelId, ITimeNode timeNode) {
			return _GetQualifiedFilters(channelId, x => _PreFilterQualifies(x, timeNode));
		}

		private IEnumerable<IPreFilterNode> _GetQualifiedFilters(Guid channelId, Predicate<IPreFilterNode> filterQualifier) {
			IPreFilterNode[] preFilters;
			TryGetValue(channelId, out preFilters);
			if(preFilters != null) return preFilters.Where(x => filterQualifier(x));
			return Enumerable.Empty<IPreFilterNode>();
		}

		private bool _PreFilterQualifies(IPreFilterNode preFilterNode, TimeSpan currentTime) {
			return currentTime >= preFilterNode.StartTime && currentTime < preFilterNode.EndTime;
		}

		private bool _PreFilterQualifies(IPreFilterNode preFilterNode, ITimeNode timeNode) {
			return TimeNode.Intersects(timeNode, preFilterNode);
		}
	}
}

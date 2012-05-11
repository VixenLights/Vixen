using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class SequenceFilterLookup : Dictionary<Guid, ISequenceFilterNode[]> {
		public void AddChannelFilters(Channel channel, IEnumerable<ISequenceFilterNode> filterNodes) {
			this[channel.Id] = filterNodes.ToArray();
		}

		public IEnumerable<ISequenceFilterNode> GetFiltersForChannel(Guid channelId, TimeSpan currentTime) {
			return _GetQualifiedFilters(channelId, x => _FilterQualifies(x, currentTime));
		}

		public IEnumerable<ISequenceFilterNode> GetFiltersForChannel(Guid channelId, ITimeNode timeNode) {
			return _GetQualifiedFilters(channelId, x => _FilterQualifies(x, timeNode));
		}

		private IEnumerable<ISequenceFilterNode> _GetQualifiedFilters(Guid channelId, Predicate<ISequenceFilterNode> filterQualifier) {
			ISequenceFilterNode[] sequenceFilters;
			TryGetValue(channelId, out sequenceFilters);
			if(sequenceFilters != null) return sequenceFilters.Where(x => filterQualifier(x));
			return Enumerable.Empty<ISequenceFilterNode>();
		}

		private bool _FilterQualifies(ISequenceFilterNode sequenceFilterNode, TimeSpan currentTime) {
			return currentTime >= sequenceFilterNode.StartTime && currentTime < sequenceFilterNode.EndTime;
		}

		private bool _FilterQualifies(ISequenceFilterNode sequenceFilterNode, ITimeNode timeNode) {
			return TimeNode.Intersects(timeNode, sequenceFilterNode);
		}
	}
}

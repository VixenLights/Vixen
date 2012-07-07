using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Services {
	public class SequenceFilterService {
		static private SequenceFilterService _instance;

		private SequenceFilterService() {
		}

		public static SequenceFilterService Instance {
			get { return _instance ?? (_instance = new SequenceFilterService()); }
		}

		public SequenceFilterLookup BuildLookup(IEnumerable<ChannelNode> channelNodes, IEnumerable<ISequenceFilterNode> filterNodes) {
			SequenceFilterLookup lookup = new SequenceFilterLookup();

			Stack<IEnumerable<ISequenceFilterNode>> filterStack = new Stack<IEnumerable<ISequenceFilterNode>>();
			foreach(ChannelNode node in channelNodes) {
				_SearchBranchForFilters(node, filterNodes, filterStack, lookup);
			}

			return lookup;
		}

		private void _SearchBranchForFilters(ChannelNode node, IEnumerable<ISequenceFilterNode> filters, Stack<IEnumerable<ISequenceFilterNode>> filtersFound, SequenceFilterLookup lookup) {
			// Must push a single value for each level we enter.
			ISequenceFilterNode[] sequenceFilterNodes = _GetFiltersForNode(node, filters);
			if(sequenceFilterNodes.Length > 0) {
				filtersFound.Push(sequenceFilterNodes);
			} else {
				filtersFound.Push(null);
			}

			if(node.IsLeaf) {
				ISequenceFilterNode[] channelFilters = filtersFound.Where(x => x != null).Reverse().SelectMany(x => x).ToArray();
				if(channelFilters.Length > 0) {
					lookup.AddChannelFilters(node.Channel, channelFilters);
				}
			} else {
				foreach(ChannelNode childNode in node.Children) {
					_SearchBranchForFilters(childNode, filters, filtersFound, lookup);
				}
			}

			// Pop a single value for every level we exit.
			filtersFound.Pop();
		}

		private ISequenceFilterNode[] _GetFiltersForNode(ChannelNode node, IEnumerable<ISequenceFilterNode> filters) {
			return filters.Where(x => x.Filter.TargetNodes.Contains(node)).ToArray();
		}

	}
}

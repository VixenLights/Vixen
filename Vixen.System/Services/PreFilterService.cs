using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Services {
	public class PreFilterService {
		private PreFilterService() {
		}

		static public PreFilterLookup BuildPreFilterLookup(IEnumerable<ChannelNode> channelNodes, IEnumerable<IPreFilterNode> preFilterNodes) {
			PreFilterLookup lookup = new PreFilterLookup();

			Stack<IEnumerable<IPreFilterNode>> preFilterStack = new Stack<IEnumerable<IPreFilterNode>>();
			foreach(ChannelNode node in channelNodes) {
				_SearchBranchForFilters(node, preFilterNodes, preFilterStack, lookup);
			}

			return lookup;
		}

		//static public void FilterEffect(EffectNode effectNode, IEnumerable<IPreFilterNode> applicableFilters) {
		//    EffectIntents effectIntents = effectNode.Effect.Render();
		//    foreach(IPreFilterNode preFilterNode in applicableFilters) {
		//        effectIntents.ApplyFilter(preFilterNode, effectNode.StartTime);
		//    }
		//}

		static private void _SearchBranchForFilters(ChannelNode node, IEnumerable<IPreFilterNode> preFilters, Stack<IEnumerable<IPreFilterNode>> preFiltersFound, PreFilterLookup lookup) {
			// Must push a single value for each level we enter.
			IPreFilterNode[] preFilterNodes = _GetPreFiltersForNode(node, preFilters);
			if(preFilterNodes.Length > 0) {
				preFiltersFound.Push(preFilterNodes);
			} else {
				preFiltersFound.Push(null);
			}

			if(node.IsLeaf) {
				IPreFilterNode[] channelFilters = preFiltersFound.Where(x => x != null).Reverse().SelectMany(x => x).ToArray();
				if(channelFilters.Length > 0) {
					lookup.AddChannelFilters(node.Channel, channelFilters);
				}
			} else {
				foreach(ChannelNode childNode in node.Children) {
					_SearchBranchForFilters(childNode, preFilters, preFiltersFound, lookup);
				}
			}

			// Pop a single value for every level we exit.
			preFiltersFound.Pop();
		}

		static private IPreFilterNode[] _GetPreFiltersForNode(ChannelNode node, IEnumerable<IPreFilterNode> preFilters) {
			return preFilters.Where(x => x.PreFilter.TargetNodes.Contains(node)).ToArray();
		}

	}
}

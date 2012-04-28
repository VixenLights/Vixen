using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Services;
using Vixen.Sys;

namespace Vixen.Execution {
	class FilteringIntentCache : IEnumerable<IEffectNode> {
		private IEnumerable<IEffectNode> _effectNodes;
		private PreFilterLookup _preFilterLookup;
		private Dictionary<IEffectNode, EffectIntents> _effectIntentCache;

		public FilteringIntentCache() {
			_effectIntentCache = new Dictionary<IEffectNode, EffectIntents>();
		}

		public void Use(IEnumerable<IEffectNode> effectNodes, IEnumerable<IPreFilterNode> sequencePreFilters) {
			if(effectNodes == null) throw new ArgumentNullException("effectNodes");
			if(sequencePreFilters == null) throw new ArgumentNullException("sequencePreFilters");

			_effectNodes = effectNodes;
			_preFilterLookup = PreFilterService.BuildPreFilterLookup(VixenSystem.Nodes, sequencePreFilters);
		}

		// Needs to pass-through the IEnumerable in order to get back effect nodes to check against the cache.
		public IEnumerator<IEffectNode> GetEnumerator() {
			if(_effectNodes == null) throw new InvalidOperationException("No data has been provided.");
			if(VixenSystem.AllowFilterEvaluation && _preFilterLookup == null) throw new InvalidOperationException("Filtering is enabled, but no filters have been provided.");

			using(IEnumerator<IEffectNode> enumerator = _effectNodes.GetEnumerator()) {
				while(enumerator.MoveNext()) {
					IEffectNode effectNode = enumerator.Current;
					_PassThroughCache(effectNode);
					yield return effectNode;
				}
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		private void _PassThroughCache(IEffectNode effectNode) {
			if(_EffectNeedsToBeCached(effectNode)) {
				if(VixenSystem.AllowFilterEvaluation) {
					_FilterEffectIntents(effectNode);
				}
				_Add(effectNode);
			}
		}

		private bool _EffectNeedsToBeCached(IEffectNode effectNode) {
			return !_Contains(effectNode) || effectNode.Effect.IsDirty;
		}

		private bool _Contains(IEffectNode effectNode) {
			return _effectIntentCache.ContainsKey(effectNode);
		}

		private void _Add(IEffectNode effectNode) {
			_effectIntentCache[effectNode] = effectNode.Effect.Render();
		}

		private void _FilterEffectIntents(IEffectNode effectNode) {
			EffectIntents effectIntents = effectNode.Effect.Render();
			foreach(Guid channelId in effectIntents.ChannelIds) {
				_ApplyFiltersForChannelToIntents(effectIntents, channelId, effectNode.StartTime);
			}
		}

		private void _ApplyFiltersForChannelToIntents(EffectIntents effectIntents, Guid channelId, TimeSpan effectStartTime) {
			foreach(IntentNode intentNode in effectIntents.GetIntentNodesForChannel(channelId)) {
				IPreFilterNode[] channelFilters = _preFilterLookup.GetFiltersForChannel(channelId, intentNode).ToArray();
				foreach(var filter in channelFilters) {
					intentNode.ApplyFilter(filter, effectStartTime);
				}
			}
		}
	}
}

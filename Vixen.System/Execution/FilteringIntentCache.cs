using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;

namespace Vixen.Execution {
	class FilteringIntentCache : IDisposable, IEnumerable<IEffectNode> {
		private IEnumerable<IEffectNode> _effectNodes;
		private SequenceFilterLookup _filterLookup;
		private Dictionary<IEffectNode, EffectIntents> _effectIntentCache;
		private CacheHitPercentValue _cacheHitPercentValue;

		public FilteringIntentCache() {
			//*** Not actually doing anything.  Intents are filtered in-place in the effect, if need be.
			//    The dictionary is used to determine if it's been run through the filters or not, but
			//    the values stored in the dictionary aren't ever actually used.
			_effectIntentCache = new Dictionary<IEffectNode, EffectIntents>();
		}

		public void Use(IEnumerable<IEffectNode> effectNodes, IEnumerable<ISequenceFilterNode> filters, string contextName = null) {
			if(effectNodes == null) throw new ArgumentNullException("effectNodes");
			if(filters == null) throw new ArgumentNullException("filters");

			_effectNodes = effectNodes;
			_filterLookup = SequenceFilterService.BuildLookup(VixenSystem.Nodes, filters);
			_CreateInstrumentationValues(contextName);
		}

		// Needs to pass-through the IEnumerable in order to get back effect nodes to check against the cache.
		public IEnumerator<IEffectNode> GetEnumerator() {
			if(_effectNodes == null) throw new InvalidOperationException("No data has been provided.");
			if(VixenSystem.AllowFilterEvaluation && _filterLookup == null) throw new InvalidOperationException("Filtering is enabled, but no filters have been provided.");

			using(IEnumerator<IEffectNode> enumerator = _effectNodes.GetEnumerator()) {
				while(enumerator.MoveNext()) {
					IEffectNode effectNode = enumerator.Current;
					_PassThroughCache(effectNode);
					yield return effectNode;
				}
			}
		}

        ~FilteringIntentCache() {
            Dispose();
        }

		public void Dispose() {
			_RemoveInstrumentationValues();
			GC.SuppressFinalize(this);
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		private void _CreateInstrumentationValues(string contextName) {
			_RemoveInstrumentationValues();
			_cacheHitPercentValue = new CacheHitPercentValue(contextName);
			VixenSystem.Instrumentation.AddValue(_cacheHitPercentValue);
		}

		private void _RemoveInstrumentationValues() {
			if(_cacheHitPercentValue != null) {
				VixenSystem.Instrumentation.RemoveValue(_cacheHitPercentValue);
			}
		}

		private void _PassThroughCache(IEffectNode effectNode) {
			if(_EffectNeedsToBeCached(effectNode)) {
				_cacheHitPercentValue.IncrementUnqualifying();
				if(VixenSystem.AllowFilterEvaluation) {
					_FilterEffectIntents(effectNode);
				}
				_Add(effectNode);
			} else {
				_cacheHitPercentValue.IncrementQualifying();
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
				ISequenceFilterNode[] channelFilters = _filterLookup.GetFiltersForChannel(channelId, intentNode).ToArray();
				foreach(var filter in channelFilters) {
					intentNode.ApplyFilter(filter, effectStartTime);
				}
			}
		}
	}
}

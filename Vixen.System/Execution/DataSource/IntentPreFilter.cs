using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Instrumentation;

namespace Vixen.Execution.DataSource
{
	internal class IntentPreFilter : IDisposable, IEnumerable<IEffectNode>
	{
		private IEnumerable<IDataNode> _effectNodes;
		private SequenceFilterLookup _filterLookup;
		private HashSet<IEffectNode> _filteredIntents;
		private CacheHitPercentValue _cacheHitPercentValue;

		public IntentPreFilter()
		{
			_filteredIntents = new HashSet<IEffectNode>();
		}

		public IEnumerable<IDataNode> Data
		{
			set { _effectNodes = value; }
		}

		public IEnumerable<ISequenceFilterNode> Filters
		{
			set { _filterLookup = SequenceFilterService.Instance.BuildLookup(VixenSystem.Nodes, value); }
		}

		public string ContextName
		{
			set { _CreateInstrumentationValues(value); }
		}

		public void ClearCache()
		{
			_filteredIntents.Clear();
		}

		// Needs to pass-through the IEnumerable in order to get back effect nodes to check against the cache.
		public IEnumerator<IEffectNode> GetEnumerator()
		{
			if (_effectNodes == null) throw new InvalidOperationException("No data has been provided.");
			if (VixenSystem.AllowFilterEvaluation && _filterLookup == null)
				throw new InvalidOperationException("Filtering is enabled, but no filters have been provided.");

			using (IEnumerator<IDataNode> enumerator = _effectNodes.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					IEffectNode effectNode = (IEffectNode) enumerator.Current;
					_PreFilter(effectNode);
					yield return effectNode;
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_RemoveInstrumentationValues();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		private void _CreateInstrumentationValues(string contextName)
		{
			_RemoveInstrumentationValues();
			_cacheHitPercentValue = new CacheHitPercentValue(contextName);
			VixenSystem.Instrumentation.AddValue(_cacheHitPercentValue);
		}

		private void _RemoveInstrumentationValues()
		{
			if (_cacheHitPercentValue != null) {
				VixenSystem.Instrumentation.RemoveValue(_cacheHitPercentValue);
			}
		}

		private void _PreFilter(IEffectNode effectNode)
		{
			if (_EffectNeedsToBeFiltered(effectNode)) {
				_cacheHitPercentValue.IncrementUnqualifying();
				if (VixenSystem.AllowFilterEvaluation) {
					_FilterEffectIntents(effectNode);
				}
				_Add(effectNode);
			}
			else {
				_cacheHitPercentValue.IncrementQualifying();
			}
		}

		private bool _EffectNeedsToBeFiltered(IEffectNode effectNode)
		{
			return !_Contains(effectNode) || effectNode.Effect.IsDirty;
		}

		private bool _Contains(IEffectNode effectNode)
		{
			return _filteredIntents.Contains(effectNode);
		}

		private void _Add(IEffectNode effectNode)
		{
			effectNode.Effect.Render();
			_filteredIntents.Add(effectNode);
		}

		private void _FilterEffectIntents(IEffectNode effectNode)
		{
			EffectIntents effectIntents = effectNode.Effect.Render();
			effectIntents.ElementIds.AsParallel().ForAll(
				elementId => _ApplyFiltersForElementToIntents(effectIntents, elementId, effectNode.StartTime));
			//foreach(Guid elementId in effectIntents.ElementIds) {
			//	_ApplyFiltersForElementToIntents(effectIntents, elementId, effectNode.StartTime);
			//}
		}

		private void _ApplyFiltersForElementToIntents(EffectIntents effectIntents, Guid elementId, TimeSpan effectStartTime)
		{
			effectIntents.GetIntentNodesForElement(elementId).AsParallel().ForAll(intentNode =>
			                                                                      	{
			                                                                      		var elementFilters =
			                                                                      			_filterLookup.GetFiltersForElement(
			                                                                      				elementId, intentNode);
			                                                                      		elementFilters.AsParallel().ForAll(
			                                                                      			filter =>
			                                                                      			intentNode.ApplyFilter(filter,
			                                                                      			                       effectStartTime));
			                                                                      	});
			//foreach(IntentNode intentNode in effectIntents.GetIntentNodesForElement(elementId)) {
			//	ISequenceFilterNode[] elementFilters = _filterLookup.GetFiltersForElement(elementId, intentNode).ToArray();
			//	foreach(var filter in elementFilters) {
			//		intentNode.ApplyFilter(filter, effectStartTime);
			//	}
			//}
		}
	}
}
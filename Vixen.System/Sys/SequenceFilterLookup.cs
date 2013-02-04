using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class SequenceFilterLookup : Dictionary<Guid, ISequenceFilterNode[]> {
		public void AddElementFilters(Element element, IEnumerable<ISequenceFilterNode> filterNodes) {
			this[element.Id] = filterNodes.ToArray();
		}

		public IEnumerable<ISequenceFilterNode> GetFiltersForElement(Guid elementId, TimeSpan currentTime) {
			return _GetQualifiedFilters(elementId, x => _FilterQualifies(x, currentTime));
		}

		public IEnumerable<ISequenceFilterNode> GetFiltersForElement(Guid elementId, ITimeNode timeNode) {
			return _GetQualifiedFilters(elementId, x => _FilterQualifies(x, timeNode));
		}

		private IEnumerable<ISequenceFilterNode> _GetQualifiedFilters(Guid elementId, Predicate<ISequenceFilterNode> filterQualifier) {
			ISequenceFilterNode[] sequenceFilters;
			TryGetValue(elementId, out sequenceFilters);
			if(sequenceFilters != null) return sequenceFilters.Where(x => filterQualifier(x));
			return Enumerable.Empty<ISequenceFilterNode>();
		}

		private bool _FilterQualifies(ISequenceFilterNode sequenceFilterNode, TimeSpan currentTime) {
			return currentTime >= sequenceFilterNode.StartTime && currentTime < sequenceFilterNode.EndTime;
		}

		private bool _FilterQualifies(ISequenceFilterNode sequenceFilterNode, ITimeNode timeNode) {
			return TimeNode.IntersectsExclusively(timeNode, sequenceFilterNode);
		}
	}
}

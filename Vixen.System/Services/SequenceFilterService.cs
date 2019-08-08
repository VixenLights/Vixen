using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Services
{
	public class SequenceFilterService
	{
		private static SequenceFilterService _instance;

		private SequenceFilterService()
		{
		}

		public static SequenceFilterService Instance
		{
			get { return _instance ?? (_instance = new SequenceFilterService()); }
		}

		public SequenceFilterLookup BuildLookup(IEnumerable<IElementNode> elementNodes,
		                                        IEnumerable<ISequenceFilterNode> filterNodes)
		{
			SequenceFilterLookup lookup = new SequenceFilterLookup();

			Stack<IEnumerable<ISequenceFilterNode>> filterStack = new Stack<IEnumerable<ISequenceFilterNode>>();
			foreach (IElementNode node in elementNodes) {
				_SearchBranchForFilters(node, filterNodes, filterStack, lookup);
			}

			return lookup;
		}

		private void _SearchBranchForFilters(IElementNode node, IEnumerable<ISequenceFilterNode> filters,
		                                     Stack<IEnumerable<ISequenceFilterNode>> filtersFound, SequenceFilterLookup lookup)
		{
			// Must push a single value for each level we enter.
			ISequenceFilterNode[] sequenceFilterNodes = _GetFiltersForNode(node, filters);
			if (sequenceFilterNodes.Length > 0) {
				filtersFound.Push(sequenceFilterNodes);
			}
			else {
				filtersFound.Push(null);
			}

			if (node.IsLeaf) {
				ISequenceFilterNode[] elementFilters = filtersFound.Where(x => x != null).Reverse().SelectMany(x => x).ToArray();
				if (elementFilters.Length > 0) {
					lookup.AddElementFilters(node.Element, elementFilters);
				}
			}
			else {
				foreach (IElementNode childNode in node.Children) {
					_SearchBranchForFilters(childNode, filters, filtersFound, lookup);
				}
			}

			// Pop a single value for every level we exit.
			filtersFound.Pop();
		}

		private ISequenceFilterNode[] _GetFiltersForNode(IElementNode node, IEnumerable<ISequenceFilterNode> filters)
		{
			return filters.Where(x => x.Filter.TargetNodes.Contains(node)).ToArray();
		}
	}
}
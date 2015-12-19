using System;
using System.Collections.Generic;
using Vixen.Module.OutputFilter;

namespace Vixen.Sys
{
	internal class FilterChildren
	{
		private Dictionary<IOutputFilterModuleInstance, HashSet<IOutputFilterModuleInstance>> _filterChildren;

		public FilterChildren()
		{
			_filterChildren = new Dictionary<IOutputFilterModuleInstance, HashSet<IOutputFilterModuleInstance>>();
		}

		public void AddFilter(IOutputFilterModuleInstance filter)
		{
			if (filter == null) throw new ArgumentNullException("filter");
			if (_filterChildren.ContainsKey(filter)) {
				throw new InvalidOperationException("Filter " + filter.Descriptor.TypeName +
				                                    " is being added to the child lookup more than once.");
			}
			_CreateChildCollection(filter);
		}

		public void RemoveFilter(IOutputFilterModuleInstance filter)
		{
			if (filter == null) throw new ArgumentNullException("filter");

			_RemoveChildren(filter);
			_RemoveFromParent(filter);
		}

		public void SetParent(IOutputFilterModuleInstance filter, IOutputFilterModuleInstance parentFilter)
		{
			if (filter == null) throw new ArgumentNullException("filter");

			_RemoveFromParent(filter);
			_SetParent(filter, parentFilter);
		}

		//public void AddChild(IOutputFilterModuleInstance parentFilter, IOutputFilterModuleInstance childFilter) {
		//}

		//public void RemoveChild(IOutputFilterModuleInstance parentFilter, IOutputFilterModuleInstance childFilter) {
		//}

		private void _SetParent(IOutputFilterModuleInstance filter, IOutputFilterModuleInstance parentFilter)
		{
			if (parentFilter != null) {
				_GetChildren(parentFilter).Add(filter);
			}
		}

		private void _RemoveChildren(IOutputFilterModuleInstance filter)
		{
			_filterChildren.Remove(filter);
		}

		private void _RemoveFromParent(IOutputFilterModuleInstance filter)
		{
			// Filter may not yet be connected.
			if (filter.Source == null) return;
			// Source may be a element.
			IOutputFilterModuleInstance parentFilter = filter.Source as IOutputFilterModuleInstance;
			if (parentFilter != null) {
				_GetChildren(parentFilter).Remove(filter);
			}
		}

		public HashSet<IOutputFilterModuleInstance> GetChildren(IOutputFilterModuleInstance filter)
		{
			return _GetChildren(filter);
		}

		private HashSet<IOutputFilterModuleInstance> _GetChildren(IOutputFilterModuleInstance filter)
		{
			HashSet<IOutputFilterModuleInstance> chilren;

			if (!_filterChildren.TryGetValue(filter, out chilren)) {
				chilren = _CreateChildCollection(filter);
			}

			return chilren;
		}

		private HashSet<IOutputFilterModuleInstance> _CreateChildCollection(IOutputFilterModuleInstance filter)
		{
			HashSet<IOutputFilterModuleInstance> childCollection = new HashSet<IOutputFilterModuleInstance>();
			_filterChildren[filter] = childCollection;

			return childCollection;
		}
	}
}
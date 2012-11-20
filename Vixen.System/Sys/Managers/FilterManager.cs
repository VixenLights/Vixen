using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;

namespace Vixen.Sys.Managers {
	public class FilterManager : IEnumerable<IOutputFilterModuleInstance> {
		private Dictionary<Guid, IOutputFilterModuleInstance> _instances;
		// The data flow manager has data flow roots, but those are channels and are updated
		// in a separate layer.  We need to track our own roots separately for updates.
		private HashSet<IOutputFilterModuleInstance> _rootFilters;
		private FilterChildren _filterChildren; 
		private object _updateLock = new object();

		public FilterManager(DataFlowManager dataFlowManager) {
			_instances = new Dictionary<Guid, IOutputFilterModuleInstance>();
			_rootFilters = new HashSet<IOutputFilterModuleInstance>();
			_filterChildren = new FilterChildren();

			dataFlowManager.ComponentAdded += DataFlowManagerOnComponentAdded;
			dataFlowManager.ComponentRemoved += DataFlowManagerOnComponentRemoved;
			dataFlowManager.ComponentSourceChanged += DataFlowManagerOnComponentSourceChanged;
		}

		public void AddFilter(IOutputFilterModuleInstance filter) {
			VixenSystem.DataFlow.AddComponent(filter);
		}

		public void RemoveFilter(IOutputFilterModuleInstance filter) {
			VixenSystem.DataFlow.RemoveComponent(filter);
		}

		public void AddRange(IEnumerable<IOutputFilterModuleInstance> filters) {
			foreach(IOutputFilterModuleInstance filter in filters) {
				AddFilter(filter);
			}
		}

		private void DataFlowManagerOnComponentAdded(object sender, DataFlowComponentEventArgs e) {
			IOutputFilterModuleInstance filter = e.Component as IOutputFilterModuleInstance;
			if(filter == null) return;

			lock(_updateLock) {
				_AddFilterInstance(filter);
				_AddRootNode(filter);
				_AddDataModel(filter);
			}
		}

		private void DataFlowManagerOnComponentRemoved(object sender, DataFlowComponentEventArgs e) {
			IOutputFilterModuleInstance filter = e.Component as IOutputFilterModuleInstance;
			if(filter == null) return;

			lock(_updateLock) {
				_RemoveFilterInstance(filter);
				_RemoveFromRoots(filter);
				_RemoveDataModel(filter);
			}
		}

		private void DataFlowManagerOnComponentSourceChanged(object sender, DataFlowComponentEventArgs e) {
			IOutputFilterModuleInstance filter = e.Component as IOutputFilterModuleInstance;
			if(filter == null) return;

			IOutputFilterModuleInstance filterParent = filter.Source as IOutputFilterModuleInstance;
			if(filterParent == null) {
				_AddRootNode(filter);
			} else {
				_RemoveFromRoots(filter);
			}
			_filterChildren.SetParent(filter, filterParent);
		}

		public void Update() {
			lock(_updateLock) {
				_rootFilters.AsParallel().ForAll(_UpdateFilterBranch);
			}
		}

		private void _AddFilterInstance(IOutputFilterModuleInstance filter) {
			_AddInstanceReference(filter);
			_filterChildren.AddFilter(filter);
		}

		private void _AddInstanceReference(IOutputFilterModuleInstance filter) {
			lock(_instances) {
				_instances[filter.DataFlowComponentId] = filter;
			}
		}

		private void _AddRootNode(IOutputFilterModuleInstance filter) {
			lock(_rootFilters) {
				_rootFilters.Add(filter);
			}
		}

		private void _AddDataModel(IOutputFilterModuleInstance filter) {
			VixenSystem.ModuleStore.InstanceData.AssignModuleInstanceData(filter);
		}

		private void _RemoveFilterInstance(IOutputFilterModuleInstance filter) {
			_RemoveInstanceReference(filter);
			_filterChildren.RemoveFilter(filter);
		}

		private void _RemoveFromRoots(IOutputFilterModuleInstance filter) {
			lock(_rootFilters) {
				_rootFilters.Remove(filter);
			}
		}

		private void _RemoveInstanceReference(IOutputFilterModuleInstance filter) {
			lock(_instances) {
				_instances.Remove(filter.DataFlowComponentId);
			}
		}

		private void _RemoveDataModel(IOutputFilterModuleInstance filter) {
			VixenSystem.ModuleStore.InstanceData.RemoveModuleInstanceData(filter);
		}

		private void _UpdateFilterBranch(IOutputFilterModuleInstance filter) {
			_UpdateFromSource(filter);

			foreach(var childFilter in _filterChildren.GetChildren(filter)) {
				_UpdateFilterBranch(childFilter);
			}
		}

		private void _UpdateFromSource(IOutputFilterModuleInstance filter) {
			if(filter == null || filter.Source == null) return;
			IDataFlowData data = filter.Source.GetOutputState();
			if (data != null)
				filter.Update(data);
		}

		public IEnumerator<IOutputFilterModuleInstance> GetEnumerator() {
			return _instances.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Data.Flow;
using Vixen.Module.OutputFilter;
using Vixen.Sys.Instrumentation;



namespace Vixen.Sys.Managers
{
	public class FilterManager : IEnumerable<IOutputFilterModuleInstance>
	{
		private MillisecondsValue _filterUpdateTimeValue = new MillisecondsValue("   Filters update");
		//private MillisecondsValue _filterUpdateWaitValue = new MillisecondsValue("   Filters wait");
		private static Stopwatch _stopwatch = Stopwatch.StartNew();
		private Dictionary<Guid, IOutputFilterModuleInstance> _instances;
		// The data flow manager has data flow roots, but those are elements and are updated
		// in a separate layer.  We need to track our own roots separately for updates.
		private HashSet<IOutputFilterModuleInstance> _rootFilters;
		private FilterChildren _filterChildren;
		private object _updateLock = new object();
		private readonly ParallelOptions _po = new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount};

		public FilterManager(DataFlowManager dataFlowManager)
		{
			_instances = new Dictionary<Guid, IOutputFilterModuleInstance>();
			_rootFilters = new HashSet<IOutputFilterModuleInstance>();
			_filterChildren = new FilterChildren();

			dataFlowManager.ComponentAdded += DataFlowManagerOnComponentAdded;
			dataFlowManager.ComponentRemoved += DataFlowManagerOnComponentRemoved;
			dataFlowManager.ComponentSourceChanged += DataFlowManagerOnComponentSourceChanged;

			VixenSystem.Instrumentation.AddValue(_filterUpdateTimeValue);
			//VixenSystem.Instrumentation.AddValue(_filterUpdateWaitValue);
		}

		public void AddFilter(IOutputFilterModuleInstance filter)
		{
			VixenSystem.DataFlow.AddComponent(filter);
		}

		public void RemoveFilter(IOutputFilterModuleInstance filter)
		{
			VixenSystem.DataFlow.RemoveComponent(filter);
		}

		public void AddRange(IEnumerable<IOutputFilterModuleInstance> filters)
		{
			foreach (IOutputFilterModuleInstance filter in filters) {
				AddFilter(filter);
			}
		}

		private void DataFlowManagerOnComponentAdded(object sender, DataFlowComponentEventArgs e)
		{
			IOutputFilterModuleInstance filter = e.Component as IOutputFilterModuleInstance;
			if (filter == null) return;

			lock (_updateLock) {
				_AddFilterInstance(filter);
				if (filter.Source != null)
				{
					// If we have a source, see if it is a parent output filter
					//If it is not a output filter, then we are a root node, otherwise we are some type of child
					IOutputFilterModuleInstance filterParent = filter.Source.Component as IOutputFilterModuleInstance;
					if (filterParent == null)
					{
						_AddRootNode(filter);
					}
					//_filterChildren.SetParent(filter, filterParent);	
				}
				_AddDataModel(filter);
			}
		}

		private void DataFlowManagerOnComponentRemoved(object sender, DataFlowComponentEventArgs e)
		{
			IOutputFilterModuleInstance filter = e.Component as IOutputFilterModuleInstance;
			if (filter == null) return;

			lock (_updateLock) {
				_RemoveFilterInstance(filter);
				_RemoveFromRoots(filter);
				_RemoveDataModel(filter);
			}
		}

		private void DataFlowManagerOnComponentSourceChanged(object sender, DataFlowComponentEventArgs e)
		{
			IOutputFilterModuleInstance filter = e.Component as IOutputFilterModuleInstance;
			if (filter == null) return;

			lock (_updateLock)
			{

				if (filter.Source != null)
				{
					IOutputFilterModuleInstance filterParent = filter.Source.Component as IOutputFilterModuleInstance;
					if (filterParent == null)
					{
						_AddRootNode(filter);
					}
					else
					{
						_RemoveFromRoots(filter);	
					}
					_filterChildren.SetParent(filter, filterParent);
				}
				else
				{
					_RemoveFromRoots(filter);
					_filterChildren.SetParent(filter, null);
				}
			}
		}

		public void Update()
		{
			_stopwatch.Restart();
			lock (_updateLock)
			{
				Parallel.ForEach(_rootFilters, _UpdateFilterBranch);	
			}
			_filterUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
		}

		private void _AddFilterInstance(IOutputFilterModuleInstance filter)
		{
			_AddInstanceReference(filter);
			_filterChildren.AddFilter(filter);
		}

		private void _AddInstanceReference(IOutputFilterModuleInstance filter)
		{
			lock (_instances) {
				_instances[filter.DataFlowComponentId] = filter;
			}
		}

		private void _AddRootNode(IOutputFilterModuleInstance filter)
		{
			_rootFilters.Add(filter);
		}

		private void _AddDataModel(IOutputFilterModuleInstance filter)
		{
			VixenSystem.ModuleStore.InstanceData.AssignModuleInstanceData(filter);
		}

		private void _RemoveFilterInstance(IOutputFilterModuleInstance filter)
		{
			_RemoveInstanceReference(filter);
			_filterChildren.RemoveFilter(filter);
		}

		private void _RemoveFromRoots(IOutputFilterModuleInstance filter)
		{
			_rootFilters.Remove(filter);
		}

		private void _RemoveInstanceReference(IOutputFilterModuleInstance filter)
		{
			lock (_instances) {
				_instances.Remove(filter.DataFlowComponentId);
			}
		}

		private void _RemoveDataModel(IOutputFilterModuleInstance filter)
		{
			VixenSystem.ModuleStore.InstanceData.RemoveModuleInstanceData(filter);
		}

		private void _UpdateFilterBranch(IOutputFilterModuleInstance filter)
		{
			_UpdateFromSource(filter);

			foreach (var childFilter in _filterChildren.GetChildren(filter)) {
				_UpdateFilterBranch(childFilter);
			}
		}

		private void _UpdateFromSource(IOutputFilterModuleInstance filter)
		{
			if (filter == null || filter.Source == null) return;
			IDataFlowData data = filter.Source.GetOutputState();
			if (data != null)
				filter.Update(data);
		}

		public List<IOutputFilterModuleInstance> GetOrphanedFilters()
		{
			return _instances.Values.Except(_rootFilters).Where(x => x.Source == null).ToList();
		}

		public void RemoveOrphanedFilters()
		{
			foreach (var filter in GetOrphanedFilters())
			{
				RemoveFilterChain(filter);
			}
		}

		public void RemoveFilterChain(IOutputFilterModuleInstance filter)
		{
			foreach (var childFilter in _filterChildren.GetChildren(filter))
			{
				RemoveFilterChain(childFilter);	
			}
			RemoveFilter(filter);
		}

		public IEnumerator<IOutputFilterModuleInstance> GetEnumerator()
		{
			return _instances.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
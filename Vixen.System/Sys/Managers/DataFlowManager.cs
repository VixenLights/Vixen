using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;

namespace Vixen.Sys.Managers
{
	public class DataFlowManager : IEnumerable<DataFlowPatch>
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private Dictionary<Guid, IDataFlowComponent> _componentLookup;
		private Dictionary<IDataFlowComponent, List<IDataFlowComponent>> _componentDestinations;

		public event EventHandler<DataFlowComponentEventArgs> ComponentSourceChanged;
		public event EventHandler<DataFlowComponentEventArgs> ComponentAdded;
		public event EventHandler<DataFlowComponentEventArgs> ComponentRemoved;

		public DataFlowManager()
		{
			_componentLookup = new Dictionary<Guid, IDataFlowComponent>();
			_componentDestinations = new Dictionary<IDataFlowComponent, List<IDataFlowComponent>>();
		}

		public void Initialize(IEnumerable<DataFlowPatch> dataFlowPatches)
		{
			foreach (DataFlowPatch dataFlowPatch in dataFlowPatches) {
				IDataFlowComponent childComponent = GetComponent(dataFlowPatch.ComponentId);
				IDataFlowComponent sourceComponent = GetComponent(dataFlowPatch.SourceComponentId);
				if (childComponent != null && dataFlowPatch.SourceComponentOutputIndex >= 0 &&
				    dataFlowPatch.SourceComponentOutputIndex < sourceComponent.Outputs.Length) {
					VixenSystem.DataFlow.SetComponentSource(childComponent, sourceComponent, dataFlowPatch.SourceComponentOutputIndex);
				}
			}
		}

		public IEnumerable<IDataFlowComponent> GetAllComponents()
		{
			return _componentLookup.Values;
		}

		public IEnumerable<IDataFlowComponent> GetDestinationsOfComponent(IDataFlowComponent component)
		{
			if (_componentDestinations.ContainsKey(component)) {
				return _componentDestinations[component];
			}
			return Enumerable.Empty<IDataFlowComponent>();
		}

		public IEnumerable<IDataFlowComponent> GetDestinationsOfComponentOutput(IDataFlowComponent component, int outputIndex)
		{
			if (_componentDestinations.ContainsKey(component)) {
				return _componentDestinations[component].Where(x => x.Source.OutputIndex == outputIndex);
			}
			return Enumerable.Empty<IDataFlowComponent>();
		}

		public IDataFlowComponent GetComponent(Guid? id)
		{
			if (!id.HasValue) return null;

			IDataFlowComponent component;
			_componentLookup.TryGetValue(id.Value, out component);
			return component;
		}

		public bool SetComponentSource(IDataFlowComponent component, IDataFlowComponent sourceComponent, int sourceOutputIndex)
		{
			return SetComponentSource(component, new DataFlowComponentReference(sourceComponent, sourceOutputIndex));
		}

		public bool SetComponentSource(IDataFlowComponent component, IDataFlowComponentReference source)
		{
			if (component == null) throw new ArgumentNullException("component");

			if (_CheckComponentSourceForCircularDependency(component, source))
				return false;

			_RemoveComponentSource(component);
			_SetComponentSource(component, source);
			return true;
		}

		public bool CheckComponentSourceForCircularDependency(IDataFlowComponent component, IDataFlowComponent source)
		{
			return _CheckComponentSourceForCircularDependency(component, source);
		}

		public bool CheckComponentSourceForCircularDependency(IDataFlowComponent component, IDataFlowComponentReference source)
		{
			return _CheckComponentSourceForCircularDependency(component, source);
		}

		public void ResetComponentSource(IDataFlowComponent component)
		{
			if (component == null) throw new ArgumentNullException("component");

			_RemoveComponentSource(component);
		}

		public void AddComponent(IDataFlowComponent component)
		{
			if (component == null) throw new ArgumentNullException("component");

			_AddComponent(component);
		}

		public void RemoveComponent(IDataFlowComponent component)
		{
			if (component == null) throw new ArgumentNullException("component");

			_RemoveComponent(component);
		}

		public void ApplyPatch(DataFlowPatch dataFlowPatch)
		{
			IDataFlowComponent component = GetComponent(dataFlowPatch.ComponentId);
			IDataFlowComponent sourceComponent = GetComponent(dataFlowPatch.SourceComponentId);
			if (component != null) {
				SetComponentSource(component, sourceComponent, dataFlowPatch.SourceComponentOutputIndex);
			}
		}

		private void _AddComponent(IDataFlowComponent component)
		{
			_componentLookup[component.DataFlowComponentId] = component;

			OnComponentAdded(component);
		}

		private void _RemoveComponent(IDataFlowComponent component)
		{
			_RemoveAsSource(component);
			_RemoveComponentSource(component);

			_componentLookup.Remove(component.DataFlowComponentId);

			OnComponentRemoved(component);
		}

		private void _RemoveAsSource(IDataFlowComponent component)
		{
			List<IDataFlowComponent> childComponents;
			_componentDestinations.TryGetValue(component, out childComponents);
			if (childComponents != null) {
				foreach (IDataFlowComponent childComponent in childComponents) {
					_RemoveComponentSource(childComponent);
				}
			}
		}

		private void _RemoveComponentSource(IDataFlowComponent component)
		{
			if (component.Source == null) return;

			IDataFlowComponent parent = component.Source.Component;
			List<IDataFlowComponent> children = null;
			_componentDestinations.TryGetValue(parent, out children);

			if (children == null) {
				Logging.Error("removing the source from a data flow component, but it's not already a child of the source!");
			} else {
				children.Remove(component);
				if (children.Count == 0) {
					_componentDestinations.Remove(parent);
				}
			}

			_SetComponentSource(component, null);
		}

		private bool _CheckComponentSourceForCircularDependency(IDataFlowComponent component,
		                                                        IDataFlowComponentReference source)
		{
			if (source == null)
				return false;

			return _CheckComponentSourceForCircularDependency(component, source.Component);
		}

		private bool _CheckComponentSourceForCircularDependency(IDataFlowComponent component, IDataFlowComponent source)
		{
			if (component == null || source == null)
				return false;

			if (source == component)
				return true;

			return _CheckComponentSourceForCircularDependency(component, source.Source);
		}

		private void _SetComponentSource(IDataFlowComponent component, IDataFlowComponentReference source)
		{
			if (Equals(source, component.Source)) return;

			component.Source = source;

			if (source != null) {
				// add the reverse reference (if we're not clearing it) -- track a data component's destinations, to make it easier to find later.
				if (!_componentDestinations.ContainsKey(source.Component)) {
					_componentDestinations[source.Component] = new List<IDataFlowComponent>();
				}
				_componentDestinations[source.Component].Add(component);
			}

			OnComponentSourceChanged(component);
		}

		protected virtual void OnComponentSourceChanged(IDataFlowComponent component)
		{
			if (ComponentSourceChanged != null) {
				ComponentSourceChanged(this, new DataFlowComponentEventArgs(component));
			}
		}

		protected virtual void OnComponentAdded(IDataFlowComponent component)
		{
			if (ComponentAdded != null) {
				ComponentAdded(this, new DataFlowComponentEventArgs(component));
			}
		}

		protected virtual void OnComponentRemoved(IDataFlowComponent component)
		{
			if (ComponentRemoved != null) {
				ComponentRemoved(this, new DataFlowComponentEventArgs(component));
			}
		}

		public IEnumerator<DataFlowPatch> GetEnumerator()
		{
			return _componentLookup.Values.SelectMany(GetDestinationsOfComponent).Select(x => new DataFlowPatch(x)).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
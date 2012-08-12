using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;

namespace Vixen.Sys.Managers {
	public class DataFlowManager : IEnumerable<DataFlowPatch> {
		private Dictionary<Guid, IDataFlowComponent> _componentLookup;

		public event EventHandler<DataFlowComponentEventArgs> ComponentSourceChanged;
		public event EventHandler<DataFlowComponentEventArgs> ComponentAdded;
		public event EventHandler<DataFlowComponentEventArgs> ComponentRemoved;

		public DataFlowManager() {
			_componentLookup = new Dictionary<Guid, IDataFlowComponent>();
		}

		public IEnumerable<IDataFlowComponent> GetAllComponents() {
			return _componentLookup.Values;
		}

		public IEnumerable<IDataFlowComponent> GetChildren(IDataFlowComponent component) {
			return _componentLookup.Values.Where(x => x.Source != null && x.Source.Component.Equals(component));
		}

		public IDataFlowComponent GetComponent(Guid? id) {
			if(!id.HasValue) return null;

			IDataFlowComponent component;
			_componentLookup.TryGetValue(id.Value, out component);
			return component;
		}

		public void SetComponentSource(IDataFlowComponent component, IDataFlowComponent sourceComponent, int sourceOutputIndex = 0) {
			if(component == null) throw new ArgumentNullException("component");

			_RemoveComponentSource(component);
			_SetComponentSource(component, new DataFlowComponentReference(sourceComponent, sourceOutputIndex));
		}

		public void ResetComponentSource(IDataFlowComponent component) {
			if(component == null) throw new ArgumentNullException("component");

			_RemoveComponentSource(component);
		}

		public void AddComponent(IDataFlowComponent component) {
			if(component == null) throw new ArgumentNullException("component");

			_AddComponent(component);
		}

		public void RemoveComponent(IDataFlowComponent component) {
			if(component == null) throw new ArgumentNullException("component");

			_RemoveComponent(component);
		}

		private void _AddComponent(IDataFlowComponent component) {
			_componentLookup[component.DataFlowComponentId] = component;

			OnComponentAdded(component);
		}

		private void _RemoveComponent(IDataFlowComponent component) {
			_RemoveAsSource(component);

			_componentLookup.Remove(component.DataFlowComponentId);

			OnComponentRemoved(component);
		}

		private void _RemoveAsSource(IDataFlowComponent component) {
			IEnumerable<IDataFlowComponent> childComponents = _componentLookup.Values.Where(x => x.Source.Component.Equals(component));
			foreach(IDataFlowComponent childComponent in childComponents) {
				_RemoveComponentSource(childComponent);
			}
		}

		private void _RemoveComponentSource(IDataFlowComponent component) {
			if(component.Source == null) return;

			_SetComponentSource(component, null);
		}

		private void _SetComponentSource(IDataFlowComponent component, IDataFlowComponentReference source) {
			if(source.Equals(component.Source)) return;

			component.Source = source;

			OnComponentSourceChanged(component);
		}

		protected virtual void OnComponentSourceChanged(IDataFlowComponent component) {
			if(ComponentSourceChanged != null) {
				ComponentSourceChanged(this, new DataFlowComponentEventArgs(component));
			}
		}

		protected virtual void OnComponentAdded(IDataFlowComponent component) {
			if(ComponentAdded != null) {
				ComponentAdded(this, new DataFlowComponentEventArgs(component));
			}
		}

		protected virtual void OnComponentRemoved(IDataFlowComponent component) {
			if(ComponentRemoved != null) {
				ComponentRemoved(this, new DataFlowComponentEventArgs(component));
			}
		}

		public IEnumerator<DataFlowPatch> GetEnumerator() {
			return _componentLookup.Values.SelectMany(GetChildren).Select(x => new DataFlowPatch(x)).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}

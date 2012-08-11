using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Data.Flow;

namespace Vixen.Sys.Managers {
	public class DataFlowManager : IEnumerable<DataFlowPatch> {
		//private Dictionary<Guid, DataFlowNode> _nodeLookup;
		//private HashSet<DataFlowNode> _rootNodes;
		private Dictionary<Guid, IDataFlowComponent> _componentLookup;

		public event EventHandler<DataFlowComponentEventArgs> ComponentSourceChanged;
		public event EventHandler<DataFlowComponentEventArgs> ComponentAdded;
		public event EventHandler<DataFlowComponentEventArgs> ComponentRemoved;

		public DataFlowManager() {
			//_nodeLookup = new Dictionary<Guid, DataFlowNode>();
			//_rootNodes = new HashSet<DataFlowNode>();
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

		//public IEnumerable<Guid> GetChildren(IDataFlowComponent component) {
		//    if(component == null) throw new ArgumentNullException("component");

		//    return GetChildren(component.DataFlowComponentId);
		//}

		//public IEnumerable<Guid> GetChildren(Guid componentId) {
		//    DataFlowNode node = _GetNode(componentId);
		//    return (node == null) ? Enumerable.Empty<Guid>() : node.Children;
		//}

		public void SetComponentSource(IDataFlowComponent component, IDataFlowComponent sourceComponent, int sourceOutputIndex) {
			if(component == null) throw new ArgumentNullException("component");
			_RemoveComponentSource(component);
			_SetComponentSource(component, new DataFlowComponentReference(sourceComponent, sourceOutputIndex));
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
			//_CreateNode(component);

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

		private void _SetComponentSource(IDataFlowComponent component, DataFlowComponentReference source) {
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

		////public void Add(IDataFlowComponent dataFlowComponent, IDataFlowComponentReference source) {
		////    AddParticipant(dataFlowComponent);
		////    if(source != null) {
		////        AddParticipant(source.Component);
		////    }
		////    AddParticipantRelationship(dataFlowComponent, source);
		////}

		//internal void AddParticipantRelationship(IDataFlowComponent dataFlowComponent, IDataFlowComponentReference componentSource) {
		//    AddParticipant(dataFlowComponent);
		//    if(componentSource != null) {
		//        AddParticipant(componentSource.Component);
		//    }
		//    //if(Get(dataFlowComponent.DataFlowComponentId) == null) throw new InvalidOperationException("Component must exist before creating a relationship.");
		//    //if(componentSource != null && Get(componentSource.Component.DataFlowComponentId) == null) throw new InvalidOperationException("Component source must exist before creating a relationship.");

		//    if(componentSource == null) {
		//        _AddRootComponent(dataFlowComponent);
		//    } else {
		//        _AddToParent(dataFlowComponent, componentSource);
		//    }
		//}

		//internal void AddParticipant(IDataFlowComponent dataFlowComponent) {
		//    if(dataFlowComponent == null) return;
		//    _componentLookup[dataFlowComponent.DataFlowComponentId] = dataFlowComponent;
		//    _CreateNode(dataFlowComponent);
		//    //if(_AddToParent(value, source)) {
		//    //    _AddRoot(value);
		//    //}
		//}

		//internal void AddParticipantRelationships(IEnumerable<DataFlowPatch> values) {
		//    foreach(DataFlowPatch dataFlowPatch in values) {
		//        //// Wherever DataFlowPatches come in, we need to be sure the components exist.
		//        //if(!_ValidPatch(dataFlowPatch)) continue;

		//        DataFlowNode dataFlowNode = new DataFlowNode(dataFlowPatch.ComponentId);
		//        if(dataFlowPatch.ParentId == null) {
		//            _AddRootNode(dataFlowNode);
		//        } else {
		//            DataFlowNode parentNode = _GetNode(dataFlowPatch.ParentId.Value);
		//            if(parentNode != null) {
		//                _AddNodeToParent(dataFlowNode, parentNode);
		//            }
		//        }
		//    }
		//}

		////public bool IsRoot(IDataFlowComponent value) {
		////    DataFlowNode node = _GetNode(value);
		////    if(node != null) {
		////        return _rootNodes.Contains(node);
		////    }
		////    return false;
		////}

		//internal void Remove(IDataFlowComponent dataFlowComponent) {
		//    DataFlowNode node = _GetNode(dataFlowComponent);
		//    if(node == null) return;

		//    _RemoveFromNodeLookup(node);
		//    _RemoveFromComponentLookup(node.DataFlowComponentId);
		//    _RemoveFromRoots(node);
		//}

		//internal IDataFlowComponent Get(Guid id) {
		//    IDataFlowComponent component;
		//    _componentLookup.TryGetValue(id, out component);
		//    return component;
		//}

		////public void Clear() {
		////    _ClearRoots();
		////    _ClearNodeLookup();
		////    _ClearComponentLookup();
		////}

		////private void _ClearRoots() {
		////    lock(_rootNodes) {
		////        _rootNodes.Clear();
		////    }
		////}

		////private void _ClearNodeLookup() {
		////    lock(_nodeLookup) {
		////        _nodeLookup.Clear();
		////    }
		////}

		////private void _ClearComponentLookup() {
		////    lock(_componentLookup) {
		////        _componentLookup.Clear();
		////    }
		////}

		//private void _AddToParent(IDataFlowComponent value, IDataFlowComponentReference source) {
		//    DataFlowNode parentNode = _GetOrCreateNode(source.Component);
		//    //if(parentNode == null) return false;
		//    if(parentNode == null) return;

		//    DataFlowNode childNode = _GetOrCreateNode(value);
		//    _AddNodeToParent(childNode, parentNode);
			
		//    value.Source = source;
			
		//    //return true;
		//}

		//private static void _AddNodeToParent(DataFlowNode childNode, DataFlowNode parentNode) {
		//    parentNode.AddChild(childNode);
		//}

		//private void _AddRootComponent(IDataFlowComponent value) {
		//    DataFlowNode node = _GetOrCreateNode(value);
		//    _AddRootNode(node);
		//    value.Source = null;
		//}

		//private void _AddRootNode(DataFlowNode node) {
		//    lock(_rootNodes) {
		//        _rootNodes.Add(node);
		//    }
		//}

		//private void _RemoveFromNodeLookup(DataFlowNode node) {
		//    lock(_nodeLookup) {
		//        _RemoveFromParent(node);
		//        _nodeLookup.Remove(node.DataFlowComponentId);
		//    }
		//}

		//private void _RemoveFromParent(DataFlowNode node) {
		//    DataFlowNode parentNode = _nodeLookup.Values.FirstOrDefault(x => x.Contains(node));
		//    if(parentNode != null) {
		//        parentNode.RemoveChild(node);
		//    }
		//}

		//private void _RemoveFromComponentLookup(Guid id) {
		//    lock(_componentLookup) {
		//        _componentLookup.Remove(id);
		//    }
		//}

		//private void _RemoveFromRoots(DataFlowNode node) {
		//    _rootNodes.Remove(node);
		//}

		//private DataFlowNode _GetNode(IDataFlowComponent dataFlowComponent) {
		//    if(dataFlowComponent == null) return null;
		//    return _GetNode(dataFlowComponent.DataFlowComponentId);
		//}

		//private DataFlowNode _GetNode(Guid nodeId) {
		//    DataFlowNode node;
		//    _nodeLookup.TryGetValue(nodeId, out node);
		//    return node;
		//}

		//private DataFlowNode _GetOrCreateNode(IDataFlowComponent dataFlowComponent) {
		//    if(dataFlowComponent == null) return null;
		//    return _GetNode(dataFlowComponent) ?? _CreateNode(dataFlowComponent);
		//}

		//private DataFlowNode _CreateNode(IDataFlowComponent dataFlowComponent) {
		//    DataFlowNode node = new DataFlowNode(dataFlowComponent);
		//    _AddNodeToLookup(node);
		//    return node;
		//}

		//private void _AddNodeToLookup(DataFlowNode node) {
		//    lock(_nodeLookup) {
		//        _nodeLookup[node.DataFlowComponentId] = node;
		//    }
		//}

		//private IEnumerable<DataFlowPatch> _GetNodePatchesFromBranch(Guid nodeId, Guid? parentId = null) {
		//    yield return new DataFlowPatch(nodeId, parentId);

		//    foreach(Guid childId in VixenSystem.DataFlow.GetChildren(nodeId)) {
		//        foreach(DataFlowPatch childPatch in _GetNodePatchesFromBranch(childId, nodeId)) {
		//            yield return childPatch;
		//        }
		//    }
		//}

		//private bool _ValidPatch(DataFlowPatch dataFlowPatch) {
		//    if(_GetNode(dataFlowPatch.ComponentId) == null) return false;
		//    return dataFlowPatch.ParentId == null || _GetNode(dataFlowPatch.ParentId.Value) != null;
		//}

		public IEnumerator<DataFlowPatch> GetEnumerator() {
			//return _rootNodes.SelectMany(x => _GetNodePatchesFromBranch(x.DataFlowComponentId)).GetEnumerator();
			return _componentLookup.Values.SelectMany(GetChildren).Select(x => new DataFlowPatch(x)).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		//#region DataFlowNode
		//private class DataFlowNode : IEquatable<DataFlowNode> {
		//    private HashSet<DataFlowNode> _children;

		//    public DataFlowNode(IDataFlowComponent dataFlowComponent)
		//        : this(dataFlowComponent.DataFlowComponentId) {
		//    }

		//    public DataFlowNode(Guid componentId) {
		//        DataFlowComponentId = componentId;
		//        _children = new HashSet<DataFlowNode>();
		//    }

		//    public Guid DataFlowComponentId { get; private set; }

		//    public void AddChild(DataFlowNode child) {
		//        _children.Add(child);
		//    }

		//    public void RemoveChild(DataFlowNode child) {
		//        _children.Remove(child);
		//    }

		//    public bool Contains(DataFlowNode child) {
		//        return _children.Contains(child);
		//    }

		//    public IEnumerable<Guid> Children {
		//        get { return _children.Select(x => x.DataFlowComponentId); }
		//    }

		//    #region IEquality
		//    public bool Equals(DataFlowNode other) {
		//        if(ReferenceEquals(null, other)) return false;
		//        if(ReferenceEquals(this, other)) return true;
		//        return other.DataFlowComponentId.Equals(DataFlowComponentId);
		//    }

		//    public override bool Equals(object obj) {
		//        if(ReferenceEquals(null, obj)) return false;
		//        if(ReferenceEquals(this, obj)) return true;
		//        if(obj.GetType() != typeof(DataFlowNode)) return false;
		//        return Equals((DataFlowNode)obj);
		//    }

		//    public override int GetHashCode() {
		//        return DataFlowComponentId.GetHashCode();
		//    }
		//    #endregion
		//}
		//#endregion
	}
}

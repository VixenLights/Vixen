using System;
using System.Collections.Generic;
using System.Linq;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class ElementModelNodeManager : IEnumerable<ElementModelNode>
    {
        private ElementModelNode _rootNode;

        // a mapping of element node GUIDs to element node instances. Used for initial creation, to easily find nodes we have already created.
        // once they've been created, they're in the dictionary. The only way to 'delete' ElementModelNodes is to make a new NodeManager,
        // which reinitializes this mapping and we can start fresh.
        private readonly Dictionary<Guid, ElementModelNode> _instances;

        public static event EventHandler NodesChanged;

        public ElementModelNodeManager()
        {
            _instances = new Dictionary<Guid, ElementModelNode>();
            ElementModelNode.Changed += ElementModelNode_Changed;
        }

        public ElementModelNodeManager(IEnumerable<ElementModelNode> nodes)
            : this()
        {
            AddNodes(nodes);
        }

        private void ElementModelNode_Changed(object sender, EventArgs e)
        {
            OnNodesChanged();
        }

        public ElementModelNode RootNode
        {
            get
            {
                if (_rootNode == null)
                    _rootNode = new ElementModelNode("Root");

                return _rootNode;
            }
        }

        public void MoveNode(ElementModelNode movingNode, ElementModelNode newParent, ElementModelNode oldParent, int index = -1)
        {
            // if null nodes, default to the root node.
            newParent = newParent ?? RootNode;
            oldParent = oldParent ?? RootNode;

            // if we are going to be moving a node within its same group, but to a later position, we need to offset
            // the destination index by 1: once we remove a node, everything shuffles up one, and we need to account for it
            if (oldParent == newParent && index >= 0 && index > newParent.IndexOfChild(movingNode))
            {
                index--;
            }

            // remove the node from its old parent, not culling any floating children (since it's about to be added
            // again somewhere else) and then move it to the new parent, in the desired position if set
            RemoveNode(movingNode, oldParent, false);
            AddChildToParent(movingNode, newParent, index);
        }

        public void AddNode(ElementModelNode node, ElementModelNode parent = null)
        {
            AddChildToParent(node, parent);
        }

        public void AddNodes(IEnumerable<ElementModelNode> nodes, ElementModelNode parent = null)
        {
            foreach (ElementModelNode node in nodes)
            {
                AddNode(node, parent);
            }
        }

        public ElementModelNode AddNode(string name, ElementModelNode parent = null, bool uniquifyName = true)
        {
            if (uniquifyName)
            {
                name = _Uniquify(name);
            }
            ElementModelNode newNode = new ElementModelNode(name);
            AddNode(newNode, parent);
            return newNode;
        }

        public void RemoveNode(ElementModelNode node, ElementModelNode parent, bool cleanup)
        {
            // if the given parent is null, it's most likely a root node (ie. with
            // a parent of our private RootNode). Try to remove it from that instead.
            if (parent == null)
            {
                node.RemoveFromParent(RootNode, cleanup);
            }
            else
            {
                node.RemoveFromParent(parent, cleanup);
                //If the parent no longer has children, add a element back to it.
                if (parent.IsLeaf && parent.ElementModel == null)
                {
                    parent.ElementModel = new ElementModel(parent.Name);
                    PropModelServices.ElementModels.AddElement(parent.ElementModel);
                }
            }

        }

        public void RenameNode(ElementModelNode node, string newName)
        {
            node.Name = _Uniquify(newName);
            if (node.ElementModel != null)
                node.ElementModel.Name = node.Name;
        }

        public void AddChildToParent(ElementModelNode child, ElementModelNode parent, int index = -1)
        {
            // if no parent was specified, add to the root node.
            if (parent == null)
                parent = RootNode;

            // if an item is a group (or is becoming one), it can't have an output
            // element anymore. Remove it.
            if (parent.ElementModel != null)
            {
                PropModelServices.ElementModels.RemoveElement(parent.ElementModel);
                parent.ElementModel = null;
            }

            // if an index was specified, insert it in that position, otherwise just add it at the end
            if (index < 0)
                parent.AddChild(child);
            else
                parent.InsertChild(index, child);
        }

        private string _Uniquify(string name)
        {
            if (_instances.Values.Any(x => x.Name == name))
            {
                string originalName = name;
                bool unique;
                int counter = 2;
                do
                {
                    name = $"{originalName} - {counter++}";
                    unique = _instances.Values.All(x => x.Name != name);
                } while (!unique);
            }
            return name;
        }

        public IEnumerable<ElementModelNode> InvalidRootNodes => RootNode.InvalidChildren();

        public bool SetElementModelNode(Guid id, ElementModelNode node)
        {
            bool rv = _instances.ContainsKey(id);

            _instances[id] = node;
            return rv;
        }

        public bool ClearElementModelNode(Guid id)
        {
            return _instances.Remove(id);
        }

        public ElementModelNode GetElementModelNode(Guid id)
        {
            if (_instances.ContainsKey(id))
            {
                return _instances[id];
            }
            return null;
        }

        public bool ElementModelNodeExists(Guid id)
        {
            return _instances.ContainsKey(id);
        }

        protected virtual void OnNodesChanged() => NodesChanged?.Invoke(this, EventArgs.Empty);

        public IEnumerable<ElementModelNode> GetLeafNodes()
        {
            // Don't want to return the root node.
            // note: this may very well return duplicate nodes, if they are part of different groups.
            return RootNode.Children.SelectMany(x => x.GetLeafEnumerator());
        }

        public IEnumerable<ElementModelNode> GetNonLeafNodes()
        {
            // Don't want to return the root node.
            // note: this may very well return duplicate nodes, if they are part of different groups.
            return RootNode.Children.SelectMany(x => x.GetNonLeafEnumerator());
        }

        public IEnumerable<ElementModelNode> GetRootNodes()
        {
            return RootNode.Children;
        }

        public IEnumerable<ElementModelNode> GetAllNodes()
        {
            //return RootNode.Children.SelectMany(x => x.GetNodeEnumerator());
            return _instances.Values;
        }

        public IEnumerator<ElementModelNode> GetEnumerator()
        {
            // Don't want to return the root node.
            return GetAllNodes().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

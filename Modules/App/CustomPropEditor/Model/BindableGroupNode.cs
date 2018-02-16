using System;
using System.Collections.Generic;
using System.Linq;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public abstract class BindableGroupNode<T>: BindableBase, IEnumerable<T>
    {
        private readonly List<BindableGroupNode<T>> _children;
        private readonly List<BindableGroupNode<T>> _parents;

        protected BindableGroupNode(string name, IEnumerable<BindableGroupNode<T>> content)
        {
            Name = name;
            _children = new List<BindableGroupNode<T>>(content ?? Enumerable.Empty<BindableGroupNode<T>>());
            _parents = new List<BindableGroupNode<T>>(Enumerable.Empty<BindableGroupNode<T>>());
            foreach (BindableGroupNode<T> child in _children)
            {
                child.AddParent(this);
            }
        }

        protected BindableGroupNode(string name, params BindableGroupNode<T>[] content)
            : this(name, (IEnumerable<BindableGroupNode<T>>)content)
        {
        }

        public virtual string Name { get; set; }

        public virtual void AddChild(BindableGroupNode<T> node)
        {
            if (!_children.Contains(node))
            {
                _children.Add(node);
                node.AddParent(this);
            }
        }

        public virtual void InsertChild(int index, BindableGroupNode<T> node)
        {
            if (!_children.Contains(node))
            {
                _children.Insert(index, node);
                node.AddParent(this);
            }
        }

        private void AddParent(BindableGroupNode<T> parent)
        {
            if (!_parents.Contains(parent))
            {
                _parents.Add(parent);
            }
        }

        public virtual bool RemoveFromParent(BindableGroupNode<T> parent, bool cleanupIfFloating)
        {
            // try to remove this node from the given parent.
            if (!parent.RemoveChild(this))
            {
                return false;
            }

            // if we don't have any parents left, we're floating free: recurse down, and
            // remove all children from this node. (This retains children that are also
            // children of other nodes, not just this one).
            if (cleanupIfFloating && Parents.Count() == 0)
            {
                foreach (BindableGroupNode<T> child in _children.ToList())
                {
                    child.RemoveFromParent(this, cleanupIfFloating);
                }
            }

            return true;
        }

        public virtual bool RemoveChild(BindableGroupNode<T> node)
        {
            if (_children.Remove(node))
            {
                node.RemoveParent(this);
                return true;
            }
            return false;
        }

        private bool RemoveParent(BindableGroupNode<T> parent)
        {
            if (_parents.Remove(parent))
            {
                return true;
            }
            return false;
        }

        public virtual BindableGroupNode<T> Get(int index)
        {
            return _children[index];
        }

        public virtual int IndexOfChild(BindableGroupNode<T> child)
        {
            return _children.IndexOf(child);
        }

        /// <summary>
        /// Recursively searches through all children for the given node.
        /// </summary>
        /// <param name="node">The node to search for.</param>
        /// <returns>True if the node is contained anywhere below this node.</returns>
        public virtual bool ContainsNode(BindableGroupNode<T> node)
        {
            foreach (BindableGroupNode<T> child in Children)
            {
                if (child.ContainsNode(node))
                    return true;
            }

            return Children.Contains(node);
        }

        public virtual BindableGroupNode<T> Find(string childName)
        {
            return _children.FirstOrDefault(x => x.Name.Equals(childName, StringComparison.OrdinalIgnoreCase));
        }

        public virtual IEnumerable<BindableGroupNode<T>> Children
        {
            get { return _children; }
        }

        public virtual IEnumerable<BindableGroupNode<T>> Parents
        {
            get { return _parents; }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _children.SelectMany(x => x).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}


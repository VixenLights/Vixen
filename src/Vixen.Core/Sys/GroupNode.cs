﻿namespace Vixen.Sys
{
	public abstract class GroupNode<T> : IEnumerable<T>
	{
		private List<GroupNode<T>> _children;
		private List<GroupNode<T>> _parents;

		protected GroupNode(string name, IEnumerable<GroupNode<T>> content)
		{
			Name = name;
			_children = new List<GroupNode<T>>(content ?? Enumerable.Empty<GroupNode<T>>());
			_parents = new List<GroupNode<T>>(Enumerable.Empty<GroupNode<T>>());
			foreach (GroupNode<T> child in _children) {
				child.AddParent(this);
			}
		}

		protected GroupNode(string name, params GroupNode<T>[] content)
			: this(name, (IEnumerable<GroupNode<T>>) content)
		{
		}

		public virtual string Name { get; set; }

		public virtual void AddChild(GroupNode<T> node)
		{
			if (!_children.Contains(node)) {
				_children.Add(node);
				node.AddParent(this);
			}
		}

		public virtual void InsertChild(int index, GroupNode<T> node)
		{
			if (!_children.Contains(node)) {
				_children.Insert(index, node);
				node.AddParent(this);
			}
		}

		private void AddParent(GroupNode<T> parent)
		{
			if (!_parents.Contains(parent)) {
				_parents.Add(parent);
			}
		}

		public virtual bool RemoveFromParent(GroupNode<T> parent, bool cleanupIfFloating)
		{
			// try to remove this node from the given parent.
			if (!parent.RemoveChild(this)) {
				return false;
			}

			// if we don't have any parents left, we're floating free: recurse down, and
			// remove all children from this node. (This retains children that are also
			// children of other nodes, not just this one).
			if (cleanupIfFloating && Parents.Count() == 0) {
				foreach (GroupNode<T> child in _children.ToList()) {
					child.RemoveFromParent(this, cleanupIfFloating);
				}
			}

			return true;
		}

		public virtual bool RemoveChild(GroupNode<T> node)
		{
			if (_children.Remove(node)) {
				node.RemoveParent(this);
				return true;
			}
			return false;
		}

		private bool RemoveParent(GroupNode<T> parent)
		{
			if (_parents.Remove(parent)) {
				return true;
			}
			return false;
		}

		public virtual GroupNode<T> Get(int index)
		{
			return _children[index];
		}

		public virtual int IndexOfChild(GroupNode<T> child)
		{
			return _children.IndexOf(child);
		}

		/// <summary>
		/// Recursively searches through all children for the given node.
		/// </summary>
		/// <param name="node">The node to search for.</param>
		/// <returns>True if the node is contained anywhere below this node.</returns>
		public virtual bool ContainsNode(GroupNode<T> node)
		{
			foreach (GroupNode<T> child in Children) {
				if (child.ContainsNode(node))
					return true;
			}

			return Children.Contains(node);
		}

		public virtual GroupNode<T> Find(string childName)
		{
			return _children.FirstOrDefault(x => x.Name.Equals(childName, StringComparison.OrdinalIgnoreCase));
		}

		public virtual IEnumerable<GroupNode<T>> Children
		{
			get { return _children; }
		}

		public virtual IEnumerable<GroupNode<T>> Parents
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
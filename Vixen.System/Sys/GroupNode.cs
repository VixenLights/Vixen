using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	abstract public class GroupNode<T> : IEnumerable<T> {
		private List<GroupNode<T>> _children;

		protected GroupNode(string name, IEnumerable<GroupNode<T>> content) {
			Name = name;
			_children = new List<GroupNode<T>>(content ?? Enumerable.Empty<GroupNode<T>>());
			foreach(GroupNode<T> child in _children) {
				child.Parent = this;
			}
		}
		
		protected GroupNode(string name, params GroupNode<T>[] content)
			: this(name, (IEnumerable<GroupNode<T>>)content) {
		}

		public string Name { get; set; }

		virtual public GroupNode<T> Parent { get; private set; }

		virtual public void Add(GroupNode<T> node) {
			if(!_children.Contains(node)) {
				_children.Add(node);
				node.Parent = this;
			}
		}

		virtual public bool Remove() {
			if(Parent != null) {
				return Parent.Remove(this);
			}
			return false;
		}

		virtual public bool Remove(GroupNode<T> node) {
			if(_children.Remove(node)) {
				node.Parent = null;
				return true;
			}
			return false;
		}

		virtual public GroupNode<T> Get(int index) {
			return _children[index];
		}

		virtual public GroupNode<T> Find(string childName) {
			return _children.FirstOrDefault(x => x.Name.Equals(childName, StringComparison.OrdinalIgnoreCase));
		}

		virtual public IEnumerable<GroupNode<T>> Children {
			get { return _children; }
		}

		virtual public IEnumerator<T> GetEnumerator() {
			return _children.SelectMany(x => x).GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		override public string ToString() {
			return Name;
		}
	}
}

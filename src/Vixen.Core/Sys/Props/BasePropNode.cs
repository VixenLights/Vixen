using System.Collections.ObjectModel;
using Vixen.Model;

namespace Vixen.Sys.Props
{
	public abstract class BasePropNode<T> :BindableBase where T: BasePropNode<T>
	{
		private readonly ObservableCollection<T> _parents = new();
		private readonly ObservableCollection<T> _children = new();

		/// <summary>
		/// Gets the collection of child nodes associated with this <see cref="BasePropNode{T}"/>.
		/// </summary>
		/// <remarks>
		/// The <see cref="Children"/> property provides access to the hierarchical structure of child nodes.
		/// It is recommended to use the provided methods in this class to manipulate the collection, 
		/// rather than modifying it directly.
		/// </remarks>
		/// <value>
		/// An <see cref="ObservableCollection{T}"/> containing the child nodes of type <typeparamref name="T"/>.
		/// </value>
		public ObservableCollection<T> Children => _children;

		/// <summary>
		/// Gets the collection of parent nodes associated with this node.
		/// </summary>
		/// <remarks>
		/// The <see cref="Parents"/> property provides access to the parent nodes of this node in the hierarchy.
		/// Modifications to this collection should be performed using the methods provided by this class
		/// to ensure consistency in the parent-child relationships.
		/// </remarks>
		/// <value>
		/// An <see cref="ObservableCollection{T}"/> containing the parent nodes of this node.
		/// </value>
		public ObservableCollection<T> Parents => _parents;

		#region Tree Management

		/// <summary>
		/// Removes the specified parent node from the list of parents of this node.
		/// </summary>
		/// <param name="parent">The parent node to be removed.</param>
		/// <returns>
		/// <c>true</c> if the parent node was successfully removed; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// This method also removes this node from the children of the specified parent node.
		/// </remarks>
		public bool RemoveParent(T parent)
		{
			bool success = _parents.Remove(parent);
			parent.RemoveChild((T)this);
			return success;
		}

		/// <summary>
		/// Adds the specified parent node to the list of parents of this node.
		/// </summary>
		/// <param name="parent">The parent node to be added.</param>
		/// <remarks>
		/// If the specified parent node is not already in the list of parents, it will be added.
		/// </remarks>
		public void AddParent(T parent)
		{
			if (!_parents.Contains(parent))
			{
				_parents.Add(parent);
			}
		}

		/// <summary>
		/// Adds a child <see cref="T"/> to the current node.
		/// </summary>
		/// <param name="node">The <see cref="T"/> to be added as a child.</param>
		/// <remarks>
		/// If the specified <paramref name="node"/> is not already a child of the current node, 
		/// it is added to the <see cref="Children"/> collection, and the current node is added 
		/// as a parent of the specified node.
		/// </remarks>
		public void AddChild(T node)
		{
			if (!_children.Contains(node))
			{
				_children.Add(node);
				node.AddParent((T)this);
			}
		}

		/// <summary>
		/// Removes the specified child node from the list of children of this node.
		/// </summary>
		/// <param name="node">The child node to be removed.</param>
		/// <returns>
		/// <c>true</c> if the child node was successfully removed; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// This method also removes this node from the parents of the specified child node.
		/// </remarks>
		public bool RemoveChild(T node)
		{
			if (_children.Remove(node))
			{
				node.RemoveParent((T)this);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Removes all child nodes from this node.
		/// </summary>
		/// <remarks>
		/// This method iterates through all child nodes, removes the current node as their parent, 
		/// and then clears the list of child nodes.
		/// </remarks>
		public void RemoveChildren()
		{
			foreach (var child in _children.ToList())
			{
				child.RemoveParent((T)this);
				_children.Remove(child);
			}
		}

		/// <summary>
		/// Inserts a child node at the specified index in the list of children of this node.
		/// </summary>
		/// <param name="index">The zero-based index at which the child node should be inserted.</param>
		/// <param name="node">The child node to be inserted.</param>
		/// <remarks>
		/// If the specified child node is not already in the list of children, it will be added at the specified index.
		/// This method also adds this node as a parent to the specified child node.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index"/> is less than 0 or greater than the number of children.
		/// </exception>
		public void InsertChild(int index, T node)
		{
			if (!_children.Contains(node))
			{
				_children.Insert(index, node);
				node.AddParent((T)this);
			}
		}

		/// <summary>
		/// Determines the zero-based index of the specified child node within the collection of child nodes.
		/// </summary>
		/// <param name="child">The child node to locate in the collection.</param>
		/// <returns>
		/// The zero-based index of the specified child node if found in the collection; otherwise, -1.
		/// </returns>
		/// <remarks>
		/// This method searches the collection of child nodes for the specified node and returns its index.
		/// If the node is not found, the method returns -1.
		/// </remarks>
		public int IndexOfChild(T child)
		{
			return _children.IndexOf(child);
		}

		#endregion

		/// <summary>
		/// Gets a value indicating whether this node is a root node.
		/// </summary>
		/// <value>
		/// <c>true</c> if this node has no parent nodes; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// A root node is defined as a node that does not have any parent nodes in the hierarchy.
		/// </remarks>
		public bool IsRootNode => !Parents.Any();
		
		
	}
}

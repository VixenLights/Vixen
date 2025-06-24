#nullable enable
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Vixen.Model;

namespace Vixen.Sys.Props
{
	public sealed class PropNode: BindableBase, IElementNode, IPropNode<PropNode, IProp>, IEqualityComparer<PropNode>
	{
		private readonly ObservableCollection<PropNode> _parents;
		private readonly ObservableCollection<PropNode> _children;
		private IProp? _prop;
		private string _name;
		private PropertyManager? _properties;

		#region Constructors

		public PropNode(IProp prop) : this(string.Empty)
		{
			_prop = prop;
		}

		public PropNode() : this("Prop 1")
		{
		}

		public PropNode(string name, PropNode parent) : this(name)
		{
			AddParent(parent);
		}

		public PropNode(IProp prop, PropNode parent) : this(string.Empty)
		{
			Prop = prop;
			AddParent(parent);
		}

		public PropNode(string name)
		{
			_name = name;
			_children = new ObservableCollection<PropNode>();
			_parents = new ObservableCollection<PropNode>();
		}

		#endregion

		#region Implementation of IEnumerable

		public IEnumerator<Element> GetEnumerator()
		{
			return GetElementEnumerator().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of IElementNode

		/// <summary>
		/// Unused in this implementation and will always be null
		/// </summary>
		public Element? Element { get; set; }

		#region Id

		public Guid Id { get; init; } = Guid.NewGuid();

		#endregion

		#region Name

		public string Name
		{
			get => Prop == null ? _name : Prop.Name;
			set
			{
				if (Prop == null)
				{
					SetProperty(ref _name, value);
				}
				else
				{
					_name = String.Empty;
					Prop.Name = value;
					OnPropertyChanged(nameof(Name));
				}

			}
		}

		#endregion

		IEnumerable<IElementNode> IElementNode.Children => _children;

		public int GetMaxChildDepth()
		{
			return GetDepth(this);
		}

		private int GetDepth(PropNode node)
		{
			if (node.IsProp)
			{
				return node.Prop!.TargetNode.GetMaxChildDepth();
			}
			var subLevel = node.Children.Select(GetDepth);
			return !subLevel.Any() ? 1 : subLevel.Max() + 1;
		}

		IEnumerable<IElementNode> IElementNode.GetLeafEnumerator()
		{
			if (IsProp)
			{
				return Prop!.TargetNode.GetLeafEnumerator();
			}

			return _children.SelectMany(x => (x as IElementNode).GetLeafEnumerator());
		}

		public IEnumerable<Element> GetElementEnumerator()
		{

			if (IsProp)
			{
				return Prop!.TargetNode.GetElementEnumerator();
			}
			else
			{
				return _children.SelectMany(x => x.GetElementEnumerator());
			}
		}

		public IEnumerable<IElementNode> GetNodeEnumerator()
		{
			if (IsProp)
			{
				return Prop!.TargetNode.GetNodeEnumerator();
			}

			return (new[] { this }).Concat(_children.SelectMany(x => x.GetNodeEnumerator()));
		}

		public IEnumerable<IElementNode> GetNonLeafEnumerator()
		{
			if (IsProp)
			{
				return Prop!.TargetNode.GetNonLeafEnumerator();
			}

			return (new[] { this }).Concat(_children.SelectMany(x => x.GetNonLeafEnumerator()));
		}

		IEnumerable<IElementNode> IElementNode.Parents => _parents;

		public PropertyManager Properties {
			get
			{
				//This may not get used very often, so defer creation
				if (_properties == null)
				{
					_properties = new(this);
				}

				return _properties;
			}
		}

		#region IsLeaf

		/// <summary>
		/// Gets a value indicating whether this node is a leaf node.
		/// </summary>
		/// <remarks>
		/// A node is considered a leaf if it represents an IProp <see cref="IProp"/> and its associated target node is also a leaf.
		/// This fulfills the IElementNode interface and represents the entire tree 
		/// </remarks>
		/// <value>
		/// <c>true</c> if this node is a leaf; otherwise, <c>false</c>.
		/// </value>
		bool IElementNode.IsLeaf => IsProp && Prop!.TargetNode.IsLeaf;

		#endregion

		public bool IsProxy => true;

		#endregion

		#region IPropNode

		#region Prop

		public IProp? Prop
		{
			get => _prop;
			set
			{
				if (_prop != null && _prop != value)
				{
					_prop.PropertyChanged -= Prop_PropertyChanged;
				}
				if (_prop != value && value != null)
				{
					value.PropertyChanged += Prop_PropertyChanged;
				}

				SetProperty(ref _prop, value);
			}
		}

		private void Prop_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			//Since we are swapping the name based on whether the Prop exists or not, we need to propagate the name changed event up
			//from the Prop when it occurs.
			if (nameof(IProp.Name).Equals(e.PropertyName))
			{
				OnPropertyChanged(nameof(Name));
			}
		}

		/// <summary>
		/// Gets the Prop with a null indicator.
		/// </summary>
		/// <param name="prop"><see cref="PropNode.Prop"/></param>
		/// <returns>True if not null</returns>
		public bool TryGetProp([NotNullWhen(true)] out IProp? prop)
		{
			prop = _prop;
			return prop is not null;
		}

		#endregion

		#region IsProp

		/// <summary>
		/// Gets a value indicating whether this <see cref="PropNode"/> represents a prop.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="PropNode"/> has an associated <see cref="IProp"/>; otherwise, <c>false</c>.
		/// </value>
		public bool IsProp => Prop != null;

		#endregion

		#region Grouping

		/// <summary>
		/// Gets a value indicating whether this node is a group node.
		/// </summary>
		/// <value>
		/// <c>true</c> if this node is a group node (i.e., it does not have an associated <see cref="IProp"/>); otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// A group node is a node that serves as a container for other nodes but does not directly represent a prop.
		/// </remarks>
		public bool IsGroupNode => Prop == null;

		/// <summary>
		/// Gets a value indicating whether group nodes can be added to this node.
		/// </summary>
		/// <value>
		/// <c>true</c> if this node is a group node and all its children are also group nodes, or it has no children; otherwise, <c>false</c>.
		/// </value>
		public bool CanAddGroupNodes => IsGroupNode && (!_children.Any() || _children.All(c => c.IsGroupNode));

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
		public bool IsRootNode => !_parents.Any();
		
		public IEnumerable<PropNode> GetIPropNodeLeafEnumerator()
		{
			return _children.SelectMany(x => x.GetIPropNodeLeafEnumerator());
		}

		/// <summary>
		/// Gets the collection of child <see cref="PropNode"/> instances associated with this node.
		/// Do not manipulate this collection directly. Instead, use the accessor methods in this class.
		/// </summary>
		/// <remarks>
		/// The <see cref="Children"/> collection represents the hierarchical structure of the prop nodes,
		/// allowing navigation and manipulation of child nodes within the tree.
		/// </remarks>
		/// <value>
		/// An <see cref="ObservableCollection{T}"/> of <see cref="PropNode"/> representing the child nodes.
		/// </value>
		public ObservableCollection<PropNode> Children => _children;

		/// <summary>
		/// Gets the collection of parent nodes associated with this node.
		/// Do not manipulate this collection directly. Instead, use the accessor methods in this class.
		/// </summary>
		/// <remarks>
		/// The <see cref="Parents"/> collection represents the parent nodes of this node in the hierarchy.
		/// Modifications to this collection will affect the parent-child relationships of this node.
		/// </remarks>
		/// <value>
		/// An <see cref="ObservableCollection{T}"/> of <see cref="PropNode"/> objects representing the parent nodes.
		/// </value>
		public ObservableCollection<PropNode> Parents => _parents;

		#endregion

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
		public bool RemoveParent(PropNode parent)
		{
			bool success = _parents.Remove(parent);
			parent.RemoveChild(this);
			return success;
		}

		/// <summary>
		/// Adds the specified parent node to the list of parents of this node.
		/// </summary>
		/// <param name="parent">The parent node to be added.</param>
		/// <remarks>
		/// If the specified parent node is not already in the list of parents, it will be added.
		/// </remarks>
		public void AddParent(PropNode parent)
		{
			if (!_parents.Contains(parent))
			{
				_parents.Add(parent);
			}
		}

		/// <summary>
		/// Adds a child <see cref="PropNode"/> to the current node.
		/// </summary>
		/// <param name="node">The <see cref="PropNode"/> to be added as a child.</param>
		/// <remarks>
		/// If the specified <paramref name="node"/> is not already a child of the current node, 
		/// it is added to the <see cref="Children"/> collection, and the current node is added 
		/// as a parent of the specified node.
		/// </remarks>
		public void AddChild(PropNode node)
		{
			if (!_children.Contains(node))
			{
				_children.Add(node);
				node.AddParent(this);
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
		public bool RemoveChild(PropNode node)
		{
			if (_children.Remove(node))
			{
				node.RemoveParent(this);
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
				child.RemoveParent(this);
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
		public void InsertChild(int index, PropNode node)
		{
			if (!_children.Contains(node))
			{
				_children.Insert(index, node);
				node.AddParent(this);
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
		public int IndexOfChild(PropNode child)
		{
			return _children.IndexOf(child);
		}

		#endregion

		public override string ToString()
		{
			return Name;
		}

		public bool Equals(PropNode? x, PropNode? y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}

			if (x is null)
			{
				return false;
			}

			if (y is null)
			{
				return false;
			}

			if (x.GetType() != y.GetType())
			{
				return false;
			}

			return x.Id.Equals(y.Id);
		}

		public int GetHashCode(PropNode obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}
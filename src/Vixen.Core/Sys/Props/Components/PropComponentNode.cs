#nullable enable
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace Vixen.Sys.Props.Components
{
	internal class PropComponentNode(string name) : BasePropNode<PropComponentNode>, IElementNode,
		IPropComponentNode<PropComponentNode, IPropComponent>, IEqualityComparer<PropComponentNode>
	{
		private IPropComponent? _propComponent;
		private string _name = name;
		private PropertyManager? _properties;

		#region Constructors

		public PropComponentNode(IPropComponent propComponent) : this(string.Empty)
		{
			_propComponent = propComponent;
		}

		public PropComponentNode() : this("Prop 1")
		{
		}

		public PropComponentNode(string name, PropComponentNode parent) : this(name)
		{
			AddParent(parent);
		}

		public PropComponentNode(IPropComponent propComponent, PropComponentNode parent) : this(string.Empty)
		{
			PropComponent = propComponent;
			AddParent(parent);
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
			get => PropComponent == null ? _name : PropComponent.Name;
			set
			{
				if (PropComponent == null)
				{
					SetProperty(ref _name, value);
				}
				else
				{
					_name = String.Empty;
					PropComponent.Name = value;
					OnPropertyChanged(nameof(Name));
				}

			}
		}

		#endregion

		#region PropComponent

		public IPropComponent? PropComponent
		{
			get => _propComponent;
			set
			{
				if (_propComponent != null && _propComponent != value)
				{
					_propComponent.PropertyChanged -= Prop_PropertyChanged;
				}
				if (_propComponent != value && value != null)
				{
					value.PropertyChanged += Prop_PropertyChanged;
				}

				SetProperty(ref _propComponent, value);
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
		/// Attempts to retrieve the associated <see cref="IPropComponent"/> of the current node.
		/// </summary>
		/// <param name="propComponent">
		/// When this method returns, contains the <see cref="IPropComponent"/> associated with the current node, 
		/// if one exists; otherwise, <see langword="null"/>.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="IPropComponent"/> is successfully retrieved; otherwise, <see langword="false"/>.
		/// </returns>
		public bool TryGetPropComponent([NotNullWhen(true)] out IPropComponent? propComponent)
		{
			propComponent = _propComponent;
			return propComponent is not null;
		}

		#endregion


		ObservableCollection<PropComponentNode> IPropComponentNode<PropComponentNode, IPropComponent>.Parents => Parents;

		ObservableCollection<PropComponentNode> IPropComponentNode<PropComponentNode, IPropComponent>.Children => Children;

		#region IsPropComponent

		/// <summary>
		/// Gets a value indicating whether this <see cref="PropComponentNode"/> represents a prop.
		/// </summary>
		/// <value>
		/// <c>true</c> if this <see cref="PropComponentNode"/> has an associated <see cref="IPropComponent"/>; otherwise, <c>false</c>.
		/// </value>
		public bool IsPropComponent => PropComponent != null;

		#endregion

		#region Grouping

		/// <summary>
		/// Gets a value indicating whether this node is a group node.
		/// </summary>
		/// <value>
		/// <c>true</c> if this node is a group node (i.e., it does not have an associated <see cref="IPropComponent"/>); otherwise, <c>false</c>.
		/// </value>
		/// <remarks>
		/// A group node is a node that serves as a container for other nodes but does not directly represent a prop.
		/// </remarks>
		public bool IsGroupNode => PropComponent == null;

		/// <summary>
		/// Gets a value indicating whether group nodes can be added to this node.
		/// </summary>
		/// <value>
		/// <c>true</c> if this node is a group node and all its children are also group nodes, or it has no children; otherwise, <c>false</c>.
		/// </value>
		public bool CanAddGroupNodes => IsGroupNode && (!Children.Any() || Children.All(c => c.IsGroupNode));

		#endregion

		public IEnumerable<PropComponentNode> GetIPropComponentNodeLeafEnumerator()
		{
			if (Children.Any())
			{
				return Children.SelectMany(x => x.GetIPropComponentNodeLeafEnumerator());
			}

			return [this];
		}

		IEnumerable<IElementNode> IElementNode.Children => Children;

		IEnumerable<IElementNode> IElementNode.Parents => Parents;

		#region IsLeaf

		/// <summary>
		/// Gets a value indicating whether this node is a leaf node.
		/// </summary>
		/// <remarks>
		/// A node is considered a leaf if it represents an IProp <see cref="IPropComponent"/> and its associated target node is also a leaf.
		/// This fulfills the IElementNode interface and represents the entire tree 
		/// </remarks>
		/// <value>
		/// <c>true</c> if this node is a leaf; otherwise, <c>false</c>.
		/// </value>
		bool IElementNode.IsLeaf => false; //TODO Need to revisit this depending on if we have a placeholder node in the real element tree for this proxy

		#endregion

		public int GetMaxChildDepth()
		{
			return GetDepth(this);
		}

		private int GetDepth(PropComponentNode node)
		{
			if (node.IsPropComponent)
			{
				//TODO determine if these will have multiple target nodes, or one proxy node
				//return node.PropComponent!.TargetNode.GetMaxChildDepth();
			}
			var subLevel = node.Children.Select(GetDepth);
			return !subLevel.Any() ? 1 : subLevel.Max() + 1;
		}

		public IEnumerable<IElementNode> GetLeafEnumerator()
		{
			if (IsPropComponent)
			{
				//TODO determine if these will have multiple target nodes, or one proxy node
				//return PropComponent!.TargetNode.GetLeafEnumerator();
			}

			return Children.SelectMany(x => (x as IElementNode).GetLeafEnumerator());
		}

		public IEnumerable<Element> GetElementEnumerator()
		{
			if (IsPropComponent)
			{
				//TODO determine if these will have multiple target nodes, or one proxy node
				//return PropComponent!.TargetNode.GetElementEnumerator();
			}

			return Children.SelectMany(x => x.GetElementEnumerator());
		}

		public IEnumerable<IElementNode> GetNodeEnumerator()
		{
			if (IsPropComponent)
			{
				//TODO determine if these will have multiple target nodes, or one proxy node
				//return PropComponent!.TargetNode.GetNodeEnumerator();
			}

			return (new[] { this }).Concat(Children.SelectMany(x => x.GetNodeEnumerator()));
		}

		public IEnumerable<IElementNode> GetNonLeafEnumerator()
		{
			if (IsPropComponent)
			{
				//TODO determine if these will have multiple target nodes, or one proxy node
				//return PropComponent!.TargetNode.GetNonLeafEnumerator();
			}

			return (new[] { this }).Concat(Children.SelectMany(x => x.GetNonLeafEnumerator()));
		}

		public PropertyManager Properties
		{
			//This may not get used very often, so defer creation
			get { return _properties ??= new(this); }
		}

		public bool IsProxy => true;

		#endregion

		#region Implementation of IEqualityComparer<in PropComponentNode>

		public bool Equals(PropComponentNode? x, PropComponentNode? y)
		{
			if (ReferenceEquals(x, y))
			{
				return true;
			}

			if (x is null || y is null)
			{
				return false;
			}

			return x.Id == y.Id && x.Name == y.Name && Equals(x.PropComponent, y.PropComponent);
		}

		public int GetHashCode(PropComponentNode? obj)
		{
			if (obj == null)
			{
				return 0;
			}

			var hashId = obj.Id.GetHashCode();
			var hashName = obj.Name?.GetHashCode() ?? 0;
			var hashPropComponent = obj.PropComponent?.GetHashCode() ?? 0;

			return HashCode.Combine(hashId, hashName, hashPropComponent);
		}

		#endregion
	}
}

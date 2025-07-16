#nullable enable
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Vixen.Sys.Props
{
	public sealed class PropNode(string name)
		: BasePropNode<PropNode>, IElementNode, IPropNode<PropNode, IProp>, IEqualityComparer<PropNode>
	{
		private IProp? _prop;
		private string _name = name;
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

		IEnumerable<IElementNode> IElementNode.Children => Children;

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

			return Children.SelectMany(x => (x as IElementNode).GetLeafEnumerator());
		}

		public IEnumerable<Element> GetElementEnumerator()
		{

			if (IsProp)
			{
				return Prop!.TargetNode.GetElementEnumerator();
			}

			return Children.SelectMany(x => x.GetElementEnumerator());
		}

		public IEnumerable<IElementNode> GetNodeEnumerator()
		{
			if (IsProp)
			{
				return Prop!.TargetNode.GetNodeEnumerator();
			}

			return new[] { this }.Concat(Children.SelectMany(x => x.GetNodeEnumerator()));
		}

		public IEnumerable<IElementNode> GetNonLeafEnumerator()
		{
			if (IsProp)
			{
				return Prop!.TargetNode.GetNonLeafEnumerator();
			}

			return (new[] { this }).Concat(Children.SelectMany(x => x.GetNonLeafEnumerator()));
		}

		IEnumerable<IElementNode> IElementNode.Parents => Parents;

		public PropertyManager Properties {
			//This may not get used very often, so defer creation
			get { return _properties ??= new(this); }
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
		public bool CanAddGroupNodes => IsGroupNode && (!Children.Any() || Children.All(c => c.IsGroupNode));

		#endregion
		
		public IEnumerable<PropNode> GetIPropNodeLeafEnumerator()
		{
			if (Children.Any())
			{
				return Children.SelectMany(x => x.GetIPropNodeLeafEnumerator());
			}

			return [this];
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
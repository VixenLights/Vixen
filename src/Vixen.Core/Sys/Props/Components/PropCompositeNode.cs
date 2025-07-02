#nullable enable
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Vixen.Model;

namespace Vixen.Sys.Props.Components
{
	public class PropCompositeNode: BindableBase, IElementNode, IEqualityComparer<PropCompositeNode>
	{
		private PropertyManager? _properties;
		private readonly Dictionary<Guid, IElementNode> _elementNodes = new();

		public PropCompositeNode()
		{
			ElementNode.Changed += ElementNode_Changed;
		}

		public IEnumerable<IElementNode> TargetNodes => _elementNodes.Values;

		internal void AddElementNodes(IEnumerable<IElementNode> nodes)
		{
			bool added = false;
			foreach (var elementNode in nodes)
			{
				if (_elementNodes.TryAdd(elementNode.Id, elementNode))
				{
					added = true;
				}
			}

			if (added)
			{
				OnPropertyChanged(nameof(TargetNodes));
			}
		}

		internal bool TryAdd(IElementNode node)
		{
			if (_elementNodes.TryAdd(node.Id, node))
			{
				OnPropertyChanged(nameof(TargetNodes));
				return true;
			}

			return false;
		}

		internal bool TryRemove(Guid id, [MaybeNullWhen(false)] out IElementNode node)
		{
			bool removed = _elementNodes.Remove(id, out node);
			if (removed)
			{
				OnPropertyChanged(nameof(TargetNodes));
			}

			return removed;

		}

		internal bool TryGet(Guid id, [MaybeNullWhen(false)] out IElementNode node)
		{
			return _elementNodes.TryGetValue(id, out node);
		}

		internal void Clear()
		{
			_elementNodes.Clear();
			OnPropertyChanged(nameof(TargetNodes));
		}

		private void ElementNode_Changed(object? sender, ElementNodeChangedEventArgs e)
		{
			if (_elementNodes.ContainsKey(e.Node.Id))
			{
				OnPropertyChanged(nameof(TargetNodes));
			}
		}

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
		/// This will always be null in this implementation
		/// </summary>
		public Element? Element { get; set; }

		public Guid Id { get; init; } = Guid.NewGuid(); 
		public string Name { get; set; } = String.Empty;
		public IEnumerable<IElementNode> Children => [];
		public IEnumerable<IElementNode> Parents => [];
		public bool IsLeaf => false;
		public int GetMaxChildDepth()
		{
			var subLevel = TargetNodes.Select(x => x.GetMaxChildDepth());
			return !subLevel.Any() ? 1 : subLevel.Max() + 1;
		}

		public IEnumerable<IElementNode> GetLeafEnumerator()
		{
			return TargetNodes.SelectMany(x => x.GetLeafEnumerator());
		}

		public IEnumerable<Element> GetElementEnumerator()
		{
			return TargetNodes.SelectMany(x => x.GetElementEnumerator());
		}

		public IEnumerable<IElementNode> GetNodeEnumerator()
		{
			return new[] { this }.Concat(TargetNodes.SelectMany(x => x.GetNodeEnumerator()));
		}

		public IEnumerable<IElementNode> GetNonLeafEnumerator()
		{
			return (new[] { this }).Concat(TargetNodes.SelectMany(x => x.GetNonLeafEnumerator()));
		}

		public PropertyManager Properties
		{
			//This may not get used very often, so defer creation
			get { return _properties ??= new(this); }
		}
		public bool IsProxy => true;

		#endregion

		#region IEqualityComparer

		public bool Equals(PropCompositeNode? x, PropCompositeNode? y)
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

		public int GetHashCode(PropCompositeNode obj)
		{
			return obj.Id.GetHashCode();
		}

		#endregion

	}
}
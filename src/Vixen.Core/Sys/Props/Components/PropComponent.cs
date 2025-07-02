#nullable enable
using System.Diagnostics.CodeAnalysis;
using Vixen.Model;

namespace Vixen.Sys.Props.Components
{
	public class PropComponent : BindableBase, IPropComponent
	{
		private readonly Dictionary<Guid, IElementNode> _elementNodes = new();
		private readonly PropCompositeNode _propCompositeNode = new();
		private string _name;

		internal PropComponent(string name, Guid ownerId, PropComponentType componentType)
		{
			_name = name;
			_propCompositeNode.Name = name;
			ComponentType = componentType;
			OwnerId = ownerId;
			ElementNode.Changed += ElementNode_Changed;
		}


		internal PropComponent(Guid ownerId): this("Component 1", ownerId, PropComponentType.PropDefined)
		{
			
		}

		internal PropComponent(PropComponentType componentType, Guid ownerId):this("Component 1", ownerId, componentType) 
		{
			
		}

		public Guid Id { get; init; } = Guid.NewGuid();
		
		public Guid OwnerId { get; init; }

		public string Name
		{
			get => _name;
			set
			{
				SetProperty(ref _name, value);
				_propCompositeNode.Name = _name;
			}
		}

		public PropComponentType ComponentType { get; set; }

		public IEnumerable<IElementNode> TargetNodes => _elementNodes.Values;

		public void AddElementNodes(IEnumerable<IElementNode> nodes)
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

		public bool TryAdd(IElementNode node)
		{
			if (_elementNodes.TryAdd(node.Id, node))
			{
				OnPropertyChanged(nameof(TargetNodes));
				return true;
			}

			return false;
		}

		public bool TryRemove(Guid id, [MaybeNullWhen(false)] out IElementNode node)
		{
			bool removed = _elementNodes.Remove(id, out node);
			if (removed)
			{
				OnPropertyChanged(nameof(TargetNodes));
			}

			return removed;

		}

		public bool TryGet(Guid id, [MaybeNullWhen(false)] out IElementNode node)
		{
			return _elementNodes.TryGetValue(id, out node);
		}

		public void Clear()
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

		public bool IsUserDefined => ComponentType == PropComponentType.UserDefined;

	}
}
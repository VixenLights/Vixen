#nullable enable
using System.Diagnostics.CodeAnalysis;
using Vixen.Model;

namespace Vixen.Sys.Props.Components
{
	public class PropComponent : BindableBase, IPropComponent
	{
		private readonly Dictionary<Guid, IElementNode> _elementNodes = new();
		private string _name;

		public PropComponent(string name, PropComponentType componentType)
		{
			_name = name;
			ComponentType = componentType;
			ElementNode.Changed += ElementNode_Changed;
		}

		private void ElementNode_Changed(object? sender, ElementNodeChangedEventArgs e)
		{
			if (_elementNodes.ContainsKey(e.Node.Id))
			{
				OnPropertyChanged(nameof(TargetNodes));
			}
		}

		public PropComponent(): this("Component 1", PropComponentType.PropDefined)
		{
			
		}

		public PropComponent(PropComponentType componentType):this("Component 1", componentType) 
		{
			
		}

		public Guid Id { get; init; } = Guid.NewGuid();

		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		public PropComponentType ComponentType { get; init; }

		public IEnumerable<IElementNode> TargetNodes
		{
			get
			{
				return _elementNodes.Values;
			}
		}

		public void AddElementNodes(IEnumerable<IElementNode> nodes)
		{
			bool added = false;
			foreach (var elementNode in nodes)
			{
				if(_elementNodes.TryAdd(elementNode.Id, elementNode))
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

		public bool TryGet(Guid id, out IElementNode node)
		{
			return _elementNodes.TryGetValue(id, out node);
		}

		public void Clear()
		{
			_elementNodes.Clear();
			OnPropertyChanged(nameof(TargetNodes));
		}

		public bool IsUserDefined => ComponentType == PropComponentType.UserDefined;

	}
}
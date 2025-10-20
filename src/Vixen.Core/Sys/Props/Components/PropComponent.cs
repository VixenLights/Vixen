#nullable enable
using System.Diagnostics.CodeAnalysis;
using Vixen.Model;

namespace Vixen.Sys.Props.Components
{
	public class PropComponent : BindableBase, IPropComponent
	{
		// These two Lists must remain in lock-step, index-wise. We use these instead of a Dictionary
		// because a Dictionary is not guaranteed to remain in the same order as elements are appended and/or removed.
		private readonly List<Guid> _elementGuid = new();
		private readonly List<IElementNode> _elementNodeList = new();

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

		public IEnumerable<IElementNode> TargetNodes => _elementNodeList;

		public void AddElementNodes(IEnumerable<IElementNode> nodes)
		{
			bool added = false;
			foreach (var elementNode in nodes)
			{
				if (TryAdd(elementNode, false))
				{
					added = true;
				}
			}

			if (added)
			{
				OnPropertyChanged(nameof(TargetNodes));
			}
		}

		public bool TryAdd(IElementNode node, bool notifyChange = true)
		{
			if (!_elementGuid.Contains(node.Id))
			{
				_elementGuid.Add(node.Id);
				_elementNodeList.Add(node);
				if (notifyChange)
				{
					OnPropertyChanged(nameof(TargetNodes));
				}
				return true;
			}

			return false;
		}

		public bool TryRemove(Guid id, [MaybeNullWhen(false)] out IElementNode node)
		{
			int index = _elementGuid.IndexOf(id);
			if (index != -1)
			{
				node = _elementNodeList[index];
				_elementGuid.RemoveAt(index);
				_elementNodeList.RemoveAt(index);
				OnPropertyChanged(nameof(TargetNodes));
			}
			else
			{
				node = null;
			}

			return index != -1;
		}

		public bool TryGet(Guid id, [MaybeNullWhen(false)] out IElementNode node)
		{
			int index = _elementGuid.IndexOf(id);
			if (index != -1)
			{
				node = _elementNodeList[index];
			}
			else
			{
				node = null;
			}

			return index != -1;
		}

		public void Clear()
		{
			_elementGuid.Clear();
			_elementNodeList.Clear();
			OnPropertyChanged(nameof(TargetNodes));
		}

		private void ElementNode_Changed(object? sender, ElementNodeChangedEventArgs e)
		{
			if (_elementGuid.Contains(e.Node.Id))
			{
				OnPropertyChanged(nameof(TargetNodes));
			}
		}

		public bool IsUserDefined => ComponentType == PropComponentType.UserDefined;

	}
}
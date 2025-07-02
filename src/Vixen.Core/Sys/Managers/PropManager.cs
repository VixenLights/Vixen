#nullable enable
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Vixen.Model;
using Vixen.Sys.Props;

namespace Vixen.Sys.Managers
{
	public class PropManager : BindableBase
	{
		private readonly Dictionary<Guid, IProp> _propLocator = new();
		private readonly PropNode _rootNode = new("Props");
		
		public event EventHandler? PropCollectionChanged;

		public PropNode RootNode
		{
			get => _rootNode;
			init
			{
				_rootNode = value;
				OnPropertyChanged(nameof(RootNode));
			}
		}

		public ObservableCollection<PropNode> RootNodes
		{
			get => RootNode.Children;
		}

		public void CreateGroupForPropNodes(string name, IEnumerable<PropNode> propNodes)
		{
			var propNode = CreatePropNode(name);
			foreach (var elementModel in propNodes)
			{
				propNode.AddChild(elementModel);
				elementModel.AddParent(propNode);
			}
		}

		public PropNode CreatePropNode(string name, PropNode? parent = null, bool oneBasedNaming = false)
		{
			if (parent == null)
			{
				parent = RootNode;
			}

			PropNode pn = new PropNode(name, parent);
			parent.AddChild(pn);
			return pn;
		}

		public PropNode AddProp(IProp prop, PropNode? parent)
		{

			PropNode propNode;
			if (parent == null)
			{
				propNode = new PropNode(prop, RootNode);
				RootNode.AddChild(propNode);
			}
			else
			{
				propNode = new PropNode(prop, parent);
				parent.AddChild(propNode);
			}

			OnPropAdded(prop);

			return propNode;
		}

		public void RemoveFromParent(PropNode propNode, PropNode parentToLeave)
		{
			//Remove us from our specified parent.
			propNode.RemoveParent(parentToLeave);
			if (!propNode.Parents.Any())
			{
				//We no longer have any parents, so clean up the props in our tree.
				var leafs = propNode.GetIPropNodeLeafEnumerator();
				foreach (var leaf in leafs)
				{
					if (leaf.TryGetProp(out var prop))
					{
						_propLocator.Remove(prop.Id);
						OnPropRemoved(prop);
						prop.CleanUp();
					}
				}
			}
		}

		public T CreateProp<T>(string name) where T : IProp, new()
		{
			var prop = new T
			{
				Name = name
			};

			_propLocator.Add(prop.Id, prop);

			return prop;
		}

		public IProp? FindById(Guid id)
		{
			return _propLocator.GetValueOrDefault(id);
		}

		private void OnPropAdded(IProp prop)
		{
			PropCollectionChanged?.Invoke(this, new PropCollectionEventArgs(prop, NotifyCollectionChangedAction.Add));
		}

		private void OnPropRemoved(IProp prop)
		{
			PropCollectionChanged?.Invoke(this, new PropCollectionEventArgs(prop, NotifyCollectionChangedAction.Remove));
		}

		public class PropCollectionEventArgs(IProp prop, NotifyCollectionChangedAction action) : EventArgs
		{
			public IProp Prop { get; init; } = prop;

			public NotifyCollectionChangedAction Action { get; init; } = action;
		}
	}
}

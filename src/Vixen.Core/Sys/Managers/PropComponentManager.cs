#nullable enable
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Vixen.Model;
using Vixen.Sys.Props.Components;

namespace Vixen.Sys.Managers
{
	public class PropComponentManager : BindableBase
	{
		//private static readonly Dictionary<Guid, IPropComponent> PropComponentLocator = new();
		private readonly PropComponentNode _rootNode = new("Prop Components");
		
		public event EventHandler? PropComponentCollectionChanged;

		public PropComponentNode RootNode
		{
			get => _rootNode;
			init
			{
				_rootNode = value;
				OnPropertyChanged(nameof(RootNode));
			}
		}

		public ObservableCollection<PropComponentNode> RootNodes
		{
			get => RootNode.Children;
		}

		public void CreateGroupForNodes(string name, IEnumerable<PropComponentNode> propNodes)
		{
			var propNode = CreatePropComponentNode(name);
			foreach (var elementModel in propNodes)
			{
				propNode.AddChild(elementModel);
				elementModel.AddParent(propNode);
			}
		}

		public PropComponentNode CreatePropComponentNode(string name, PropComponentNode? parent = null, bool oneBasedNaming = false)
		{
			if (parent == null)
			{
				parent = RootNode;
			}

			PropComponentNode pn = new PropComponentNode(name, parent);
			parent.AddChild(pn);
			return pn;
		}

		public PropComponentNode AddPropComponent(IPropComponent propComponent, PropComponentNode? parent)
		{

			PropComponentNode propNode;
			if (parent == null)
			{
				propNode = new PropComponentNode(propComponent, RootNode);
				RootNode.AddChild(propNode);
			}
			else
			{
				propNode = new PropComponentNode(propComponent, parent);
				parent.AddChild(propNode);
			}

			OnPropComponentAdded(propComponent);

			return propNode;
		}

		public void RemoveFromParent(PropComponentNode propNode, PropComponentNode parentToLeave)
		{
			//Remove us from our specified parent.
			propNode.RemoveParent(parentToLeave);
			if (!propNode.Parents.Any())
			{
				//We no longer have any parents, so clean up the props in our tree.
				var leafs = propNode.GetIPropComponentNodeLeafEnumerator();
				foreach (var leaf in leafs)
				{
					if (leaf.TryGetPropComponent(out var prop))
					{
						//PropComponentLocator.Remove(prop.Id);
						OnPropRemoved(prop);
						//prop.CleanUp();
					}
				}
			}
		}

		public static IPropComponent CreatePropComponent(string name, Guid ownerId, PropComponentType type)
		{
			var propComponent = new PropComponent(name, ownerId, type);

			//PropComponentLocator.Add(propComponent.Id, propComponent);

			return propComponent;
		}

		//public IPropComponent? FindById(Guid id)
		//{
		//	return PropComponentLocator.GetValueOrDefault(id);
		//}

		private void OnPropComponentAdded(IPropComponent propComponent)
		{
			PropComponentCollectionChanged?.Invoke(this, new PropComponentCollectionEventArgs(propComponent, NotifyCollectionChangedAction.Add));
		}

		private void OnPropRemoved(IPropComponent propComponent)
		{
			PropComponentCollectionChanged?.Invoke(this, new PropComponentCollectionEventArgs(propComponent, NotifyCollectionChangedAction.Remove));
		}

		public class PropComponentCollectionEventArgs(IPropComponent propComponent, NotifyCollectionChangedAction action) : EventArgs
		{
			public IPropComponent PropComponent { get; init; } = propComponent;

			public NotifyCollectionChangedAction Action { get; init; } = action;
		}
	}
}

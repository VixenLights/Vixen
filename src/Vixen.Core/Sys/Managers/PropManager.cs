#nullable enable
using System.Collections.ObjectModel;
using Vixen.Annotations;
using Vixen.Model;
using Vixen.Sys.Props;

namespace Vixen.Sys.Managers
{
	public class PropManager: BindableBase
    {
        private readonly Dictionary<Guid, IProp> _propLocator = new Dictionary<Guid, IProp>();
        private readonly PropNode _rootNode;

        public PropManager()
        {
            RootNode = new PropNode("Props");
        }

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
            set
            {
                RootNode.Children = value;
                OnPropertyChanged(nameof(RootNodes));
			} 
        }

        public void CreateGroupForPropNodes(string name, IEnumerable<PropNode> propNodes)
        {
            var em = CreateNode(name);
            foreach (var elementModel in propNodes)
            {
                em.Children.Add(elementModel);
                elementModel.Parents.Add(em.Id);
            }
        }

        public PropNode CreateNode(string name, PropNode parent = null, bool oneBasedNaming = false)
        {
            if (parent == null)
            {
                parent = RootNode;
            }

            PropNode pn = new PropNode(name, parent);
            parent.AddChild(pn);
            return pn;
        }

        public PropNode AddProp(IProp prop, PropNode parent)
        {

            PropNode propNode = null;
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

			return propNode;
        }

        public void RemoveFromParent(PropNode propNode, PropNode parentToLeave)
        {
            propNode.RemoveParent(parentToLeave);
            parentToLeave.RemoveChild(propNode);
        }

        public T CreateProp<T>(string name) where T: IProp, new()
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
	}
}

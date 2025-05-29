#nullable enable
using System.Collections.ObjectModel;
using Vixen.Model;

namespace Vixen.Sys.Props
{
    [Serializable]
	public class PropNode: BindableBase, IEqualityComparer<PropNode>, IEquatable<PropNode>
    {
        private ObservableCollection<PropNode> _children;
        private ObservableCollection<Guid> _parents;
        private IProp? _prop;
        private string _name;

        #region Constructors

        public PropNode(IProp prop):this(string.Empty)
        {
            Prop = prop;
        }

        public PropNode() : this("Prop 1")
        {
        }

        public PropNode(string name)
        {
			Name = name;
            Children = new ObservableCollection<PropNode>();
            Parents = new ObservableCollection<Guid>();
		}

        public PropNode(string name, PropNode parent) : this(name)
        {
            Parents.Add(parent.Id);
        }

        public PropNode(IProp prop, PropNode parent):this(string.Empty)
        {
            Prop = prop;
            Parents.Add(parent.Id);
        }

		#endregion

		#region Properties

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

		#region Prop

		public IProp? Prop
        {
            get => _prop;
            set => SetProperty(ref _prop, value);
        }

		#endregion

		#endregion

		#region IsLeaf

		public bool IsLeaf => !Children.Any();

        #endregion

		#region Parents

		public ObservableCollection<Guid> Parents
        {
            get { return _parents; }
            set
            {
                if (Equals(value, _parents)) return;
                _parents = value;
                OnPropertyChanged(nameof(Parents));
            }
        }

		#endregion

		#region Children

        public ObservableCollection<PropNode> Children
        {
            get { return _children; }
            set
            {
                if (Equals(value, _children)) return;
                _children = value;
                OnPropertyChanged(nameof(Children));
                OnPropertyChanged(nameof(IsLeaf));
                OnPropertyChanged(nameof(IsGroupNode));
            }
        }

		#endregion

		#region Node Info

        public bool IsGroupNode => Prop == null;

        public bool CanAddGroupNodes => IsGroupNode && (!Children.Any() || Children.Any(c => c.IsGroupNode));

        public bool CanAddLeafNodes => IsGroupNode && (!Children.Any() || Children.Any(c => c.IsLeaf));

        public bool IsRootNode => !Parents.Any();

        #endregion

		#region Tree Management

		public bool RemoveParent(PropNode parent)
        {
            bool success = Parents.Remove(parent.Id);
            parent.RemoveChild(this);
            if (!Parents.Any())
            {
                //We are now orphaned and need to clean up
                if (IsLeaf)
                {
                    //We are at the bottom and just need to remove our lights
                    Prop = null;
                    OnPropertyChanged(nameof(Prop));
                }
                else
                {
                    foreach (var child in Children.ToList())
                    {
                        child.RemoveParent(this);
                    }
                }
            }

            return success;
        }

        public void AddParent(PropNode parent)
        {
            Parents.Add(parent.Id);
        }

        public void AddChild(PropNode em)
        {
            Children.Add(em);
        }

        public bool RemoveChild(PropNode child)
        {
            var status = Children.Remove(child);
            return status;
        }

		#endregion

		#region Enumerators


		public IEnumerable<PropNode> GetNodeEnumerator()
        {
            return (new[] { this }).Concat(Children.SelectMany(x => x.GetNodeEnumerator()));
        }

        public IEnumerable<PropNode> GetLeafEnumerator()
        {
            if (IsLeaf)
            {
                return [this];
            }

            return Children.SelectMany(x => x.GetLeafEnumerator());
        }

        public IEnumerable<PropNode> GetNonLeafEnumerator()
        {
            if (IsLeaf)
            {
                return [];
            }

            return (new[] { this }).Concat(Children.SelectMany(x => x.GetNonLeafEnumerator()));
        }

		#endregion

		#region Equals

		public bool Equals(PropNode? x, PropNode? y)
        {
            return y != null && x != null && x.Id == y.Id;
        }

		public int GetHashCode(PropNode obj)
        {
            return obj.Id.GetHashCode();
        }

        public bool Equals(PropNode? other)
        {
            return other != null && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

		#endregion

	}
}

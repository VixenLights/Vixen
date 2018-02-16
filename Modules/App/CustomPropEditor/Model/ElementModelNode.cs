using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.Model
{
    /// <summary>
    /// Symbolic of a ElementModelNode in Vixen core
    /// </summary>
	public class ElementModelNode: BindableGroupNode<ElementModel>, IEqualityComparer<ElementModelNode>
    {
        public static event EventHandler Changed;
        private ElementModel _elementModel;
        private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

        internal ElementModelNode(Guid id, string name, ElementModel element, IEnumerable<ElementModelNode> content)
            : base(name, content)
        {
            if (PropModelServices.ElementModelNodes.ElementModelNodeExists(id))
            {
                throw new InvalidOperationException("Trying to create a ElementModelNode that already exists.");
            }

            PropModelServices.ElementModelNodes.SetElementModelNode(id, this);
            Id = id;
            if (element != null)
            {
                //Ensure the element name matches the ElementModelNode name as there is some code that expects that.
                element.Name = name;
            }
            ElementModel = element;
           
        }

        internal ElementModelNode(string name, ElementModel element, IEnumerable<ElementModelNode> content)
	        : this(Guid.NewGuid(), name, element, content)
	    {
	    }

	    internal ElementModelNode(string name, IEnumerable<ElementModelNode> content)
	        : this(name, null, content)
	    {
	    }

	    private ElementModelNode(Guid id, string name, ElementModel element, params ElementModelNode[] content)
	        : this(id, name, element, content as IEnumerable<ElementModelNode>)
	    {
	    }

	    internal ElementModelNode(string name, ElementModel element, params ElementModelNode[] content)
	        : this(name, element, content as IEnumerable<ElementModelNode>)
	    {
	    }

	    internal ElementModelNode(string name, params ElementModelNode[] content)
	        : this(name, null, content)
	    {
	    }

        public Guid Id { get; set; }

        public ElementModel ElementModel {

            get { return _elementModel; }
            set
            {
                if (_elementModel != null)
                {
                    PropModelServices.ElementModels.RemoveElement(_elementModel);
                }

                _elementModel = value;

                if (_elementModel != null)
                {
                    // this Element should be unique to this ElementNode. If it already exists in the element -> ElementNode
                    // mapping in the Element Manager, something Very Bad (tm) has happened.
                    if (PropModelServices.ElementModels.GetElementNodeForElement(value) != null)
                    {
                        Logging.Error(
                            $"ElementNode: assigning element (id: {value.Id}) to this ElementNode (id: {Id}), but it already exists in another ElementNode! (id: {PropModelServices.ElementModels.GetElementNodeForElement(value).Id})");

                    }

                    PropModelServices.ElementModels.SetElementNodeForElement(_elementModel, this);
                }
            }
        }


        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                if (_elementModel != null)
                {
                    _elementModel.Name = value;
                }
                OnPropertyChanged(nameof(Name));
            }
        }

        public new ElementModelNode Find(string childName)
        {
            return base.Find(childName) as ElementModelNode;
        }

        public new IEnumerable<ElementModelNode> Children => base.Children.Cast<ElementModelNode>();

        public new IEnumerable<ElementModelNode> Parents => base.Parents.Cast<ElementModelNode>();

        public bool IsLeaf => !base.Children.Any();

        /// <summary>
        /// Finds all nodes that would be considered invalid children for this node. This is effectively the
        /// node itself, and any parent nodes it has. It also includes any immediate child nodes.
        /// </summary>
        /// <returns>An enumeration of invalid child nodes for this node.</returns>
        public IEnumerable<ElementModelNode> InvalidChildren()
        {
            HashSet<ElementModelNode> result = new HashSet<ElementModelNode>();

            // the node itself is an invalid child for itself!
            result.Add(this);

            // any children it already has are invalid.
            result.AddRange(Children);

            // any parents it has (all the way back to root) are invalid,
            // otherwise that will create loops.
            result.AddRange(GetAllParentNodes());

            return result;
        }

        #region Overrides

        //public override void AddChild(BindableGroupNode<ElementModel> node)
        //{
        //    base.AddChild(node);
        //   // OnChanged(this);
        //}

        //public override void InsertChild(int index, BindableGroupNode<ElementModel> node)
        //{
        //    base.InsertChild(index, node);
        //    //OnChanged(this);
        //}

        public override bool RemoveFromParent(BindableGroupNode<ElementModel> parent, bool cleanup)
        {
            bool result = base.RemoveFromParent(parent, cleanup);

            // if we're cleaning up after removal (eg. being deleted), and we're actually floating
            // (ie. this node doesn't exist anywhere else), remove the associated element (if any)
            if (cleanup && !Parents.Any())
            {
                if (ElementModel != null)
                {
                    PropModelServices.ElementModels.RemoveElement(ElementModel);
                    ElementModel = null;
                }
                VixenSystem.Nodes.ClearElementNode(Id);
            }

           // OnChanged(this);
            return result;
        }

        public override bool RemoveChild(BindableGroupNode<ElementModel> node)
        {
            bool result = base.RemoveChild(node);
            //OnChanged(this);
            return result;
        }

        public override BindableGroupNode<ElementModel> Get(int index)
        {
            if (IsLeaf) throw new InvalidOperationException("Cannot get child nodes from a leaf.");
            return base.Get(index);
        }

        public override IEnumerator<ElementModel> GetEnumerator()
        {
            return GetElementEnumerator().GetEnumerator();
        }

        #endregion

        #region Enumerators

        public IEnumerable<ElementModel> GetElementEnumerator()
        {
            if (IsLeaf)
            {
                // Element is already an enumerable, so AsEnumerable<> won't work.
                return (new[] { ElementModel });
            }

            return Children.SelectMany(x => x.GetElementEnumerator());
        }

        public IEnumerable<ElementModelNode> GetNodeEnumerator()
        {
            // "this" is already an enumerable, so AsEnumerable<> won't work.
            return (new[] { this }).Concat(Children.SelectMany(x => x.GetNodeEnumerator()));
        }

        public IEnumerable<ElementModelNode> GetLeafEnumerator()
        {
            if (IsLeaf)
            {
                // Element is already an enumerable, so AsEnumerable<> won't work.
                return (new[] { this });
            }
            else
            {
                return Children.SelectMany(x => x.GetLeafEnumerator());
            }
        }

        public IEnumerable<ElementModelNode> GetNonLeafEnumerator()
        {
            if (IsLeaf)
            {
                return Enumerable.Empty<ElementModelNode>();
            }
            else
            {
                // "this" is already an enumerable, so AsEnumerable<> won't work.
                return (new[] { this }).Concat(Children.SelectMany(x => x.GetNonLeafEnumerator()));
            }
        }

        public IEnumerable<ElementModelNode> GetAllParentNodes()
        {
            return Parents.Concat(Parents.SelectMany(x => x.GetAllParentNodes()));
        }

        public int GetMaxChildDepth()
        {
            return GetDepth(this);
        }

        private int GetDepth(ElementModelNode node)
        {
            if (node.IsLeaf)
            {
                return 1;
            }
            var subLevel = node.Children.Select(GetDepth);
            return !subLevel.Any() ? 1 : subLevel.Max() + 1;
        }

        #endregion


        public bool Equals(ElementModelNode x, ElementModelNode y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ElementModelNode obj)
        {
            return Id.GetHashCode();
        }

        #region Static members

        protected static void OnChanged(ElementNode value)
        {
            Changed?.Invoke(value, EventArgs.Empty);
        }

        #endregion
    }
}

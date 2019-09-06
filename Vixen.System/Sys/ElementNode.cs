using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys
{
	/// <summary>
	/// A logical node that encapsulates a single Element or a branch/group of other ElementNodes.
	/// </summary>
	[Serializable]
	public class ElementNode : GroupNode<Element>, IElementNode, IEqualityComparer<ElementNode>
	{
		// Making this static so there doesn't have to be potentially thousands of
		// subscriptions from the node manager.
		public static event EventHandler Changed;
		//Logger Class
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		#region Constructors

		internal ElementNode(Guid id, string name, Element element, IEnumerable<ElementNode> content)
			: base(name, content)
		{
			if (VixenSystem.Nodes.ElementNodeExists(id)) {
				throw new InvalidOperationException("Trying to create a ElementNode that already exists.");
			}
			else {
				VixenSystem.Nodes.SetElementNode(id, this);
			}
			Id = id;
			if (element != null)
			{
				//Ensure the element name matches the ElementNode name as there is some code that expects that.
				element.Name = name;
			}
			Element = element;
			Properties = new PropertyManager(this);
		}

		internal ElementNode(string name, Element element, IEnumerable<ElementNode> content)
			: this(Guid.NewGuid(), name, element, content)
		{
		}

		internal ElementNode(string name, IEnumerable<ElementNode> content)
			: this(name, null, content)
		{
		}

		private ElementNode(Guid id, string name, Element element, params ElementNode[] content)
			: this(id, name, element, content as IEnumerable<ElementNode>)
		{
		}

		internal ElementNode(string name, Element element, params ElementNode[] content)
			: this(name, element, content as IEnumerable<ElementNode>)
		{
		}

		internal ElementNode(string name, params ElementNode[] content)
			: this(name, null, content)
		{
		}

		#endregion

		private Element _element;

		public Element Element
		{
			get { return _element; }
			set
			{
				if (_element != null) {
					VixenSystem.Elements.RemoveElement(_element);
				}

				_element = value;

				if (_element != null) {
					// this Element should be unique to this ElementNode. If it already exists in the element -> ElementNode
					// mapping in the Element Manager, something Very Bad (tm) has happened.
					if (VixenSystem.Elements.GetElementNodeForElement(value) != null) {
						Logging.Error(string.Format("ElementNode: assigning element (id: {0}) to this ElementNode (id: {1}), but it already exists in another ElementNode! (id: {2})", value.Id, Id, VixenSystem.Elements.GetElementNodeForElement(value).Id));

					}

					VixenSystem.Elements.SetElementNodeForElement(_element, this);
				}
			}
		}

		public Guid Id { get; private set; }

		public override string Name
		{
			get { return base.Name; }
			set
			{
				base.Name = value;
				if (_element != null) {
					_element.Name = value;
				}
			}
		}

		public new ElementNode Find(string childName)
		{
			return base.Find(childName) as ElementNode;
		}

		public new IEnumerable<ElementNode> Children
		{
			get { return base.Children.Cast<ElementNode>(); }
		}
		IEnumerable<IElementNode> IElementNode.Children => Children;

		public new IEnumerable<ElementNode> Parents
		{
			get { return base.Parents.Cast<ElementNode>(); }
		}

		IEnumerable<IElementNode> IElementNode.Parents => Parents;

		public bool Masked
		{
			get { return this.All(x => x.Masked); }
			set
			{
				foreach (Element element in this) {
					element.Masked = value;
				}
			}
		}

		public bool IsLeaf
		{
			get { return !base.Children.Any(); }
		}

		/// <summary>
		/// Finds all nodes that would be considered invalid children for this node. This is effectively the
		/// node itself, and any parent nodes it has. It also includes any immediate child nodes.
		/// </summary>
		/// <returns>An enumeration of invalid child nodes for this node.</returns>
		public IEnumerable<ElementNode> InvalidChildren()
		{
			HashSet<ElementNode> result = new HashSet<ElementNode>();

			// the node itself is an invalid child for itself!
			result.Add(this);

			// any children it already has are invalid.
			result.AddRange(Children);

			// any parents it has (all the way back to root) are invalid,
			// otherwise that will create loops.
			result.AddRange(GetAllParentNodes());

			return result;
		}

		public PropertyManager Properties { get; private set; }

		/// <inheritdoc />
		public bool IsProxy => false;

		#region Overrides

		public override void AddChild(GroupNode<Element> node)
		{
			base.AddChild(node);
			OnChanged(this);
		}

		public override void InsertChild(int index, GroupNode<Element> node)
		{
			base.InsertChild(index, node);
			OnChanged(this);
		}

		public override bool RemoveFromParent(GroupNode<Element> parent, bool cleanup)
		{
			bool result = base.RemoveFromParent(parent, cleanup);

			// if we're cleaning up after removal (eg. being deleted), and we're actually floating
			// (ie. this node doesn't exist anywhere else), remove the associated element (if any)
			if (cleanup && !Parents.Any()) {
				if( Element != null) {
					VixenSystem.Elements.RemoveElement(Element);
					foreach (var source in Properties.ToArray())
					{
						Properties.Remove(source.Descriptor.TypeId);
					}
					Element = null;
				}
				VixenSystem.Nodes.ClearElementNode(Id);
			}

			OnChanged(this);
			return result;
		}

		public override bool RemoveChild(GroupNode<Element> node)
		{
			bool result = base.RemoveChild(node);
			OnChanged(this);
			return result;
		}

		public override GroupNode<Element> Get(int index)
		{
			if (IsLeaf) throw new InvalidOperationException("Cannot get child nodes from a leaf.");
			return base.Get(index);
		}

		public override IEnumerator<Element> GetEnumerator()
		{
			return GetElementEnumerator().GetEnumerator();
		}

		#endregion

		#region Enumerators

		public IEnumerable<Element> GetElementEnumerator()
		{
			if (IsLeaf) {
				// Element is already an enumerable, so AsEnumerable<> won't work.
				return (new[] {Element});
			}
			else {
				return this.Children.SelectMany(x => x.GetElementEnumerator());
			}
		}

		IEnumerable<Element> IElementNode.GetElementEnumerator()
		{
			return GetElementEnumerator();
		}

		public IEnumerable<ElementNode> GetNodeEnumerator()
		{
			// "this" is already an enumerable, so AsEnumerable<> won't work.
			return (new[] {this}).Concat(Children.SelectMany(x => x.GetNodeEnumerator()));
		}

		IEnumerable<IElementNode> IElementNode.GetNodeEnumerator()
		{
			return GetNodeEnumerator();
		}

		public IEnumerable<ElementNode> GetLeafEnumerator()
		{
			if (IsLeaf) {
				// Element is already an enumerable, so AsEnumerable<> won't work.
				return (new[] {this});
			}
			else {
				return Children.SelectMany(x => x.GetLeafEnumerator());
			}
		}

		IEnumerable<IElementNode> IElementNode.GetLeafEnumerator()
		{
			return GetLeafEnumerator();
		}

		public IEnumerable<ElementNode> GetNonLeafEnumerator()
		{
			if (IsLeaf) {
				return Enumerable.Empty<ElementNode>();
			}
			else {
				// "this" is already an enumerable, so AsEnumerable<> won't work.
				return (new[] {this}).Concat(Children.SelectMany(x => x.GetNonLeafEnumerator()));
			}
		}

		IEnumerable<IElementNode> IElementNode.GetNonLeafEnumerator()
		{
			return GetNonLeafEnumerator();
		}

		public IEnumerable<ElementNode> GetAllParentNodes()
		{
			return Parents.Concat(Parents.SelectMany(x => x.GetAllParentNodes()));
		}

		public int GetMaxChildDepth()
		{
			return GetDepth(this);
		}

		private int GetDepth(ElementNode node)
		{
			if (node.IsLeaf)
			{
				return 1;
			}
			var subLevel = node.Children.Select(GetDepth);
			return !subLevel.Any() ? 1 : subLevel.Max() + 1;
		}

		#endregion

		#region Static members

		protected static void OnChanged(ElementNode value)
		{
		    Changed?.Invoke(value, EventArgs.Empty);
		}

		#endregion

		public bool Equals(ElementNode x, ElementNode y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(ElementNode obj)
		{
			return Id.GetHashCode();
		}
	}
}
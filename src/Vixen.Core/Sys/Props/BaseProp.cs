#nullable enable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Vixen.Attributes;
using Vixen.Model;
using Vixen.Services;

namespace Vixen.Sys.Props
{
	[Serializable]
	public abstract class BaseProp : BindableBase, IProp
	{
		private readonly Guid _id;
		private string _name;
		private string _createdBy;
		private DateTime _modifiedDate;

		#region Constructors

		protected BaseProp(string name, PropType propType)
		{
			Id = Guid.NewGuid();
			_name = name;
			CreationDate = _modifiedDate = DateTime.Now;
			_createdBy = Environment.UserName;
			PropType = propType;

			PropertyChanged += BaseProp_PropertyChanged;
		}

		private void BaseProp_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != null)
			{
				switch (e.PropertyName)
				{
					case nameof(Name):
						RenamePropElement();
						break;
				}
			}
		}

		#endregion
		[Browsable(false)]
		public Guid Id
		{
			get => _id;
			init => SetProperty(ref _id, value);
		}

		public string Name
		{
			get => _name;
			set => SetProperty(ref _name, value);
		}

		[PropertyOrder(30)]
		[DisplayName("Created By")]
		public string CreatedBy
		{
			get => _createdBy;
			set => SetProperty(ref _createdBy, value);
		}

		[PropertyOrder(31)]
		[DisplayName("Creation Date")]
		public DateTime CreationDate { get; init; }

		[PropertyOrder(32)]
		[DisplayName("Modified Date")]
		public DateTime ModifiedDate
		{
			get => _modifiedDate;
			set => SetProperty(ref _modifiedDate, value);
		}

		[PropertyOrder(0)]
		[DisplayName("Prop Type")]
		public PropType PropType { get; init; }



		[Browsable(false)]
		public Guid RootPropElementNodeId { get; protected set; } = Guid.Empty;

		protected string AutoPropName => $"Auto-Prop {Name}";

		protected string AutoPropStringName => "Auto-Prop String";

		protected string AutoPropNodeName => "Auto-Prop Node";

		[Browsable(false)]
		public virtual IPropModel PropModel { get; protected set; } = null!;

		public virtual void CleanUp()
		{
			RemovePropElements();
		}

		protected bool TryGetPropElementNode([NotNullWhen(true)] out ElementNode? elementNode)
		{
			elementNode = null;
			if (RootPropElementNodeId != Guid.Empty)
			{
				elementNode = VixenSystem.Nodes.GetElementNode(RootPropElementNodeId);
			}

			return elementNode is not null;
		}

		protected ElementNode GetOrCreatePropElementNode()
		{
			ElementNode? propNode = null;
			if (TryGetPropElementNode(out var elementNode))
			{
				propNode = elementNode;
			}
			else
			{
				propNode = ElementNodeService.Instance.CreateSingle(VixenSystem.Nodes.PropRootNode, AutoPropName, true, false);
				RootPropElementNodeId = propNode.Id;
			}

			return propNode;

		}

		protected void RenamePropElement()		
		{
			var propNode = GetOrCreatePropElementNode();
			VixenSystem.Nodes.RenameNode(propNode, AutoPropName, false);
		}

		protected IEnumerable<IElementNode> AddNodeElements(ElementNode node, int count, int namingIndex = 0)
		{
			List<IElementNode> nodesAdded = new List<IElementNode>();
			for (int j = namingIndex; j < count + namingIndex; j++)
			{
				string nodeName = $"{AutoPropNodeName} {j + 1}";

				var newNode = ElementNodeService.Instance.CreateSingle(node, nodeName, true, false);
				nodesAdded.Add(newNode);
			}

			return nodesAdded;
		}

		protected void RemoveElements(ElementNode node, int count)
		{
			var nodesToRemove = node.Children.Skip(node.Children.Count() - count).ToList();
			foreach (var nodeToRemove in nodesToRemove)
			{
				VixenSystem.Nodes.RemoveNode(nodeToRemove, node, true);
			}
		}

		/// <summary>
		/// Removes the entire Prop element tree. 
		/// </summary>
		protected virtual void RemovePropElements()
		{
			VixenSystem.Nodes.RemoveNode(GetOrCreatePropElementNode(), VixenSystem.Nodes.PropRootNode, true);
		}

		// Add logic to manage Element structure into the regular element tree, including supported properties
		// like patching order, orientation, etc. 

		// Add logic to handle the visual structure and mapping for the preview.

		// Implementing Prop classes should utilize the logic in this class to interface with the core of Vixen
		// Implementing Prop classes with auto generate the standard structure for the type of the Prop.
		// Implementing Prop classes will define and shape the default model view. The preview will manage sizing and placing them. 
		// Location will need to be handled as an offset into any previews that the Prop participates in. 
		// * Prop should have a list of associated preview ids
		// * Prop should have a lookup of the offset by preview id. 


		// Custom definitions for targeted rendering will need to be handled in some form. TBD.

		// Add other properties to manage things like controller target, 

	}
}

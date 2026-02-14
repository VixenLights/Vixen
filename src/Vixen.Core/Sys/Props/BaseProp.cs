#nullable enable
using NLog;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using Vixen.Attributes;
using Vixen.Model;
using Vixen.Services;
using Vixen.Sys.Props.Components;
using Vixen.Sys.Props.Model;

namespace Vixen.Sys.Props
{
	/// <summary>
	/// Represents the base class for all Prop types in the Vixen system.
	/// </summary>
	/// <remarks>
	/// This abstract class provides common functionality and properties for all Props,
	/// including unique identification, creation and modification metadata, and management
	/// of associated components and target nodes.
	/// </remarks>
	[Serializable]
	[CategoryOrder("Attributes", 1)]
	[CategoryOrder("Creation", 100)]
	public abstract class BaseProp<TModel> : BindableBase, IProp where TModel : BasePropModel, IPropModel
	{
		#region Protected Static Properties

		protected static Logger Logging { get; set; } = LogManager.GetCurrentClassLogger();

		#endregion

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
			PropComponents = new();
			UserDefinedPropComponents = new();

			PropertyChanged += BaseProp_PropertyChanged;
		}

		private void BaseProp_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != null)
			{
				switch (e.PropertyName)
				{
				}
				if (e.PropertyName != nameof(ModifiedDate))
				{
					PropModified();
				}
			}
		}

		#endregion
		/// <summary>
		/// Gets the unique identifier for the Prop.
		/// </summary>
		/// <remarks>
		/// This identifier is automatically generated upon the creation of the Prop
		/// and is used to uniquely distinguish it within the Vixen system.
		/// </remarks>
		[Browsable(false)]
		public Guid Id
		{
			get => _id;
			init => SetProperty(ref _id, value);
		}

		/// <summary>
		/// Gets or sets the name of the Prop.
		/// </summary>
		[Category("Attributes")]
		[PropertyOrder(0)]
		public string Name
		{
			get => _name;
			set
			{
				if (!VixenSystem.Props.IsUniquePropTitle(value))
				{
					MessageBox.Show($"{value} already exists. Enter a unique name.", "Prop name must be unique", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				else
				{
					RenamePropElement(_name, value);
					SetProperty(ref _name, value);
					OnPropertyChanged(nameof(Name));
				}
			}
		}

		[Category("Attributes")]
		[DisplayName("Prop Type")]
		[PropertyOrder(1)]
		[ReadOnly(true)]
		public PropType PropType { get; init; }


		/// <summary>
		/// Gets or sets the name of the user who created this Prop.
		/// </summary>
		/// <value>
		/// A <see cref="string"/> representing the creator's username.
		/// </value>
		/// <remarks>
		/// This property is used to track the origin of the Prop for auditing and informational purposes.
		/// </remarks>
		[Category("Creation")]
		[DisplayName("Created By")]
		[PropertyOrder(0)]
		[ReadOnly(true)]
		public string CreatedBy
		{
			get => _createdBy;
			set => SetProperty(ref _createdBy, value);
		}

		/// <summary>
		/// Gets the date and time when the Prop was created.
		/// </summary>
		/// <remarks>
		/// This property is initialized during the construction of the Prop and remains immutable.
		/// It provides metadata about the creation of the Prop for tracking and auditing purposes.
		/// </remarks>
		[Category("Creation")]
		[DisplayName("Creation Date")]
		[PropertyOrder(1)]
		[ReadOnly(true)]
		public DateTime CreationDate { get; init; }

		/// <summary>
		/// Gets or sets the date and time when the Prop was last modified.
		/// </summary>
		/// <value>
		/// A <see cref="DateTime"/> representing the last modification date and time of the Prop.
		/// </value>
		[Category("Creation")]
		[DisplayName("Modified Date")]
		[PropertyOrder(2)]
		[ReadOnly(true)]
		public DateTime ModifiedDate
		{
			get => _modifiedDate;
			set => SetProperty(ref _modifiedDate, value);
		}

		/// <summary>
		/// Gets or sets the visual model associated with the Prop.
		/// </summary>
		/// <remarks>
		/// The <see cref="IPropModel"/> represents the underlying data structure or configuration
		/// for the Props visual that is used to draw it. This property can be overridden in derived classes to
		/// provide specific implementations of the Prop model.
		/// </remarks>
		[Browsable(false)]
		IPropModel IProp.PropModel => PropModel;

		private TModel _propModel;
		
		[Browsable(false)]
		public TModel PropModel
		{
			get => _propModel;
			protected set => SetProperty(ref _propModel, value);
		}
		
		/// <summary>
		/// Gets or sets the unique identifier of the root <see cref="ElementNode"/> 
		/// associated with this Prop.
		/// </summary>
		/// <remarks>
		/// This property serves as a link between the Prop and its corresponding 
		/// root <see cref="ElementNode"/> in the legacy ElementNode tree. 
		/// If the identifier is <see cref="Guid.Empty"/>, it indicates that no 
		/// root <see cref="ElementNode"/> is currently associated with the Prop.
		/// </remarks>
		[Browsable(false)]
		public Guid RootPropElementNodeId { get; protected set; } = Guid.Empty;

		/// <summary>
		/// Gets the prefix used for automatically generated property names in the Prop system.
		/// </summary>
		/// <remarks>
		/// This property provides a consistent prefix for naming automatically generated properties.
		/// It is primarily used internally for constructing names for various Prop-related elements.
		/// </remarks>
		protected string AutoPropPrefix => "Auto-Prop";

		/// <summary>
		/// Gets the automatically generated name for the Prop, combining a predefined prefix with the Prop's name.
		/// </summary>
		/// <remarks>
		/// The <see cref="AutoPropName"/> property is used to create a unique and descriptive name for the Props linked ElementNodes.
		/// It is constructed by concatenating the <c>AutoPropPrefix</c> with the <see cref="Name"/> property.
		/// This name is primarily utilized for internal identification and ElementNode creation purposes.
		/// </remarks>
		protected string AutoPropName => $"{AutoPropPrefix} {Name}";

		/// <summary>
		/// Gets the name of the auto-generated string associated with the Prop.
		/// </summary>
		/// <remarks>
		/// This property provides a standardized name for the auto-generated string ElementNodes
		/// linked to the Prop, using a predefined prefix followed by the word "String".
		/// This name is primarily utilized for internal identification and ElementNode creation purposes.
		/// </remarks>
		protected string AutoPropStringName => $"{AutoPropPrefix} String";

		/// <summary>
		/// Gets the automatically generated name for a Prop linked leaf ElementNode.
		/// </summary>
		/// <remarks>
		/// This property combines a predefined prefix with the word "Node" to create a standardized
		/// name for prop nodes. It is primarily used for internal naming purposes when creating or
		/// managing prop elements.
		/// Example: In a pixel based Prop, this would represent one of the light nodes, but it is not limited to lights.
		/// </remarks>
		protected string AutoPropNodeName => $"{AutoPropPrefix} Node";

				
		/// <summary>
		/// Gets the target <see cref="IElementNode"/> associated with this Prop.
		/// </summary>
		/// <remarks>
		/// This property retrieves the backing <see cref="IElementNode"/> for the Prop. 
		/// If the backing node does not exist, it will be created using the 
		/// <see cref="GetOrCreateElementNode"/> method.
		/// </remarks>
		/// <value>
		/// The <see cref="IElementNode"/> representing the target node of the Prop.
		/// </value>
		[Browsable(false)]
		public virtual IElementNode TargetNode => GetOrCreateElementNode();

		[Browsable(false)]
		public ObservableCollection<IPropComponent> PropComponents { get; init; }

		[Browsable(false)]
		public ObservableCollection<IPropComponent> UserDefinedPropComponents { get; init; }

		public abstract string GetSummary();

		public virtual void CleanUp()
		{
			RemovePropElementNode();
			PropModified();
		}
		
		/// <summary>
		/// Minimally updates the <see cref="ModifiedDate"/> property to the current date and time.
		/// </summary>
		/// <remarks>
		/// This method should be called internally by implementations whenever a modification occurs to ensure
		/// the <see cref="ModifiedDate"/> reflects the latest changes.
		/// </remarks>
		protected void PropModified()
		{
			ModifiedDate = DateTime.Now;
		}

		/// <summary>
		/// Attempts to retrieve the backing <see cref="ElementNode"/> associated with the current prop.
		/// </summary>
		/// <param name="elementNode">
		/// When this method returns, contains the <see cref="ElementNode"/> associated with the current prop
		/// in the legacy ElementNode tree,
		/// if one exists; otherwise, <c>null</c>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the <see cref="ElementNode"/> was successfully retrieved; otherwise, <c>false</c>.
		/// </returns>
		protected bool TryGetPropElementNode([NotNullWhen(true)] out ElementNode? elementNode)
		{
			elementNode = null;
			if (RootPropElementNodeId != Guid.Empty)
			{
				elementNode = VixenSystem.Nodes.GetElementNode(RootPropElementNodeId);
			}

			return elementNode is not null;
		}

		/// <summary>
		/// Retrieves the existing backing <see cref="ElementNode"/> associated with the Prop,
		/// or creates a new one if it does not exist.
		/// </summary>
		/// <returns>
		/// The <see cref="ElementNode"/> in the legacy ElementNode tree representing the Prop.
		/// </returns>
		/// <remarks>
		/// This method ensures that the property has a backing <see cref="ElementNode"/>.
		/// If an existing <see cref="ElementNode"/> is not found, a new one is created.
		/// The newly created node is linked to the Prop by assigning its ID to <see cref="RootPropElementNodeId"/>.
		/// </remarks>
		protected ElementNode GetOrCreateElementNode()
		{
			ElementNode? backingElementNode;
			if (TryGetPropElementNode(out var elementNode))
			{
				backingElementNode = elementNode;
			}
			else
			{
				backingElementNode = ElementNodeService.Instance.CreateSingle(VixenSystem.Nodes.PropRootNode, AutoPropName, true, false);
				RootPropElementNodeId = backingElementNode.Id;
			}

			return backingElementNode;

		}

		/// <summary>
		/// Renames the backing <see cref="ElementNode"/> associated with the Prop to reflect the current name of the Prop.
		/// </summary>
		/// <remarks>
		/// This method ensures that the <see cref="ElementNode"/> representing the Prop is updated with a new name
		/// derived from the Prop's current name and the <see cref="AutoPropPrefix"/>.
		/// The renaming operation is performed using the <see cref="VixenSystem.Nodes"/> manager.
		/// </remarks>
		protected void RenamePropElement(String oldName, String newName)		
		{
			foreach(var node in PropComponents)
			{
				node.Name = node.Name.Replace(oldName, newName);
			}
		}

		/// <summary>
		/// Adds a specified number of child nodes to the given parent node and returns the newly created nodes.
		/// </summary>
		/// <param name="node">The parent <see cref="ElementNode"/> to which the child nodes will be added.</param>
		/// <param name="count">The number of child nodes to add. Must be greater than zero.</param>
		/// <param name="namingIndex">
		/// The starting index for naming the new nodes. The names will be generated sequentially starting from this index.
		/// </param>
		/// <returns>
		/// An <see cref="IEnumerable{T}"/> of <see cref="IElementNode"/> representing the newly created child nodes.
		/// </returns>
		/// <remarks>
		/// The method generates names for the new nodes using a predefined naming pattern and ensures that the nodes are 
		/// created as children of the specified parent node. If <paramref name="count"/> is zero, an empty collection is returned.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="node"/> parameter is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="count"/> is less than zero.</exception>
		protected IEnumerable<IElementNode> AddNodeElements(ElementNode node, int count, int namingIndex = 0)
		{
			if (count == 0) return [];
			List<IElementNode> nodesAdded = new List<IElementNode>();
			for (int j = namingIndex; j < count + namingIndex; j++)
			{
				string nodeName = $"{AutoPropNodeName} {j + 1}";

				var newNode = ElementNodeService.Instance.CreateSingle(node, nodeName, true, false);
				nodesAdded.Add(newNode);
			}

			return nodesAdded;
		}

		/// <summary>
		/// Removes a specified number of child elements from the given <see cref="ElementNode"/>.
		/// </summary>
		/// <param name="node">The <see cref="ElementNode"/> from which child elements will be removed.</param>
		/// <param name="count">The number of child elements to remove from the end of the node's children.</param>
		/// <remarks>
		/// This method removes the specified number of child elements from the end of the provided node's children collection.
		/// It ensures that the nodes are properly removed from the system using <see cref="VixenSystem.Nodes.RemoveNode"/>.
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="node"/> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="count"/> is greater than the number of children in the <paramref name="node"/>.
		/// </exception>
		protected void RemoveElementNodes(ElementNode node, int count)
		{
			var nodesToRemove = node.Children.Skip(node.Children.Count() - count).ToList();
			foreach (var nodeToRemove in nodesToRemove)
			{
				VixenSystem.Nodes.RemoveNode(nodeToRemove, node, true);
			}
		}

		/// <summary>
		/// Removes the entire linked <see cref="ElementNode"/> tree associated with this Prop.
		/// </summary>
		/// <remarks>
		/// This method removes the backing element node of the Prop from the system's node hierarchy.
		/// It ensures that the node is detached from its parent and cleans up any associated resources.
		/// </remarks>
		protected virtual void RemovePropElementNode()
		{
			VixenSystem.Nodes.RemoveNode(GetOrCreateElementNode(), VixenSystem.Nodes.PropRootNode, true);
			RootPropElementNodeId = Guid.Empty;
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

using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using VixenModules.Property.State.Setup.Models;

namespace VixenModules.Property.State.Setup.ViewModels
{
	/// <summary>
	/// Provides editable values and assignments for one State property item.
	/// </summary>
	public sealed class StateItemViewModel : ViewModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateItemViewModel"/> class.
		/// </summary>
		/// <param name="item">The state item draft to edit.</param>
		/// <param name="elementTree">The element hierarchy available for assignment.</param>
		public StateItemViewModel(StateItemData item, StateElementNodeSnapshot elementTree)
		{
			ArgumentNullException.ThrowIfNull(item);
			ArgumentNullException.ThrowIfNull(elementTree);

			DeferValidationUntilFirstSaveCall = false;
			AssignmentRoots =
			[
				new StateAssignmentTreeNode(elementTree, item.ElementNodeIds.ToHashSet())
			];
			AssignmentSelection = new StateAssignmentTreeSelectionController(AssignmentRoots);

			foreach (var root in AssignmentRoots)
			{
				root.AssignmentChanged += AssignmentChanged;
			}

			Item = item;
			UpdateAssignments();
			Validate(true);
		}

		[Model]
		internal StateItemData Item
		{
			get => GetValue<StateItemData>(ItemProperty);
			private set => SetValue(ItemProperty, value);
		}

		private static readonly IPropertyData ItemProperty = RegisterProperty<StateItemData>(nameof(Item));

		/// <summary>
		/// Gets the controller that manages temporary assignment-tree selection.
		/// </summary>
		/// <value>The controller that manages temporary assignment-tree selection.</value>
		internal StateAssignmentTreeSelectionController AssignmentSelection { get; }

		internal void ExpandCheckedAssignments()
		{
			foreach (var root in AssignmentRoots)
			{
				root.ExpandCheckedDescendants();
			}
		}

		/// <summary>
		/// Gets or sets the user-visible state item name.
		/// </summary>
		/// <value>The user-visible state item name.</value>
		[ViewModelToModel(nameof(Item))]
		public string Name
		{
			get => GetValue<string>(NameProperty);
			set
			{
				var normalizedName = value?.Trim() ?? string.Empty;
				SetValue(NameProperty, normalizedName);
			}
		}

		/// <summary>
		/// Identifies the <see cref="Name"/> property.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name), string.Empty);

		/// <summary>
		/// Gets or sets the display color associated with the state item.
		/// </summary>
		/// <value>The display color associated with the state item.</value>
		[ViewModelToModel(nameof(Item))]
		public System.Drawing.Color Color
		{
			get => GetValue<System.Drawing.Color>(ColorProperty);
			set => SetValue(ColorProperty, value);
		}

		/// <summary>
		/// Identifies the <see cref="Color"/> property.
		/// </summary>
		public static readonly IPropertyData ColorProperty = RegisterProperty<System.Drawing.Color>(nameof(Color));

		/// <summary>
		/// Gets the effective number of assigned leaf element nodes.
		/// </summary>
		/// <value>The effective number of assigned leaf element nodes.</value>
		public int AssignmentCount => AssignmentRoots
			.SelectMany(root => root.GetEffectiveLeafNodeIds())
			.Distinct()
			.Count();

		/// <summary>
		/// Gets the root nodes displayed in the element assignment tree.
		/// </summary>
		/// <value>The root nodes displayed in the element assignment tree.</value>
		public ObservableCollection<StateAssignmentTreeNode> AssignmentRoots { get; }

		/// <inheritdoc />
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError(NameProperty, "State item name is required."));
			}
		}

		private void AssignmentChanged(object? sender, EventArgs e)
		{
			UpdateAssignments();
		}

		private void UpdateAssignments()
		{
			Item.ElementNodeIds = AssignmentRoots
				.SelectMany(root => root.GetSelectedNodeIds())
				.Distinct()
				.ToList();
			RaisePropertyChanged(nameof(AssignmentCount));
		}
	}
}

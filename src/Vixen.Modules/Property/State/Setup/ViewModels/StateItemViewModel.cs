using System.Collections.ObjectModel;
using VixenModules.Property.State.Setup.Models;

namespace VixenModules.Property.State.Setup.ViewModels
{
	/// <summary>
	/// Provides editable values and assignments for one State property item.
	/// </summary>
	public sealed class StateItemViewModel : StateBindableBase
	{
		private readonly StateItemData _item;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateItemViewModel"/> class.
		/// </summary>
		/// <param name="item">The state item draft to edit.</param>
		/// <param name="elementTree">The element hierarchy available for assignment.</param>
		public StateItemViewModel(StateItemData item, StateElementNodeSnapshot elementTree)
		{
			ArgumentNullException.ThrowIfNull(item);
			ArgumentNullException.ThrowIfNull(elementTree);

			_item = item;
			AssignmentRoots =
			[
				new StateAssignmentTreeNode(elementTree, item.ElementNodeIds.ToHashSet())
			];

			foreach (var root in AssignmentRoots)
			{
				root.AssignmentChanged += AssignmentChanged;
			}
		}

		/// <summary>
		/// Gets or sets the user-visible state item name.
		/// </summary>
		/// <value>The user-visible state item name.</value>
		public string Name
		{
			get => _item.Name;
			set
			{
				if (_item.Name == value)
				{
					return;
				}

				_item.Name = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the display color associated with the state item.
		/// </summary>
		/// <value>The display color associated with the state item.</value>
		public System.Drawing.Color Color
		{
			get => _item.Color;
			set
			{
				if (_item.Color == value)
				{
					return;
				}

				_item.Color = value;
				OnPropertyChanged();
			}
		}

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

		internal StateItemData Item => _item;

		private void AssignmentChanged(object? sender, EventArgs e)
		{
			_item.ElementNodeIds = AssignmentRoots
				.SelectMany(root => root.GetSelectedNodeIds())
				.Distinct()
				.ToList();
			OnPropertyChanged(nameof(AssignmentCount));
		}
	}
}

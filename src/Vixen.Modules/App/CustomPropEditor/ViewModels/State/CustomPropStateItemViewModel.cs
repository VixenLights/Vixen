using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Extensions;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using DrawingColor = System.Drawing.Color;

namespace VixenModules.App.CustomPropEditor.ViewModels.State
{
	/// <summary>
	/// Represents one editable State item row in the Custom Prop Editor.
	/// </summary>
	public sealed class CustomPropStateItemViewModel : ViewModelBase
	{
		private readonly Prop _prop;
		private readonly Action _itemChanged;
		private HashSet<Guid> _validElementModelIds;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomPropStateItemViewModel"/> class.
		/// </summary>
		/// <param name="stateItem">The State item model to edit.</param>
		/// <param name="prop">The custom prop containing assignment elements.</param>
		/// <param name="itemChanged">The callback invoked when the item changes.</param>
		public CustomPropStateItemViewModel(StateItemModel stateItem, Prop prop, Action itemChanged)
		{
			StateItem = stateItem ?? throw new ArgumentNullException(nameof(stateItem));
			_prop = prop ?? throw new ArgumentNullException(nameof(prop));
			_itemChanged = itemChanged ?? throw new ArgumentNullException(nameof(itemChanged));
			RefreshValidElementModelIds();
			AssignmentTree = new ObservableCollection<CustomPropStateAssignmentTreeNodeViewModel>(
				[new CustomPropStateAssignmentTreeNodeViewModel(prop.RootNode, stateItem, OnAssignmentChanged)]);
		}

		/// <summary>
		/// Gets the State item model being edited.
		/// </summary>
		[Browsable(false)]
		public StateItemModel StateItem { get; }

		/// <summary>
		/// Gets the State item assignment tree.
		/// </summary>
		public ObservableCollection<CustomPropStateAssignmentTreeNodeViewModel> AssignmentTree { get; }

		/// <summary>
		/// Gets or sets the State item name.
		/// </summary>
		public string Name
		{
			get => StateItem.Name;
			set
			{
				var normalizedValue = value ?? string.Empty;
				if (StateItem.Name == normalizedValue)
				{
					return;
				}

				StateItem.Name = normalizedValue;
				OnItemChanged(nameof(Name));
			}
		}

		/// <summary>
		/// Gets or sets the State item color.
		/// </summary>
		public DrawingColor Color
		{
			get => StateItem.Color;
			set
			{
				if (StateItem.Color.Equals(value))
				{
					return;
				}

				StateItem.Color = value;
				OnItemChanged(nameof(Color));
				RaisePropertyChanged(nameof(ColorHex));
			}
		}

		/// <summary>
		/// Gets or sets the State item color as a hexadecimal string.
		/// </summary>
		public string ColorHex
		{
			get => StateItem.Color.ToHex();
			set
			{
				var color = ConvertHexToColor(value);
				if (color == DrawingColor.Empty)
				{
					return;
				}

				Color = color;
			}
		}

		/// <summary>
		/// Gets a value that indicates whether the State item has at least one assignment.
		/// </summary>
		public bool HasAssignments => AssignmentCount > 0;

		/// <summary>
		/// Gets the number of assigned element models that still exist in the prop.
		/// </summary>
		public int AssignmentCount => StateItem.ElementModelIds
			.Distinct()
			.Count(id => _validElementModelIds.Contains(id));

		/// <summary>
		/// Refreshes assignment state after external assignment changes.
		/// </summary>
		public void RefreshAssignments()
		{
			RefreshValidElementModelIds();
			RaisePropertyChanged(nameof(HasAssignments));
			RaisePropertyChanged(nameof(AssignmentCount));

			foreach (var rootNode in AssignmentTree)
			{
				rootNode.RefreshAssignmentState();
			}
		}

		/// <summary>
		/// Adds element model assignments to this State item.
		/// </summary>
		/// <param name="elementModelIds">The element model identifiers to assign.</param>
		/// <returns><see langword="true" /> if assignments were added; otherwise, <see langword="false" />.</returns>
		public bool AssignElementModelIds(IEnumerable<Guid> elementModelIds)
		{
			var changed = false;
			foreach (var elementModelId in elementModelIds.Where(id => id != Guid.Empty).Distinct())
			{
				if (StateItem.ElementModelIds.Contains(elementModelId))
				{
					continue;
				}

				StateItem.ElementModelIds.Add(elementModelId);
				changed = true;
			}

			if (!changed)
			{
				return false;
			}

			OnAssignmentChanged();
			return true;
		}

		/// <summary>
		/// Toggles an element model assignment on this State item.
		/// </summary>
		/// <param name="elementModelId">The element model identifier to toggle.</param>
		/// <returns><see langword="true" /> if the assignment data changed; otherwise, <see langword="false" />.</returns>
		public bool ToggleElementModelId(Guid elementModelId)
		{
			if (elementModelId == Guid.Empty)
			{
				return false;
			}

			if (!StateItem.ElementModelIds.Remove(elementModelId))
			{
				StateItem.ElementModelIds.Add(elementModelId);
			}

			OnAssignmentChanged();
			return true;
		}

		/// <summary>
		/// Clears the dirty state for this State item.
		/// </summary>
		public void ClearIsDirty()
		{
			IsDirty = false;
		}

		private void OnAssignmentChanged()
		{
			OnItemChanged(nameof(HasAssignments));
			RaisePropertyChanged(nameof(AssignmentCount));
			RefreshAssignments();
		}

		private void RefreshValidElementModelIds()
		{
			_validElementModelIds = _prop.GetAll()
				.Select(model => model.Id)
				.ToHashSet();
		}

		private void OnItemChanged(string propertyName)
		{
			IsDirty = true;
			_itemChanged();
			RaisePropertyChanged(propertyName);
		}

		private static DrawingColor ConvertHexToColor(string hexColor)
		{
			if (string.IsNullOrWhiteSpace(hexColor))
			{
				return DrawingColor.Empty;
			}

			try
			{
				var mediaColor = ColorConverter.ConvertFromString(hexColor);
				if (mediaColor is Color color)
				{
					return color.ToDrawingColor();
				}
			}
			catch (FormatException)
			{
				return DrawingColor.Empty;
			}
			catch (NotSupportedException)
			{
				return DrawingColor.Empty;
			}

			return DrawingColor.Empty;
		}
	}
}

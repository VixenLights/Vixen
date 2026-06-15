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
		public bool HasAssignments => StateItem.ElementModelIds.Any(id => _prop.GetAll().Any(model => model.Id == id));

		/// <summary>
		/// Refreshes assignment state after external assignment changes.
		/// </summary>
		public void RefreshAssignments()
		{
			RaisePropertyChanged(nameof(HasAssignments));

			foreach (var rootNode in AssignmentTree)
			{
				rootNode.RefreshAssignmentState();
			}
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
			RefreshAssignments();
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

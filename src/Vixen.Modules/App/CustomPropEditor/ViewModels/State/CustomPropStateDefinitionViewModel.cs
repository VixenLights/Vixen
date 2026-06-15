using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModels.State
{
	/// <summary>
	/// Represents one editable State definition in the Custom Prop Editor.
	/// </summary>
	public sealed class CustomPropStateDefinitionViewModel : ViewModelBase
	{
		private readonly Prop _prop;
		private readonly Action _definitionChanged;
		private CustomPropStateItemViewModel _selectedItem;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomPropStateDefinitionViewModel"/> class.
		/// </summary>
		/// <param name="stateDefinition">The State definition model to edit.</param>
		/// <param name="prop">The custom prop containing assignment elements.</param>
		/// <param name="definitionChanged">The callback invoked when the definition changes.</param>
		public CustomPropStateDefinitionViewModel(
			StateDefinitionModel stateDefinition,
			Prop prop,
			Action definitionChanged)
		{
			StateDefinition = stateDefinition ?? throw new ArgumentNullException(nameof(stateDefinition));
			_prop = prop ?? throw new ArgumentNullException(nameof(prop));
			_definitionChanged = definitionChanged ?? throw new ArgumentNullException(nameof(definitionChanged));
			Items = new ObservableCollection<CustomPropStateItemViewModel>(
				StateDefinition.Items.Select(CreateItemViewModel));
			Items.CollectionChanged += ItemsOnCollectionChanged;
			SelectedItem = Items.FirstOrDefault();
		}

		/// <summary>
		/// Gets the State definition model being edited.
		/// </summary>
		[Browsable(false)]
		public StateDefinitionModel StateDefinition { get; }

		/// <summary>
		/// Gets the editable State item rows.
		/// </summary>
		public ObservableCollection<CustomPropStateItemViewModel> Items { get; }

		/// <summary>
		/// Gets or sets the selected State item row.
		/// </summary>
		public CustomPropStateItemViewModel SelectedItem
		{
			get => _selectedItem;
			set
			{
				if (Equals(value, _selectedItem))
				{
					return;
				}

				_selectedItem = value;
				RaisePropertyChanged(nameof(SelectedItem));
			}
		}

		/// <summary>
		/// Gets or sets the State definition name.
		/// </summary>
		public string Name
		{
			get => StateDefinition.Name;
			set
			{
				var normalizedValue = value ?? string.Empty;
				if (StateDefinition.Name == normalizedValue)
				{
					return;
				}

				StateDefinition.Name = normalizedValue;
				OnDefinitionChanged(nameof(Name));
			}
		}

		/// <summary>
		/// Gets or sets the State definition description.
		/// </summary>
		public string Description
		{
			get => StateDefinition.Description;
			set
			{
				var normalizedValue = value ?? string.Empty;
				if (StateDefinition.Description == normalizedValue)
				{
					return;
				}

				StateDefinition.Description = normalizedValue;
				OnDefinitionChanged(nameof(Description));
			}
		}

		/// <summary>
		/// Adds a State item to this definition.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddItem(StateItemModel item)
		{
			ArgumentNullException.ThrowIfNull(item);

			StateDefinition.Items.Add(item);
			var itemViewModel = CreateItemViewModel(item);
			Items.Add(itemViewModel);
			SelectedItem = itemViewModel;
			OnDefinitionChanged(nameof(Items));
		}

		/// <summary>
		/// Removes a State item from this definition.
		/// </summary>
		/// <param name="itemViewModel">The item view model to remove.</param>
		public void RemoveItem(CustomPropStateItemViewModel itemViewModel)
		{
			if (itemViewModel == null)
			{
				return;
			}

			var index = Items.IndexOf(itemViewModel);
			if (index < 0)
			{
				return;
			}

			StateDefinition.Items.Remove(itemViewModel.StateItem);
			Items.RemoveAt(index);
			SelectedItem = Items.ElementAtOrDefault(Math.Min(index, Items.Count - 1));
			OnDefinitionChanged(nameof(Items));
		}

		/// <summary>
		/// Moves a State item within this definition.
		/// </summary>
		/// <param name="itemViewModel">The item view model to move.</param>
		/// <param name="offset">The relative row offset.</param>
		public void MoveItem(CustomPropStateItemViewModel itemViewModel, int offset)
		{
			if (itemViewModel == null)
			{
				return;
			}

			var oldIndex = Items.IndexOf(itemViewModel);
			var newIndex = oldIndex + offset;
			if (oldIndex < 0 || newIndex < 0 || newIndex >= Items.Count)
			{
				return;
			}

			Items.Move(oldIndex, newIndex);
			StateDefinition.Items.Move(oldIndex, newIndex);
			SelectedItem = itemViewModel;
			OnDefinitionChanged(nameof(Items));
		}

		/// <summary>
		/// Refreshes assignment state on every State item.
		/// </summary>
		public void RefreshAssignments()
		{
			foreach (var item in Items)
			{
				item.RefreshAssignments();
			}
		}

		/// <summary>
		/// Clears the dirty state for this State definition and its items.
		/// </summary>
		public void ClearIsDirty()
		{
			IsDirty = false;

			foreach (var item in Items)
			{
				item.ClearIsDirty();
			}
		}

		private CustomPropStateItemViewModel CreateItemViewModel(StateItemModel item)
		{
			return new CustomPropStateItemViewModel(item, _prop, OnItemChanged);
		}

		private void OnItemChanged()
		{
			OnDefinitionChanged(nameof(Items));
		}

		private void OnDefinitionChanged(string propertyName)
		{
			IsDirty = true;
			_definitionChanged();
			RaisePropertyChanged(propertyName);
		}

		private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(nameof(Items));
		}
	}
}

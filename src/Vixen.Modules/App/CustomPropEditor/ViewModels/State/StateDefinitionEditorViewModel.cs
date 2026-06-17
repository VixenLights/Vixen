using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using VixenModules.Property.State.Setup.Services;

namespace VixenModules.App.CustomPropEditor.ViewModels.State
{
	/// <summary>
	/// Coordinates Custom Prop Editor State definition authoring.
	/// </summary>
	public sealed class StateDefinitionEditorViewModel : ViewModelBase
	{
		private readonly IStateDefinitionDialogService _stateDefinitionDialogService;
		private CustomPropStateDefinitionViewModel _selectedStateDefinition;
		private TaskCommand _addStateDefinitionCommand;
		private TaskCommand _deleteStateDefinitionCommand;
		private TaskCommand _renameStateDefinitionCommand;
		private TaskCommand _copyStateDefinitionCommand;
		private Command _addStateItemCommand;
		private TaskCommand _removeStateItemCommand;
		private Command _moveStateItemUpCommand;
		private Command _moveStateItemDownCommand;
		private Command<IList> _persistStateItemSortCommand;
		
		/// <summary>
		/// Occurs when State definition data changes.
		/// </summary>
		public event EventHandler StateDataChanged;

		/// <summary>
		/// Occurs when the State preview selection changes.
		/// </summary>
		public event EventHandler StatePreviewChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateDefinitionEditorViewModel"/> class.
		/// </summary>
		/// <param name="prop">The custom prop to edit.</param>
		/// <param name="stateDefinitionDialogService"></param>
		public StateDefinitionEditorViewModel(
			Prop prop,
			IStateDefinitionDialogService stateDefinitionDialogService)
		{
			ArgumentNullException.ThrowIfNull(stateDefinitionDialogService);

			_stateDefinitionDialogService = stateDefinitionDialogService;
			StateDefinitions = new ObservableCollection<CustomPropStateDefinitionViewModel>();
			SelectedStateItems = new ObservableCollection<CustomPropStateItemViewModel>();
			SelectedStateItems.CollectionChanged += SelectedStateItemsOnCollectionChanged;
			ValidationMessages = new ObservableCollection<string>();
			SetProp(prop);
		}

		/// <summary>
		/// Gets the custom prop currently being edited.
		/// </summary>
		[Browsable(false)]
		public Prop Prop { get; private set; }

		/// <summary>
		/// Gets the model element that stores authored State definitions.
		/// </summary>
		[Browsable(false)]
		public ElementModel ModelElement { get; private set; }

		/// <summary>
		/// Gets the editable State definitions.
		/// </summary>
		public ObservableCollection<CustomPropStateDefinitionViewModel> StateDefinitions { get; }

		/// <summary>
		/// Gets validation messages that block saving.
		/// </summary>
		public ObservableCollection<string> ValidationMessages { get; }

		/// <summary>
		/// Gets the selected State item rows.
		/// </summary>
		public ObservableCollection<CustomPropStateItemViewModel> SelectedStateItems { get; }

		/// <summary>
		/// Gets or sets the selected State definition.
		/// </summary>
		public CustomPropStateDefinitionViewModel SelectedStateDefinition
		{
			get => _selectedStateDefinition;
			set
			{
				if (Equals(value, _selectedStateDefinition))
				{
					return;
				}

				_selectedStateDefinition = value;
				SelectSingleStateItem(_selectedStateDefinition?.SelectedItem);
				RaisePropertyChanged(nameof(SelectedStateDefinition));
				RaisePropertyChanged(nameof(SelectedStateItem));
				RaiseCommandCanExecuteChanged();
				StatePreviewChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets or sets the selected State item.
		/// </summary>
		public CustomPropStateItemViewModel SelectedStateItem
		{
			get => SelectedStateDefinition?.SelectedItem;
			set
			{
				if (SelectedStateDefinition == null)
				{
					return;
				}

				SelectedStateDefinition.SelectedItem = value;
				if (SelectedStateItems.Count == 0 && value != null)
				{
					SelectSingleStateItem(value);
				}

				RaisePropertyChanged(nameof(SelectedStateItem));
				RaiseCommandCanExecuteChanged();
				StatePreviewChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets a value that indicates whether canvas assignment edits are enabled for the selected State items.
		/// </summary>
		public bool CanEditCanvasAssignments => SelectedStateItems.Count <= 1;

		/// <summary>
		/// Gets a value that indicates whether State validation currently blocks saving.
		/// </summary>
		public bool HasValidationErrors => ValidationMessages.Any();

		/// <summary>
		/// Gets the validation message to show when saving is blocked.
		/// </summary>
		public string SaveValidationMessage => string.Join(Environment.NewLine, ValidationMessages);

		/// <summary>
		/// Gets the Add State definition command.
		/// </summary>
		public TaskCommand AddStateDefinitionCommand =>
			_addStateDefinitionCommand ??= new TaskCommand(AddStateDefinitionAsync);

		/// <summary>
		/// Gets the Delete State definition command.
		/// </summary>
		public TaskCommand DeleteStateDefinitionCommand =>
			_deleteStateDefinitionCommand ??= new TaskCommand(DeleteStateDefinitionAsync, CanDeleteStateDefinition);

		/// <summary>
		/// Gets the Rename State definition command.
		/// </summary>
		public TaskCommand RenameStateDefinitionCommand =>
			_renameStateDefinitionCommand ??= new TaskCommand(RenameStateDefinitionAsync, CanRenameStateDefinition);

		/// <summary>
		/// Gets the Copy State definition command.
		/// </summary>
		public TaskCommand CopyStateDefinitionCommand =>
			_copyStateDefinitionCommand ??= new TaskCommand(CopyStateDefinitionAsync, CanCopyStateDefinition);

		/// <summary>
		/// Gets the Add State item command.
		/// </summary>
		public Command AddStateItemCommand =>
			_addStateItemCommand ??= new Command(AddStateItem, CanAddStateItem);

		/// <summary>
		/// Gets the Remove State item command.
		/// </summary>
		public TaskCommand RemoveStateItemCommand =>
			_removeStateItemCommand ??= new TaskCommand(RemoveStateItemAsync, CanRemoveStateItem);

		/// <summary>
		/// Gets the Move State item up command.
		/// </summary>
		public Command MoveStateItemUpCommand =>
			_moveStateItemUpCommand ??= new Command(MoveStateItemUp, CanMoveStateItemUp);

		/// <summary>
		/// Gets the Move State item down command.
		/// </summary>
		public Command MoveStateItemDownCommand =>
			_moveStateItemDownCommand ??= new Command(MoveStateItemDown, CanMoveStateItemDown);

		/// <summary>
		/// Gets the command that persists the displayed State item sort order.
		/// </summary>
		public Command<IList> PersistStateItemSortCommand =>
			_persistStateItemSortCommand ??= new Command<IList>(PersistStateItemSortOrder, CanPersistStateItemSortOrder);

		/// <summary>
		/// Rebinds the editor to another custom prop.
		/// </summary>
		/// <param name="prop">The custom prop to edit.</param>
		public void SetProp(Prop prop)
		{
			Prop = prop ?? throw new ArgumentNullException(nameof(prop));
			var migrated = CustomPropStateMigrationService.MigrateLegacyStateData(prop);
			ModelElement = PropStateModelResolver.GetModelElement(prop);
			ModelElement.NormalizeStateModelData();

			StateDefinitions.CollectionChanged -= StateDefinitionsOnCollectionChanged;
			StateDefinitions.Clear();

			foreach (var stateDefinition in ModelElement.StateDefinitionModels)
			{
				StateDefinitions.Add(CreateDefinitionViewModel(stateDefinition));
			}

			StateDefinitions.CollectionChanged += StateDefinitionsOnCollectionChanged;
			SelectedStateDefinition = StateDefinitions.FirstOrDefault();
			RefreshValidation();
			IsDirty = migrated;
			RaisePropertyChanged(nameof(Prop));
			RaisePropertyChanged(nameof(ModelElement));
		}

		/// <summary>
		/// Refreshes State assignment counts after the prop element model changes.
		/// </summary>
		public void RefreshAssignments()
		{
			foreach (var stateDefinition in StateDefinitions)
			{
				stateDefinition.RefreshAssignments();
			}

			RefreshValidation();
		}

		/// <summary>
		/// Refreshes validation messages for the current State definition data.
		/// </summary>
		public void RefreshValidation()
		{
			ValidationMessages.Clear();

			AddModelTypeValidationMessages();
			AddStateDefinitionValidationMessages();

			RaisePropertyChanged(nameof(HasValidationErrors));
			RaisePropertyChanged(nameof(SaveValidationMessage));
			RaiseCommandCanExecuteChanged();
		}

		/// <summary>
		/// Clears the editor dirty state.
		/// </summary>
		public void ClearIsDirty()
		{
			IsDirty = false;

			foreach (var stateDefinition in StateDefinitions)
			{
				stateDefinition.ClearIsDirty();
			}
		}

		private async Task AddStateDefinitionAsync()
		{
			var name = await _stateDefinitionDialogService.RequestNameAsync(
				"Add State Definition",
				GetNextStateDefinitionName(),
				GetStateDefinitionNames(),
				null);
			if (!TryNormalizeStateDefinitionName(name, null, out var normalizedName))
			{
				return;
			}
			var stateDefinition = StateDefinitionModel.CreateDefault(normalizedName);
			ModelElement.StateDefinitionModels.Add(stateDefinition);
			var stateDefinitionViewModel = CreateDefinitionViewModel(stateDefinition);
			StateDefinitions.Add(stateDefinitionViewModel);
			SelectedStateDefinition = stateDefinitionViewModel;
			OnStateDataChanged();
		}

		private async Task DeleteStateDefinitionAsync()
		{
			if (SelectedStateDefinition == null ||
			    !await _stateDefinitionDialogService.ConfirmDeleteAsync(SelectedStateDefinition.Name))
			{
				return;
			}

			var index = StateDefinitions.IndexOf(SelectedStateDefinition);
			ModelElement.StateDefinitionModels.Remove(SelectedStateDefinition.StateDefinition);
			StateDefinitions.RemoveAt(index);
			SelectedStateDefinition = StateDefinitions.ElementAtOrDefault(Math.Min(index, StateDefinitions.Count - 1));
			OnStateDataChanged();
		}

		private bool CanDeleteStateDefinition()
		{
			return SelectedStateDefinition != null;
		}

		private async Task RenameStateDefinitionAsync()
		{
			if (SelectedStateDefinition == null)
			{
				return;
			}

			var name = await _stateDefinitionDialogService.RequestNameAsync(
				"Rename State Definition",
				SelectedStateDefinition.Name,
				GetStateDefinitionNames(),
				SelectedStateDefinition.Name);
			if (!TryNormalizeStateDefinitionName(name, SelectedStateDefinition.Name, out var normalizedName))
			{
				return;
			}

			SelectedStateDefinition.Name = normalizedName;
			OnStateDataChanged();
		}

		private bool CanRenameStateDefinition()
		{
			return SelectedStateDefinition != null;
		}

		private async Task CopyStateDefinitionAsync()
		{
			if (SelectedStateDefinition == null)
			{
				return;
			}
			
			var name = await _stateDefinitionDialogService.RequestNameAsync(
				"Copy State Definition",
				SelectedStateDefinition.Name + @" Copy",
				GetStateDefinitionNames(),
				null);
			
			if (!TryNormalizeStateDefinitionName(name, null, out var normalizedName))
			{
				return;
			}

			var copy = SelectedStateDefinition.StateDefinition.CloneAsNew(normalizedName);
			ModelElement.StateDefinitionModels.Add(copy);
			var copyViewModel = CreateDefinitionViewModel(copy);
			StateDefinitions.Add(copyViewModel);
			SelectedStateDefinition = copyViewModel;
			OnStateDataChanged();
		}

		private bool CanCopyStateDefinition()
		{
			return SelectedStateDefinition != null;
		}

		private void AddStateItem()
		{
			if (SelectedStateDefinition == null)
			{
				return;
			}

			SelectedStateDefinition.AddItem(new StateItemModel
			{
				Name = GetUniqueStateItemName(StateItemModel.DefaultName, SelectedStateDefinition)
			});
			SelectSingleStateItem(SelectedStateDefinition.SelectedItem);
			RaisePropertyChanged(nameof(SelectedStateItem));
			OnStateDataChanged();
		}

		private bool CanAddStateItem()
		{
			return SelectedStateDefinition != null;
		}

		private async Task RemoveStateItemAsync()
		{
			if (SelectedStateDefinition == null)
			{
				return;
			}

			var itemsToRemove = GetSelectedStateItemsForRemoval();
			if (itemsToRemove.Count == 0)
			{
				return;
			}

			var confirmed = itemsToRemove.Count == 1
				? await _stateDefinitionDialogService.ConfirmDeleteStateItemAsync(itemsToRemove[0].Name)
				: await _stateDefinitionDialogService.ConfirmDeleteStateItemsAsync(itemsToRemove.Count);
			
			if (!confirmed)
			{
				return;
			}

			SelectedStateDefinition.RemoveItems(itemsToRemove);
			SelectSingleStateItem(SelectedStateDefinition.SelectedItem);
			RaisePropertyChanged(nameof(SelectedStateItem));
			OnStateDataChanged();
		}

		private bool CanRemoveStateItem()
		{
			return GetSelectedStateItemsForRemoval().Count > 0;
		}

		private void MoveStateItemUp()
		{
			MoveSelectedStateItem(-1);
		}

		private bool CanMoveStateItemUp()
		{
			return SelectedStateDefinition != null &&
				SelectedStateItem != null &&
				SelectedStateDefinition.Items.IndexOf(SelectedStateItem) > 0;
		}

		private void MoveStateItemDown()
		{
			MoveSelectedStateItem(1);
		}

		private bool CanMoveStateItemDown()
		{
			return SelectedStateDefinition != null &&
				SelectedStateItem != null &&
				SelectedStateDefinition.Items.IndexOf(SelectedStateItem) < SelectedStateDefinition.Items.Count - 1;
		}

		private void MoveSelectedStateItem(int offset)
		{
			if (SelectedStateDefinition == null || SelectedStateItem == null)
			{
				return;
			}

			SelectedStateDefinition.MoveItem(SelectedStateItem, offset);
			SelectSingleStateItem(SelectedStateDefinition.SelectedItem);
			RaisePropertyChanged(nameof(SelectedStateItem));
			OnStateDataChanged();
		}

		private void PersistStateItemSortOrder(IList sortedItems)
		{
			if (SelectedStateDefinition == null || sortedItems == null)
			{
				return;
			}

			SelectedStateDefinition.ApplyItemOrder(sortedItems.OfType<CustomPropStateItemViewModel>());
		}

		private bool CanPersistStateItemSortOrder(IList sortedItems)
		{
			return SelectedStateDefinition != null && sortedItems != null;
		}

		private IReadOnlyList<CustomPropStateItemViewModel> GetSelectedStateItemsForRemoval()
		{
			if (SelectedStateDefinition == null)
			{
				return [];
			}

			var selectedItems = SelectedStateItems
				.Where(item => SelectedStateDefinition.Items.Contains(item))
				.Distinct()
				.ToList();
			if (selectedItems.Count > 0)
			{
				return selectedItems;
			}

			return SelectedStateItem != null && SelectedStateDefinition.Items.Contains(SelectedStateItem)
				? [SelectedStateItem]
				: [];
		}

		private void SelectSingleStateItem(CustomPropStateItemViewModel item)
		{
			SelectedStateItems.Clear();
			if (item != null)
			{
				SelectedStateItems.Add(item);
			}

			RaisePropertyChanged(nameof(SelectedStateItems));
			RaisePropertyChanged(nameof(CanEditCanvasAssignments));
		}

		private CustomPropStateDefinitionViewModel CreateDefinitionViewModel(StateDefinitionModel stateDefinition)
		{
			return new CustomPropStateDefinitionViewModel(stateDefinition, Prop, OnStateDataChanged);
		}

		private void OnStateDataChanged()
		{
			IsDirty = true;
			RefreshValidation();
			StateDataChanged?.Invoke(this, EventArgs.Empty);
			StatePreviewChanged?.Invoke(this, EventArgs.Empty);
		}

		private void StateDefinitionsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(nameof(StateDefinitions));
		}

		private void SelectedStateItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			RaisePropertyChanged(nameof(SelectedStateItems));
			RaisePropertyChanged(nameof(CanEditCanvasAssignments));
			RaiseCommandCanExecuteChanged();
			StatePreviewChanged?.Invoke(this, EventArgs.Empty);
		}

		private void AddModelTypeValidationMessages()
		{
			var explicitModels = PropStateModelResolver.GetExplicitModelElements(Prop);
			if (explicitModels.Count > 1)
			{
				ValidationMessages.Add("Only one element can have Model Type set to Model.");
			}

			foreach (var explicitModel in explicitModels.Where(model => !PropStateModelResolver.CanAssignModelType(model, ElementModelType.Model)))
			{
				ValidationMessages.Add($"Element \"{explicitModel.Name}\" cannot be a Model because it is a leaf element.");
			}
		}

		private void AddStateDefinitionValidationMessages()
		{
			foreach (var stateDefinition in StateDefinitions)
			{
				if (string.IsNullOrWhiteSpace(stateDefinition.Name))
				{
					ValidationMessages.Add("State definition names cannot be empty.");
				}

				if (!stateDefinition.Items.Any())
				{
					ValidationMessages.Add($"State definition \"{GetDisplayName(stateDefinition.Name)}\" must contain at least one item.");
				}

				foreach (var item in stateDefinition.Items)
				{
					if (string.IsNullOrWhiteSpace(item.Name))
					{
						ValidationMessages.Add($"State definition \"{GetDisplayName(stateDefinition.Name)}\" contains an item with no name.");
					}

					if (!item.HasAssignments)
					{
						ValidationMessages.Add($"State item \"{GetDisplayName(item.Name)}\" in \"{GetDisplayName(stateDefinition.Name)}\" must assign at least one element.");
					}
				}
			}

			var duplicateDefinitionNames = StateDefinitions
				.GroupBy(definition => (definition.Name ?? string.Empty).Trim(), StringComparer.OrdinalIgnoreCase)
				.Where(group => !string.IsNullOrWhiteSpace(group.Key) && group.Count() > 1)
				.Select(group => group.Key);

			foreach (var duplicateDefinitionName in duplicateDefinitionNames)
			{
				ValidationMessages.Add($"State definition name \"{duplicateDefinitionName}\" is duplicated.");
			}
		}

		private IReadOnlyCollection<string> GetStateDefinitionNames()
		{
			return StateDefinitions.Select(definition => definition.Name).ToList();
		}
		
		private string GetNextStateDefinitionName()
		{
			var index = 1;
			var existingNames = GetStateDefinitionNames();
			string name;
			do
			{
				name = $"State - {index}";
				index++;
			}
			while (existingNames.Contains(name, StringComparer.Ordinal));

			return name;
		}

		private bool TryNormalizeStateDefinitionName(
			string name,
			string currentName,
			out string normalizedName)
		{
			var candidateName = name?.Trim() ?? string.Empty;
			normalizedName = candidateName;
			return !string.IsNullOrWhiteSpace(candidateName) &&
				!StateDefinitions.Any(definition =>
					!definition.Name.Equals(currentName, StringComparison.Ordinal) &&
					definition.Name.Equals(candidateName, StringComparison.Ordinal));
		}

		private static string GetUniqueStateItemName(
			string baseName,
			CustomPropStateDefinitionViewModel stateDefinition,
			CustomPropStateItemViewModel excludedItem = null)
		{
			var existingNames = stateDefinition.Items
				.Where(item => !ReferenceEquals(item, excludedItem))
				.Select(item => item.Name)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);

			return GetUniqueName(baseName, existingNames);
		}

		private static string GetUniqueName(string baseName, ISet<string> existingNames)
		{
			var normalizedBaseName = string.IsNullOrWhiteSpace(baseName)
				? StateDefinitionModel.DefaultName
				: baseName.Trim();
			if (!existingNames.Contains(normalizedBaseName))
			{
				return normalizedBaseName;
			}

			var index = 2;
			string candidate;
			do
			{
				candidate = $"{normalizedBaseName} {index}";
				index++;
			}
			while (existingNames.Contains(candidate));

			return candidate;
		}

		private static string GetDisplayName(string name)
		{
			return string.IsNullOrWhiteSpace(name) ? "<unnamed>" : name.Trim();
		}

		private void RaiseCommandCanExecuteChanged()
		{
			_deleteStateDefinitionCommand?.RaiseCanExecuteChanged();
			_renameStateDefinitionCommand?.RaiseCanExecuteChanged();
			_copyStateDefinitionCommand?.RaiseCanExecuteChanged();
			_addStateItemCommand?.RaiseCanExecuteChanged();
			_removeStateItemCommand?.RaiseCanExecuteChanged();
			_moveStateItemUpCommand?.RaiseCanExecuteChanged();
			_moveStateItemDownCommand?.RaiseCanExecuteChanged();
			_persistStateItemSortCommand?.RaiseCanExecuteChanged();
		}
	}
}

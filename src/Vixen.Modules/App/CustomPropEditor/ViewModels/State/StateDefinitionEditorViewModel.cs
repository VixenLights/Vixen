using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.ViewModels.State
{
	/// <summary>
	/// Coordinates Custom Prop Editor State definition authoring.
	/// </summary>
	public sealed class StateDefinitionEditorViewModel : ViewModelBase
	{
		private CustomPropStateDefinitionViewModel _selectedStateDefinition;
		private Command _addStateDefinitionCommand;
		private Command _deleteStateDefinitionCommand;
		private Command _renameStateDefinitionCommand;
		private Command _copyStateDefinitionCommand;
		private Command _addStateItemCommand;
		private Command _removeStateItemCommand;
		private Command _moveStateItemUpCommand;
		private Command _moveStateItemDownCommand;

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
		public StateDefinitionEditorViewModel(Prop prop)
		{
			StateDefinitions = new ObservableCollection<CustomPropStateDefinitionViewModel>();
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
				RaisePropertyChanged(nameof(SelectedStateItem));
				RaiseCommandCanExecuteChanged();
				StatePreviewChanged?.Invoke(this, EventArgs.Empty);
			}
		}

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
		public Command AddStateDefinitionCommand =>
			_addStateDefinitionCommand ??= new Command(AddStateDefinition);

		/// <summary>
		/// Gets the Delete State definition command.
		/// </summary>
		public Command DeleteStateDefinitionCommand =>
			_deleteStateDefinitionCommand ??= new Command(DeleteStateDefinition, CanDeleteStateDefinition);

		/// <summary>
		/// Gets the Rename State definition command.
		/// </summary>
		public Command RenameStateDefinitionCommand =>
			_renameStateDefinitionCommand ??= new Command(RenameStateDefinition, CanRenameStateDefinition);

		/// <summary>
		/// Gets the Copy State definition command.
		/// </summary>
		public Command CopyStateDefinitionCommand =>
			_copyStateDefinitionCommand ??= new Command(CopyStateDefinition, CanCopyStateDefinition);

		/// <summary>
		/// Gets the Add State item command.
		/// </summary>
		public Command AddStateItemCommand =>
			_addStateItemCommand ??= new Command(AddStateItem, CanAddStateItem);

		/// <summary>
		/// Gets the Remove State item command.
		/// </summary>
		public Command RemoveStateItemCommand =>
			_removeStateItemCommand ??= new Command(RemoveStateItem, CanRemoveStateItem);

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
		/// Refreshes State assignment trees after the prop element model changes.
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

		private void AddStateDefinition()
		{
			var stateDefinition = StateDefinitionModel.CreateDefault(GetUniqueStateDefinitionName(StateDefinitionModel.DefaultName));
			ModelElement.StateDefinitionModels.Add(stateDefinition);
			var stateDefinitionViewModel = CreateDefinitionViewModel(stateDefinition);
			StateDefinitions.Add(stateDefinitionViewModel);
			SelectedStateDefinition = stateDefinitionViewModel;
			OnStateDataChanged();
		}

		private void DeleteStateDefinition()
		{
			if (SelectedStateDefinition == null)
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

		private void RenameStateDefinition()
		{
			if (SelectedStateDefinition == null)
			{
				return;
			}

			SelectedStateDefinition.Name = GetUniqueStateDefinitionName(SelectedStateDefinition.Name, SelectedStateDefinition);
			OnStateDataChanged();
		}

		private bool CanRenameStateDefinition()
		{
			return SelectedStateDefinition != null;
		}

		private void CopyStateDefinition()
		{
			if (SelectedStateDefinition == null)
			{
				return;
			}

			var copy = new StateDefinitionModel
			{
				Name = GetUniqueStateDefinitionName($"{SelectedStateDefinition.Name} Copy"),
				Description = SelectedStateDefinition.Description,
				Items = new ObservableCollection<StateItemModel>(
					SelectedStateDefinition.Items.Select(item => new StateItemModel
					{
						Name = item.Name,
						Color = item.Color,
						ElementModelIds = new ObservableCollection<Guid>(item.StateItem.ElementModelIds.Distinct())
					}))
			};

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
			RaisePropertyChanged(nameof(SelectedStateItem));
			OnStateDataChanged();
		}

		private bool CanAddStateItem()
		{
			return SelectedStateDefinition != null;
		}

		private void RemoveStateItem()
		{
			if (SelectedStateDefinition == null || SelectedStateItem == null)
			{
				return;
			}

			SelectedStateDefinition.RemoveItem(SelectedStateItem);
			RaisePropertyChanged(nameof(SelectedStateItem));
			OnStateDataChanged();
		}

		private bool CanRemoveStateItem()
		{
			return SelectedStateItem != null;
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
			RaisePropertyChanged(nameof(SelectedStateItem));
			OnStateDataChanged();
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

		private string GetUniqueStateDefinitionName(string baseName, CustomPropStateDefinitionViewModel excludedDefinition = null)
		{
			var existingNames = StateDefinitions
				.Where(definition => !ReferenceEquals(definition, excludedDefinition))
				.Select(definition => definition.Name)
				.ToHashSet(StringComparer.OrdinalIgnoreCase);

			return GetUniqueName(baseName, existingNames);
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
		}
	}
}

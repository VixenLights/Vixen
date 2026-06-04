using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys;
using VixenModules.Property.State.Setup.Models;
using VixenModules.Property.State.Setup.Preview;
using VixenModules.Property.State.Setup.Services;
using WPFCommon.Extensions;

namespace VixenModules.Property.State.Setup.ViewModels
{
	/// <summary>
	/// Provides the editable draft used by the State property mapping window.
	/// </summary>
	public sealed class StateMapperViewModel : ViewModelBase
	{
		private const string FormTitle = "State Mapper";
		private const string AllStateItemGroups = "<ALL>";
		private readonly StateData _source;
		private readonly IElementNode _rootNode;
		private readonly Dictionary<Guid, IElementNode> _nodesById;
		private readonly StateElementNodeSnapshot _elementTree;
		private readonly IStateColorPickerService _colorPickerService;
		private readonly IStateDefinitionDialogService _stateDefinitionDialogService;
		private readonly StatePreviewCoordinator _previewCoordinator;
		private bool _suppressPreviewRefresh;
		private bool _suppressStateDefinitionSelection;
		private StateDefinitionViewModel? _lastValidSelectedStateDefinition;
		private TaskCommand? _addStateDefinitionCommand;
		private TaskCommand? _deleteStateDefinitionCommand;
		private TaskCommand? _renameStateDefinitionCommand;
		private TaskCommand? _copyStateDefinitionCommand;
		private TaskCommand? _addItemCommand;
		private TaskCommand? _removeItemCommand;
		private TaskCommand<StateItemViewModel>? _editColorCommand;
		private TaskCommand? _okCommand;
		private TaskCommand? _cancelCommand;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateMapperViewModel"/> class.
		/// </summary>
		/// <param name="rootNode">The selected element node whose hierarchy can be assigned to states.</param>
		/// <param name="source">The persisted State property data to update when the draft is saved.</param>
		/// <param name="colorPickerService">The service that displays the applicable Vixen color chooser.</param>
		public StateMapperViewModel(
			IElementNode rootNode,
			StateData source,
			IStateColorPickerService colorPickerService)
			: this(
				rootNode,
				source,
				colorPickerService,
				new BroadcastStatePreviewPublisher(),
				new StateDefinitionDialogService())
		{
		}

		internal StateMapperViewModel(
			IElementNode rootNode,
			StateData source,
			IStateColorPickerService colorPickerService,
			IStatePreviewPublisher previewPublisher)
			: this(rootNode, source, colorPickerService, previewPublisher, new StateDefinitionDialogService())
		{
		}

		internal StateMapperViewModel(
			IElementNode rootNode,
			StateData source,
			IStateColorPickerService colorPickerService,
			IStatePreviewPublisher previewPublisher,
			IStateDefinitionDialogService stateDefinitionDialogService)
		{
			ArgumentNullException.ThrowIfNull(rootNode);
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(colorPickerService);
			ArgumentNullException.ThrowIfNull(previewPublisher);
			ArgumentNullException.ThrowIfNull(stateDefinitionDialogService);

			_rootNode = rootNode;
			_source = source;
			_colorPickerService = colorPickerService;
			_stateDefinitionDialogService = stateDefinitionDialogService;
			_previewCoordinator = new StatePreviewCoordinator(previewPublisher);
			var draft = (StateData)source.Clone();
			draft.Normalize();
			DeferValidationUntilFirstSaveCall = false;
			_elementTree = StateElementNodeSnapshot.FromElementNode(rootNode);
			_nodesById = rootNode
				.GetNodeEnumerator()
				.Prepend(rootNode)
				.DistinctBy(node => node.Id)
				.ToDictionary(node => node.Id);
			StateDefinitions = new ObservableCollection<StateDefinitionViewModel>(
				draft.StateDefinitions.Select(definition => new StateDefinitionViewModel(definition, _elementTree)));
			AvailableStateItemGroups = [];
			StateDefinitions.CollectionChanged += StateDefinitionsCollectionChanged;
			foreach (var definition in StateDefinitions)
			{
				AttachDefinition(definition);
			}

			Title = FormTitle;
			Draft = draft;
			SelectedStateDefinition = StateDefinitions.FirstOrDefault();
			SelectItem(Items.FirstOrDefault(), false);
			_lastValidSelectedStateDefinition = SelectedStateDefinition;
			IsStateItemGroupPreviewMode = true;
			Validate(true);
		}

		[Model]
		internal StateData Draft
		{
			get => GetValue<StateData>(DraftProperty);
			private set => SetValue(DraftProperty, value);
		}

		private static readonly IPropertyData DraftProperty = RegisterProperty<StateData>(nameof(Draft));

		/// <summary>
		/// Gets or sets the title displayed by the mapping window.
		/// </summary>
		/// <value>The title displayed by the mapping window.</value>
		public new string Title
		{
			get => GetValue<string>(TitleProperty);
			set => SetValue(TitleProperty, value);
		}

		/// <summary>
		/// Identifies the <see cref="Title"/> property.
		/// </summary>
		public static readonly IPropertyData TitleProperty = RegisterProperty<string>(nameof(Title));

		/// <summary>
		/// Gets or sets the name of the selected State definition.
		/// </summary>
		/// <value>The name of the selected State definition.</value>
		public string Name
		{
			get => GetValue<string>(NameProperty);
			set
			{
				var normalizedName = value?.Trim() ?? string.Empty;
				SetValue(NameProperty, normalizedName);
				if (SelectedStateDefinition != null)
				{
					SelectedStateDefinition.Name = normalizedName;
				}

				ValidateAndRefreshOkCommand();
			}
		}

		/// <summary>
		/// Identifies the <see cref="Name"/> property.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name), string.Empty);

		/// <summary>
		/// Gets or sets the description of the selected State definition.
		/// </summary>
		/// <value>The description of the selected State definition.</value>
		public string Description
		{
			get => GetValue<string>(DescriptionProperty);
			set
			{
				SetValue(DescriptionProperty, value);
				if (SelectedStateDefinition != null)
				{
					SelectedStateDefinition.Description = value;
				}
			}
		}

		/// <summary>
		/// Identifies the <see cref="Description"/> property.
		/// </summary>
		public static readonly IPropertyData DescriptionProperty = RegisterProperty<string>(nameof(Description), string.Empty);

		/// <summary>
		/// Gets the editable State definitions.
		/// </summary>
		/// <value>The editable State definitions.</value>
		public ObservableCollection<StateDefinitionViewModel> StateDefinitions { get; }

		/// <summary>
		/// Gets or sets the selected State definition.
		/// </summary>
		/// <value>The selected State definition.</value>
		public StateDefinitionViewModel? SelectedStateDefinition
		{
			get => GetValue<StateDefinitionViewModel?>(SelectedStateDefinitionProperty);
			set
			{
				if (ReferenceEquals(GetValue<StateDefinitionViewModel?>(SelectedStateDefinitionProperty), value))
				{
					return;
				}

				if (!_suppressStateDefinitionSelection &&
					_lastValidSelectedStateDefinition != null &&
					HasBlockingValidationErrors())
				{
					_suppressStateDefinitionSelection = true;
					try
					{
						SetValue(SelectedStateDefinitionProperty, _lastValidSelectedStateDefinition);
					}
					finally
					{
						_suppressStateDefinitionSelection = false;
					}

					RaisePropertyChanged(nameof(SelectedStateDefinition));
					return;
				}

				SetValue(SelectedStateDefinitionProperty, value);
				_lastValidSelectedStateDefinition = value;
				ApplySelectedStateDefinition();
			}
		}

		/// <summary>
		/// Identifies the <see cref="SelectedStateDefinition"/> property.
		/// </summary>
		public static readonly IPropertyData SelectedStateDefinitionProperty =
			RegisterProperty<StateDefinitionViewModel?>(nameof(SelectedStateDefinition));

		/// <summary>
		/// Gets the editable state item rows.
		/// </summary>
		/// <value>The editable state item rows.</value>
		public ObservableCollection<StateItemViewModel> Items => SelectedStateDefinition?.Items ?? [];

		/// <summary>
		/// Gets or sets the row whose assignment tree is displayed.
		/// </summary>
		/// <value>The row whose assignment tree is displayed.</value>
		public StateItemViewModel? SelectedItem
		{
			get => GetValue<StateItemViewModel?>(SelectedItemProperty);
			set => SelectItem(value, true);
		}

		/// <summary>
		/// Identifies the <see cref="SelectedItem"/> property.
		/// </summary>
		public static readonly IPropertyData SelectedItemProperty = RegisterProperty<StateItemViewModel?>(nameof(SelectedItem));

		/// <summary>
		/// Gets or sets a value that indicates whether State preview output is enabled.
		/// </summary>
		/// <value><see langword="true" /> if State preview output is enabled; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
		public bool IsPreviewEnabled
		{
			get => GetValue<bool>(IsPreviewEnabledProperty);
			set
			{
				if (GetValue<bool>(IsPreviewEnabledProperty) == value)
				{
					return;
				}

				SetValue(IsPreviewEnabledProperty, value);
				if (value)
				{
					RefreshPreview();
				}
				else
				{
					_previewCoordinator.Clear();
				}
			}
		}

		/// <summary>
		/// Identifies the <see cref="IsPreviewEnabled"/> property.
		/// </summary>
		public static readonly IPropertyData IsPreviewEnabledProperty = RegisterProperty<bool>(nameof(IsPreviewEnabled));

		/// <summary>
		/// Gets or sets a value that indicates whether preview includes a State Item Group instead of the selected row.
		/// </summary>
		/// <value><see langword="true" /> if preview includes a State Item Group; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
		public bool IsStateItemGroupPreviewMode
		{
			get => GetValue<bool>(IsStateItemGroupPreviewModeProperty);
			set
			{
				if (GetValue<bool>(IsStateItemGroupPreviewModeProperty) == value)
				{
					return;
				}

				SetValue(IsStateItemGroupPreviewModeProperty, value);
				if (!IsPreviewEnabled)
				{
					return;
				}

				_previewCoordinator.Clear();
				RefreshPreview();
			}
		}

		/// <summary>
		/// Identifies the <see cref="IsStateItemGroupPreviewMode"/> property.
		/// </summary>
		public static readonly IPropertyData IsStateItemGroupPreviewModeProperty =
			RegisterProperty<bool>(nameof(IsStateItemGroupPreviewMode));

		/// <summary>
		/// Gets the State Item Group choices available for preview.
		/// </summary>
		/// <value>The State Item Group choices available for preview.</value>
		public ObservableCollection<string> AvailableStateItemGroups { get; }

		/// <summary>
		/// Gets or sets the State Item Group selected for preview.
		/// </summary>
		/// <value>The State Item Group selected for preview. The default is <c>&lt;ALL&gt;</c>.</value>
		public string SelectedStateItemGroup
		{
			get => GetValue<string>(SelectedStateItemGroupProperty);
			set
			{
				var normalizedValue = string.IsNullOrWhiteSpace(value)
					? AllStateItemGroups
					: value.Trim();
				if (GetValue<string>(SelectedStateItemGroupProperty) == normalizedValue)
				{
					return;
				}

				SetValue(SelectedStateItemGroupProperty, normalizedValue);
				if (IsPreviewEnabled && IsStateItemGroupPreviewMode)
				{
					RefreshPreview();
				}
			}
		}

		/// <summary>
		/// Identifies the <see cref="SelectedStateItemGroup"/> property.
		/// </summary>
		public static readonly IPropertyData SelectedStateItemGroupProperty =
			RegisterProperty(nameof(SelectedStateItemGroup), AllStateItemGroups);

		/// <summary>
		/// Gets the command that adds a state item row.
		/// </summary>
		/// <value>The command that adds a state item row.</value>
		public TaskCommand AddItemCommand => _addItemCommand ??= new TaskCommand(AddItemAsync, CanAddItem);

		/// <summary>
		/// Gets the command that adds a State definition.
		/// </summary>
		/// <value>The command that adds a State definition.</value>
		public TaskCommand AddStateDefinitionCommand =>
			_addStateDefinitionCommand ??= new TaskCommand(AddStateDefinitionAsync, CanAddStateDefinition);

		/// <summary>
		/// Gets the command that deletes the selected State definition.
		/// </summary>
		/// <value>The command that deletes the selected State definition.</value>
		public TaskCommand DeleteStateDefinitionCommand =>
			_deleteStateDefinitionCommand ??= new TaskCommand(DeleteStateDefinitionAsync, CanDeleteStateDefinition);

		/// <summary>
		/// Gets the command that renames the selected State definition.
		/// </summary>
		/// <value>The command that renames the selected State definition.</value>
		public TaskCommand RenameStateDefinitionCommand =>
			_renameStateDefinitionCommand ??= new TaskCommand(RenameStateDefinitionAsync, CanRenameStateDefinition);

		/// <summary>
		/// Gets the command that copies the selected State definition.
		/// </summary>
		/// <value>The command that copies the selected State definition.</value>
		public TaskCommand CopyStateDefinitionCommand =>
			_copyStateDefinitionCommand ??= new TaskCommand(CopyStateDefinitionAsync, CanCopyStateDefinition);

		/// <summary>
		/// Gets the command that removes the selected state item row.
		/// </summary>
		/// <value>The command that removes the selected state item row.</value>
		public TaskCommand RemoveItemCommand => _removeItemCommand ??= new TaskCommand(RemoveItemAsync, CanRemoveItem);

		/// <summary>
		/// Gets the command that edits a state item color.
		/// </summary>
		/// <value>The command that edits a state item color.</value>
		public TaskCommand<StateItemViewModel> EditColorCommand =>
			_editColorCommand ??= new TaskCommand<StateItemViewModel>(EditColorAsync, CanEditColor);

		/// <summary>
		/// Gets the command that saves the draft and closes the window.
		/// </summary>
		/// <value>The command that saves the draft and closes the window.</value>
		public TaskCommand OkCommand => _okCommand ??= new TaskCommand(OkAsync, CanOk);

		/// <summary>
		/// Gets the command that discards the draft and closes the window.
		/// </summary>
		/// <value>The command that discards the draft and closes the window.</value>
		public TaskCommand CancelCommand => _cancelCommand ??= new TaskCommand(CancelMapAsync, CanCancel);

		/// <inheritdoc />
		protected override Task<bool> SaveAsync()
		{
			NormalizeNames();
			Validate(true);
			if (HasBlockingValidationErrors())
			{
				RaiseOkCanExecuteChanged();
				return Task.FromResult(false);
			}

			_source.StateDefinitions = StateDefinitions.Select(definition => definition.ToData()).ToList();
			return Task.FromResult(true);
		}

		/// <inheritdoc />
		protected override async Task OnClosedAsync(bool? result)
		{
			_previewCoordinator.Release();
			await base.OnClosedAsync(result);
		}

		private bool CanAddStateDefinition() => !HasBlockingValidationErrors();

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

			var definition = new StateDefinitionViewModel(
				StateDefinitionData.CreateDefault(normalizedName),
				_elementTree);
			StateDefinitions.Add(definition);
			SelectedStateDefinition = definition;
			ValidateAndRefreshOkCommand();
		}

		private bool CanDeleteStateDefinition() => SelectedStateDefinition != null && StateDefinitions.Count > 1;

		private async Task DeleteStateDefinitionAsync()
		{
			if (SelectedStateDefinition == null ||
				StateDefinitions.Count <= 1 ||
				!await _stateDefinitionDialogService.ConfirmDeleteAsync(SelectedStateDefinition.Name))
			{
				return;
			}

			var index = StateDefinitions.IndexOf(SelectedStateDefinition);
			StateDefinitions.Remove(SelectedStateDefinition);
			SelectedStateDefinition = StateDefinitions[Math.Min(index, StateDefinitions.Count - 1)];
			ValidateAndRefreshOkCommand();
		}

		private bool CanRenameStateDefinition() => SelectedStateDefinition != null && !HasBlockingValidationErrors();

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
			ApplySelectedStateDefinitionValues();
			ValidateAndRefreshOkCommand();
		}

		private bool CanCopyStateDefinition() => SelectedStateDefinition != null && !HasBlockingValidationErrors();

		private async Task CopyStateDefinitionAsync()
		{
			if (SelectedStateDefinition == null)
			{
				return;
			}

			var name = await _stateDefinitionDialogService.RequestNameAsync(
				"Copy State Definition",
				GetNextStateDefinitionName(),
				GetStateDefinitionNames(),
				null);
			if (!TryNormalizeStateDefinitionName(name, null, out var normalizedName))
			{
				return;
			}

			var copiedDefinition = SelectedStateDefinition.ToData().CloneAsNew(normalizedName);
			var definition = new StateDefinitionViewModel(copiedDefinition, _elementTree);
			StateDefinitions.Add(definition);
			SelectedStateDefinition = definition;
			ValidateAndRefreshOkCommand();
		}

		private bool CanAddItem() => SelectedStateDefinition != null;

		private Task AddItemAsync()
		{
			if (SelectedStateDefinition == null)
			{
				return Task.CompletedTask;
			}

			_suppressPreviewRefresh = true;
			try
			{
				var item = new StateItemViewModel(new StateItemData(), _elementTree);
				Items.Add(item);
				SelectedItem = item;
			}
			finally
			{
				_suppressPreviewRefresh = false;
			}

			RefreshPreview();
			RemoveItemCommand.RaiseCanExecuteChanged();
			ValidateAndRefreshOkCommand();
			return Task.CompletedTask;
		}

		private bool CanRemoveItem() => SelectedStateDefinition != null && SelectedItem != null;

		private Task RemoveItemAsync()
		{
			if (SelectedItem == null)
			{
				return Task.CompletedTask;
			}

			_suppressPreviewRefresh = true;
			try
			{
				var index = Items.IndexOf(SelectedItem);
				Items.Remove(SelectedItem);
				SelectedItem = Items.Count == 0
					? null
					: Items[Math.Min(index, Items.Count - 1)];
			}
			finally
			{
				_suppressPreviewRefresh = false;
			}

			RefreshPreview();
			RemoveItemCommand.RaiseCanExecuteChanged();
			ValidateAndRefreshOkCommand();
			return Task.CompletedTask;
		}

		private static bool CanEditColor(StateItemViewModel? item) => item != null;

		private Task EditColorAsync(StateItemViewModel? item)
		{
			if (item == null)
			{
				return Task.CompletedTask;
			}

			var nodes = item.AssignmentRoots
				.SelectMany(root => root.GetEffectiveLeafNodeIds())
				.Select(id => _nodesById.GetValueOrDefault(id))
				.Where(node => node != null)
				.Cast<IElementNode>()
				.ToList();
			var selectedColor = _colorPickerService.ChooseColor(
				nodes.Count > 0 ? nodes : [_rootNode],
				item.Color);
			if (selectedColor.HasValue)
			{
				item.Color = selectedColor.Value;
			}

			return Task.CompletedTask;
		}

		/// <inheritdoc />
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			if (string.IsNullOrWhiteSpace(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError(NameProperty, "State definition name is required."));
			}
			else if (Name.Length < 3)
			{
				validationResults.Add(FieldValidationResult.CreateWarning(NameProperty, "State definition names shorter than three characters may be unclear."));
			}
		}

		/// <inheritdoc />
		protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
		{
			if (Items.Any(item => string.IsNullOrWhiteSpace(item.Name)))
			{
				validationResults.Add(BusinessRuleValidationResult.CreateError("Every State item requires a name."));
			}

			var duplicateNames = StateDefinitions
				.GroupBy(definition => definition.Name, StringComparer.Ordinal)
				.Where(group => !string.IsNullOrWhiteSpace(group.Key) && group.Skip(1).Any())
				.Select(group => group.Key)
				.ToList();
			if (duplicateNames.Count > 0)
			{
				validationResults.Add(BusinessRuleValidationResult.CreateError("State definition names must be unique."));
			}

			var hasCaseOnlyDuplicates = StateDefinitions
				.GroupBy(definition => definition.Name, StringComparer.OrdinalIgnoreCase)
				.Any(group => group
					.Select(definition => definition.Name)
					.Distinct(StringComparer.Ordinal)
					.Skip(1)
					.Any());
			if (hasCaseOnlyDuplicates)
			{
				validationResults.Add(BusinessRuleValidationResult.CreateWarning("State definition names differ only by casing. Check you don't have a typo."));
			}
		}

		private bool CanOk() => !HasBlockingValidationErrors();

		private Task OkAsync() => this.SaveAndCloseViewModelAsync();

		private bool CanCancel() => true;

		private Task CancelMapAsync() => this.CancelAndCloseViewModelAsync();

		private void StateDefinitionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (StateDefinitionViewModel definition in e.OldItems)
				{
					DetachDefinition(definition);
				}
			}

			if (e.NewItems != null)
			{
				foreach (StateDefinitionViewModel definition in e.NewItems)
				{
					AttachDefinition(definition);
				}
			}

			_deleteStateDefinitionCommand?.RaiseCanExecuteChanged();
			ValidateAndRefreshOkCommand();
		}

		private void AttachDefinition(StateDefinitionViewModel definition)
		{
			definition.PropertyChanged += DefinitionPropertyChanged;
			definition.Items.CollectionChanged += ItemsCollectionChanged;
			foreach (var item in definition.Items)
			{
				AttachItem(item);
			}
		}

		private void DetachDefinition(StateDefinitionViewModel definition)
		{
			definition.PropertyChanged -= DefinitionPropertyChanged;
			definition.Items.CollectionChanged -= ItemsCollectionChanged;
			foreach (var item in definition.Items)
			{
				DetachItem(item);
			}
		}

		private void DefinitionPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(StateDefinitionViewModel.Name))
			{
				if (ReferenceEquals(sender, SelectedStateDefinition) &&
					SelectedStateDefinition != null)
				{
					SetValue(NameProperty, SelectedStateDefinition.Name);
				}

				ValidateAndRefreshOkCommand();
			}

			if (e.PropertyName == nameof(StateDefinitionViewModel.Description) &&
				ReferenceEquals(sender, SelectedStateDefinition) &&
				SelectedStateDefinition != null)
			{
				SetValue(DescriptionProperty, SelectedStateDefinition.Description);
			}
		}

		private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.OldItems != null)
			{
				foreach (StateItemViewModel item in e.OldItems)
				{
					DetachItem(item);
				}
			}

			if (e.NewItems != null)
			{
				foreach (StateItemViewModel item in e.NewItems)
				{
					AttachItem(item);
				}
			}

			var isSelectedDefinitionItems = ReferenceEquals(sender, SelectedStateDefinition?.Items);
			if (!_suppressPreviewRefresh &&
				isSelectedDefinitionItems &&
				SelectedItem != null &&
				!Items.Contains(SelectedItem))
			{
				SelectedItem = Items.FirstOrDefault();
			}

			if (isSelectedDefinitionItems)
			{
				RebuildAvailableStateItemGroups();
				RefreshPreview();
				RaisePropertyChanged(nameof(Items));
			}

			ValidateAndRefreshOkCommand();
		}

		private void AttachItem(StateItemViewModel item)
		{
			item.PropertyChanged += ItemPropertyChanged;
		}

		private void DetachItem(StateItemViewModel item)
		{
			item.PropertyChanged -= ItemPropertyChanged;
		}

		private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(StateItemViewModel.Name))
			{
				if (IsSelectedDefinitionItem(sender))
				{
					RebuildAvailableStateItemGroups();
				}

				ValidateAndRefreshOkCommand();
			}

			if (sender is StateItemViewModel item &&
				IsSelectedDefinitionItem(item) &&
				(e.PropertyName == nameof(StateItemViewModel.Name) ||
					e.PropertyName is nameof(StateItemViewModel.Color) or nameof(StateItemViewModel.AssignmentCount) &&
					IsPreviewedItem(item)))
			{
				RefreshPreview();
			}
		}

		private void NormalizeNames()
		{
			foreach (var definition in StateDefinitions)
			{
				definition.Name = definition.Name;
				foreach (var item in definition.Items)
				{
					item.Name = item.Name;
				}
			}

			ApplySelectedStateDefinitionValues();
		}

		private bool HasBlockingValidationErrors()
		{
			return StateDefinitions.Count == 0 ||
				StateDefinitions.Any(definition =>
					string.IsNullOrWhiteSpace(definition.Name) ||
					definition.Items.Any(item => string.IsNullOrWhiteSpace(item.Name))) ||
				StateDefinitions
					.GroupBy(definition => definition.Name, StringComparer.Ordinal)
					.Any(group => !string.IsNullOrWhiteSpace(group.Key) && group.Skip(1).Any());
		}

		private void ValidateAndRefreshOkCommand()
		{
			Validate(true);
			RaiseOkCanExecuteChanged();
		}

		private void RaiseOkCanExecuteChanged()
		{
			_okCommand?.RaiseCanExecuteChanged();
			_addStateDefinitionCommand?.RaiseCanExecuteChanged();
			_deleteStateDefinitionCommand?.RaiseCanExecuteChanged();
			_renameStateDefinitionCommand?.RaiseCanExecuteChanged();
			_copyStateDefinitionCommand?.RaiseCanExecuteChanged();
			_addItemCommand?.RaiseCanExecuteChanged();
			_removeItemCommand?.RaiseCanExecuteChanged();
		}

		private void RebuildAvailableStateItemGroups()
		{
			var selectedGroup = SelectedStateItemGroup;
			var groups = Items
				.Select(item => item.Name.Trim())
				.Where(name => !string.IsNullOrEmpty(name))
				.Distinct(StringComparer.Ordinal)
				.ToList();

			var previousSuppressPreviewRefresh = _suppressPreviewRefresh;
			_suppressPreviewRefresh = true;
			try
			{
				SynchronizeAvailableStateItemGroups(groups);

				SelectedStateItemGroup = groups.Contains(selectedGroup, StringComparer.Ordinal)
					? selectedGroup
					: AllStateItemGroups;
			}
			finally
			{
				_suppressPreviewRefresh = previousSuppressPreviewRefresh;
			}
		}

		private void SynchronizeAvailableStateItemGroups(IReadOnlyList<string> groups)
		{
			if (AvailableStateItemGroups.Count == 0)
			{
				AvailableStateItemGroups.Add(AllStateItemGroups);
			}
			else if (!AvailableStateItemGroups[0].Equals(AllStateItemGroups, StringComparison.Ordinal))
			{
				AvailableStateItemGroups.Insert(0, AllStateItemGroups);
			}

			for (var desiredIndex = 0; desiredIndex < groups.Count; desiredIndex++)
			{
				var group = groups[desiredIndex];
				var collectionIndex = desiredIndex + 1;
				if (AvailableStateItemGroups.Count > collectionIndex &&
					AvailableStateItemGroups[collectionIndex].Equals(group, StringComparison.Ordinal))
				{
					continue;
				}

				var existingIndex = IndexOfAvailableStateItemGroup(group, collectionIndex + 1);
				if (existingIndex >= 0)
				{
					AvailableStateItemGroups.Move(existingIndex, collectionIndex);
				}
				else
				{
					AvailableStateItemGroups.Insert(collectionIndex, group);
				}
			}

			while (AvailableStateItemGroups.Count > groups.Count + 1)
			{
				AvailableStateItemGroups.RemoveAt(AvailableStateItemGroups.Count - 1);
			}
		}

		private int IndexOfAvailableStateItemGroup(string group, int startIndex)
		{
			for (var i = startIndex; i < AvailableStateItemGroups.Count; i++)
			{
				if (AvailableStateItemGroups[i].Equals(group, StringComparison.Ordinal))
				{
					return i;
				}
			}

			return -1;
		}

		private void RefreshPreview()
		{
			if (!IsPreviewEnabled || _suppressPreviewRefresh)
			{
				return;
			}

			var pairs = GetPreviewedItems()
				.SelectMany(item => item.AssignmentRoots
					.SelectMany(root => root.GetEffectiveLeafNodeIds())
					.Select(id => new StatePreviewPair(id, item.Color.ToHex())))
				.Distinct();
			_previewCoordinator.Refresh(pairs);
		}

		private IEnumerable<StateItemViewModel> GetPreviewedItems()
		{
			if (!IsStateItemGroupPreviewMode)
			{
				return SelectedItem == null ? [] : [SelectedItem];
			}

			return SelectedStateItemGroup == AllStateItemGroups
				? Items
				: Items.Where(item => item.Name.Equals(SelectedStateItemGroup, StringComparison.Ordinal));
		}

		private bool IsPreviewedItem(StateItemViewModel item)
		{
			return GetPreviewedItems().Contains(item);
		}

		private bool IsSelectedDefinitionItem(object? item)
		{
			return item is StateItemViewModel stateItem &&
				SelectedStateDefinition?.Items.Contains(stateItem) == true;
		}

		private void SelectItem(StateItemViewModel? item, bool expandCheckedAssignments)
		{
			if (ReferenceEquals(GetValue<StateItemViewModel?>(SelectedItemProperty), item))
			{
				return;
			}

			SetValue(SelectedItemProperty, item);
			if (expandCheckedAssignments)
			{
				item?.ExpandCheckedAssignments();
			}

			_removeItemCommand?.RaiseCanExecuteChanged();
			if (!IsStateItemGroupPreviewMode)
			{
				RefreshPreview();
			}
		}

		private void ApplySelectedStateDefinition()
		{
			ApplySelectedStateDefinitionValues();
			var previousSuppressPreviewRefresh = _suppressPreviewRefresh;
			_suppressPreviewRefresh = true;
			try
			{
				SelectItem(null, false);
				SelectedStateItemGroup = AllStateItemGroups;
				RebuildAvailableStateItemGroups();
				RaisePropertyChanged(nameof(Items));
				RaiseOkCanExecuteChanged();
			}
			finally
			{
				_suppressPreviewRefresh = previousSuppressPreviewRefresh;
			}

			if (IsPreviewEnabled)
			{
				_previewCoordinator.Clear();
				RefreshPreview();
			}
		}

		private void ApplySelectedStateDefinitionValues()
		{
			SetValue(NameProperty, SelectedStateDefinition?.Name ?? string.Empty);
			SetValue(DescriptionProperty, SelectedStateDefinition?.Description ?? string.Empty);
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
			string? name,
			string? currentName,
			out string normalizedName)
		{
			var candidateName = name?.Trim() ?? string.Empty;
			normalizedName = candidateName;
			return !string.IsNullOrWhiteSpace(candidateName) &&
				!StateDefinitions.Any(definition =>
					!definition.Name.Equals(currentName, StringComparison.Ordinal) &&
					definition.Name.Equals(candidateName, StringComparison.Ordinal));
		}
	}
}

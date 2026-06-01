using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys;
using VixenModules.Property.State.Setup.Models;
using VixenModules.Property.State.Setup.Services;

namespace VixenModules.Property.State.Setup.ViewModels
{
	/// <summary>
	/// Provides the editable draft used by the State property mapping window.
	/// </summary>
	public sealed class StateMapperViewModel : ViewModelBase
	{
		private const string FormTitle = "State Mapper";
		private readonly StateData _source;
		private readonly IElementNode _rootNode;
		private readonly Dictionary<Guid, IElementNode> _nodesById;
		private readonly StateElementNodeSnapshot _elementTree;
		private readonly IStateColorPickerService _colorPickerService;
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
		{
			ArgumentNullException.ThrowIfNull(rootNode);
			ArgumentNullException.ThrowIfNull(source);
			ArgumentNullException.ThrowIfNull(colorPickerService);

			_rootNode = rootNode;
			_source = source;
			_colorPickerService = colorPickerService;
			var draft = (StateData)source.Clone();
			DeferValidationUntilFirstSaveCall = false;
			_elementTree = StateElementNodeSnapshot.FromElementNode(rootNode);
			_nodesById = rootNode
				.GetNodeEnumerator()
				.Prepend(rootNode)
				.DistinctBy(node => node.Id)
				.ToDictionary(node => node.Id);
			Items = new ObservableCollection<StateItemViewModel>(
				draft.Items.Select(item => new StateItemViewModel(item, _elementTree)));
			Items.CollectionChanged += ItemsCollectionChanged;
			foreach (var item in Items)
			{
				AttachItem(item);
			}

			Title = FormTitle;
			SelectedItem = Items.FirstOrDefault();
			Draft = draft;
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
		/// Gets or sets the name that identifies the overall state definition.
		/// </summary>
		/// <value>The name that identifies the overall state definition.</value>
		[ViewModelToModel(nameof(Draft))]
		public string Name
		{
			get => GetValue<string>(NameProperty);
			set
			{
				var normalizedName = value?.Trim() ?? string.Empty;
				SetValue(NameProperty, normalizedName);
				ValidateAndRefreshOkCommand();
			}
		}

		/// <summary>
		/// Identifies the <see cref="Name"/> property.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name), string.Empty);

		/// <summary>
		/// Gets or sets the user-provided description of the state definition.
		/// </summary>
		/// <value>The user-provided description of the state definition.</value>
		[ViewModelToModel(nameof(Draft))]
		public string Description
		{
			get => GetValue<string>(DescriptionProperty);
			set => SetValue(DescriptionProperty, value);
		}

		/// <summary>
		/// Identifies the <see cref="Description"/> property.
		/// </summary>
		public static readonly IPropertyData DescriptionProperty = RegisterProperty<string>(nameof(Description), string.Empty);

		/// <summary>
		/// Gets the editable state item rows.
		/// </summary>
		/// <value>The editable state item rows.</value>
		public ObservableCollection<StateItemViewModel> Items { get; }

		/// <summary>
		/// Gets or sets the row whose assignment tree is displayed.
		/// </summary>
		/// <value>The row whose assignment tree is displayed.</value>
		public StateItemViewModel? SelectedItem
		{
			get => GetValue<StateItemViewModel?>(SelectedItemProperty);
			set
			{
				SetValue(SelectedItemProperty, value);
				value?.ExpandCheckedAssignments();
				_removeItemCommand?.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Identifies the <see cref="SelectedItem"/> property.
		/// </summary>
		public static readonly IPropertyData SelectedItemProperty = RegisterProperty<StateItemViewModel?>(nameof(SelectedItem));

		/// <summary>
		/// Gets the command that adds a state item row.
		/// </summary>
		/// <value>The command that adds a state item row.</value>
		public TaskCommand AddItemCommand => _addItemCommand ??= new TaskCommand(AddItemAsync, CanAddItem);

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

			_source.Name = Draft.Name;
			_source.Description = Draft.Description;
			_source.Items = Items.Select(item => item.Item.Clone()).ToList();
			return Task.FromResult(true);
		}

		private bool CanAddItem() => true;

		private Task AddItemAsync()
		{
			var item = new StateItemViewModel(new StateItemData(), _elementTree);
			Items.Add(item);
			SelectedItem = item;
			RemoveItemCommand.RaiseCanExecuteChanged();
			ValidateAndRefreshOkCommand();
			return Task.CompletedTask;
		}

		private bool CanRemoveItem() => SelectedItem != null;

		private Task RemoveItemAsync()
		{
			if (SelectedItem == null)
			{
				return Task.CompletedTask;
			}

			var index = Items.IndexOf(SelectedItem);
			Items.Remove(SelectedItem);
			SelectedItem = Items.Count == 0
				? null
				: Items[Math.Min(index, Items.Count - 1)];
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
				validationResults.Add(FieldValidationResult.CreateError(NameProperty, "State name is required."));
			}
			else if (Name.Length < 3)
			{
				validationResults.Add(FieldValidationResult.CreateWarning(NameProperty, "State names shorter than three characters may be unclear."));
			}
		}

		/// <inheritdoc />
		protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
		{
			if (Items.Any(item => string.IsNullOrWhiteSpace(item.Name)))
			{
				validationResults.Add(BusinessRuleValidationResult.CreateError("Every State item requires a name."));
			}

			var hasCaseOnlyDuplicates = Items
				.GroupBy(item => item.Name, StringComparer.OrdinalIgnoreCase)
				.Any(group => group
					.Select(item => item.Name)
					.Distinct(StringComparer.Ordinal)
					.Skip(1)
					.Any());
			if (hasCaseOnlyDuplicates)
			{
				validationResults.Add(BusinessRuleValidationResult.CreateWarning("State item names differ only by casing. Check you don't have a typo."));
			}
		}

		private bool CanOk() => !HasBlockingValidationErrors();

		private Task OkAsync() => this.SaveAndCloseViewModelAsync();

		private bool CanCancel() => true;

		private Task CancelMapAsync() => this.CancelAndCloseViewModelAsync();

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
				ValidateAndRefreshOkCommand();
			}
		}

		private void NormalizeNames()
		{
			Name = Name;
			foreach (var item in Items)
			{
				item.Name = item.Name;
			}
		}

		private bool HasBlockingValidationErrors()
		{
			return string.IsNullOrWhiteSpace(Name) ||
				Items.Any(item => string.IsNullOrWhiteSpace(item.Name));
		}

		private void ValidateAndRefreshOkCommand()
		{
			Validate(true);
			RaiseOkCanExecuteChanged();
		}

		private void RaiseOkCanExecuteChanged()
		{
			_okCommand?.RaiseCanExecuteChanged();
		}
	}
}

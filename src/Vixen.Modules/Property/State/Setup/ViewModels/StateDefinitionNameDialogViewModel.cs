using Catel.Data;
using Catel.MVVM;
using WPFCommon.Extensions;

namespace VixenModules.Property.State.Setup.ViewModels
{
	/// <summary>
	/// Provides the editable state definition name prompt.
	/// </summary>
	public sealed class StateDefinitionNameDialogViewModel : ViewModelBase
	{
		private readonly IReadOnlyCollection<string> _existingNames;
		private readonly string? _currentName;
		private TaskCommand? _okCommand;
		private TaskCommand? _cancelCommand;

		/// <summary>
		/// Initializes a new instance of the <see cref="StateDefinitionNameDialogViewModel"/> class.
		/// </summary>
		/// <param name="title">The dialog title.</param>
		/// <param name="initialName">The initial state definition name.</param>
		/// <param name="existingNames">The existing state definition names.</param>
		/// <param name="currentName">The current name to exclude from duplicate checks.</param>
		public StateDefinitionNameDialogViewModel(
			string title,
			string initialName,
			IReadOnlyCollection<string> existingNames,
			string? currentName)
		{
			Title = title;
			_existingNames = existingNames;
			_currentName = currentName;
			DeferValidationUntilFirstSaveCall = false;
			Name = initialName;
			Validate(true);
		}

		/// <summary>
		/// Gets or sets the requested state definition name.
		/// </summary>
		/// <value>The requested state definition name.</value>
		public string Name
		{
			get => GetValue<string>(NameProperty);
			set
			{
				SetValue(NameProperty, value);
				UpdateWarning();
				_okCommand?.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Identifies the <see cref="Name"/> property.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name), string.Empty);

		/// <summary>
		/// Gets the accepted state definition name.
		/// </summary>
		/// <value>The accepted state definition name.</value>
		public string? ResultName { get; private set; }

		/// <summary>
		/// Gets the non-blocking validation warning.
		/// </summary>
		/// <value>The non-blocking validation warning.</value>
		public string Warning
		{
			get => GetValue<string>(WarningProperty);
			private set => SetValue(WarningProperty, value);
		}

		/// <summary>
		/// Identifies the <see cref="Warning"/> property.
		/// </summary>
		public static readonly IPropertyData WarningProperty = RegisterProperty<string>(nameof(Warning), string.Empty);

		/// <summary>
		/// Gets the command that accepts the name and closes the window.
		/// </summary>
		/// <value>The command that accepts the name and closes the window.</value>
		public TaskCommand OkCommand => _okCommand ??= new TaskCommand(OkAsync, CanOk);

		/// <summary>
		/// Gets the command that cancels the dialog.
		/// </summary>
		/// <value>The command that cancels the dialog.</value>
		public TaskCommand CancelCommand => _cancelCommand ??= new TaskCommand(CancelDialogAsync);

		private bool CanOk() => !HasErrors;

		private Task OkAsync()
		{
			ResultName = Name.Trim();
			return this.SaveAndCloseViewModelAsync();
		}

		private Task CancelDialogAsync() => this.CancelAndCloseViewModelAsync();

		/// <inheritdoc />
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			var name = Name.Trim();
			if (string.IsNullOrWhiteSpace(name))
			{
				validationResults.Add(FieldValidationResult.CreateError(NameProperty, "State definition name is required."));
			}
			else if (IsDuplicate(name))
			{
				validationResults.Add(FieldValidationResult.CreateError(NameProperty, "State definition names must be unique."));
			}
		}

		private void UpdateWarning()
		{
			var name = Name.Trim();
			Warning = IsCaseOnlyDuplicate(name)
				? "State definition names differ only by casing. Check you don't have a typo."
				: string.Empty;
		}

		private bool IsDuplicate(string name)
		{
			return _existingNames.Any(existingName =>
				!existingName.Equals(_currentName, StringComparison.Ordinal) &&
				existingName.Equals(name, StringComparison.Ordinal));
		}

		private bool IsCaseOnlyDuplicate(string name)
		{
			return _existingNames.Any(existingName =>
				!existingName.Equals(_currentName, StringComparison.Ordinal) &&
				existingName.Equals(name, StringComparison.OrdinalIgnoreCase) &&
				!existingName.Equals(name, StringComparison.Ordinal));
		}
	}
}

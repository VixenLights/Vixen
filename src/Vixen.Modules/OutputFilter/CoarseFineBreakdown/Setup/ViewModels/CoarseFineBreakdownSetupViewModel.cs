#nullable enable

using Catel.Data;
using Catel.MVVM;

namespace VixenModules.OutputFilter.CoarseFineBreakdown.Setup.ViewModels
{
	/// <summary>
	/// Provides staged editing and validation for coarse/fine default-value mapping.
	/// </summary>
	internal sealed class CoarseFineBreakdownSetupViewModel : ViewModelBase
	{
		private readonly CoarseFineBreakdownSetupResult _originalConfiguration;
		private readonly Action<CoarseFineBreakdownSetupResult>? _applyLiveConfiguration;
		private TaskCommand? _okCommand;
		private TaskCommand? _cancelCommand;
		private CoarseFineBreakdownSetupResult? _lastAppliedLiveConfiguration;

		/// <summary>
		/// Initializes a new instance of the <see cref="CoarseFineBreakdownSetupViewModel"/> class.
		/// </summary>
		/// <param name="enableDefaultValueMapping"><see langword="true" /> to initially enable the resting output; otherwise, <see langword="false" />.</param>
		/// <param name="restingCoarseValue">The initial resting coarse value.</param>
		/// <param name="restingFineValue">The initial resting fine value.</param>
		/// <param name="applyLiveConfiguration">An optional callback that applies a valid configuration while live mode is enabled.</param>
		public CoarseFineBreakdownSetupViewModel(
			bool enableDefaultValueMapping,
			byte restingCoarseValue,
			byte restingFineValue,
			Action<CoarseFineBreakdownSetupResult>? applyLiveConfiguration = null)
		{
			DeferValidationUntilFirstSaveCall = false;
			_originalConfiguration = new CoarseFineBreakdownSetupResult(
				enableDefaultValueMapping,
				restingCoarseValue,
				restingFineValue);
			_applyLiveConfiguration = applyLiveConfiguration;
			EnableDefaultValueMapping = enableDefaultValueMapping;
			RestingCoarseValue = restingCoarseValue;
			RestingFineValue = restingFineValue;
			Validate(true);
		}

		/// <summary>
		/// Gets or sets a value that indicates whether missing output values emit the resting values.
		/// </summary>
		/// <value><see langword="true" /> to emit resting values; otherwise, <see langword="false" />.</value>
		public bool EnableDefaultValueMapping
		{
			get => GetValue<bool>(EnableDefaultValueMappingProperty);
			set
			{
				SetValue(EnableDefaultValueMappingProperty, value);
				ApplyLiveConfiguration();
			}
		}

		/// <summary>
		/// Identifies the <see cref="EnableDefaultValueMapping"/> property.
		/// </summary>
		public static readonly IPropertyData EnableDefaultValueMappingProperty =
			RegisterProperty(nameof(EnableDefaultValueMapping), false);

		/// <summary>
		/// Gets or sets the staged high-byte resting value.
		/// </summary>
		/// <value>A value from <c>0</c> through <c>255</c>.</value>
		public decimal RestingCoarseValue
		{
			get => GetValue<decimal>(RestingCoarseValueProperty);
			set
			{
				SetValue(RestingCoarseValueProperty, value);
				_okCommand?.RaiseCanExecuteChanged();
				ApplyLiveConfiguration();
			}
		}

		/// <summary>
		/// Identifies the <see cref="RestingCoarseValue"/> property.
		/// </summary>
		public static readonly IPropertyData RestingCoarseValueProperty =
			RegisterProperty(nameof(RestingCoarseValue), 0m);

		/// <summary>
		/// Gets or sets the staged low-byte resting value.
		/// </summary>
		/// <value>A value from <c>0</c> through <c>255</c>.</value>
		public decimal RestingFineValue
		{
			get => GetValue<decimal>(RestingFineValueProperty);
			set
			{
				SetValue(RestingFineValueProperty, value);
				_okCommand?.RaiseCanExecuteChanged();
				ApplyLiveConfiguration();
			}
		}

		/// <summary>
		/// Identifies the <see cref="RestingFineValue"/> property.
		/// </summary>
		public static readonly IPropertyData RestingFineValueProperty =
			RegisterProperty(nameof(RestingFineValue), 0m);

		/// <summary>
		/// Gets or sets a value that indicates whether valid edits immediately update the filter.
		/// </summary>
		/// <value><see langword="true" /> to immediately apply valid edits; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
		public bool IsLiveMode
		{
			get => GetValue<bool>(IsLiveModeProperty);
			set
			{
				SetValue(IsLiveModeProperty, value);
				ApplyLiveConfiguration();
			}
		}

		/// <summary>
		/// Identifies the <see cref="IsLiveMode"/> property.
		/// </summary>
		public static readonly IPropertyData IsLiveModeProperty =
			RegisterProperty(nameof(IsLiveMode), false);

		/// <summary>
		/// Gets the accepted configuration, if the dialog was accepted.
		/// </summary>
		/// <value>The accepted configuration; otherwise, <see langword="null" />.</value>
		public CoarseFineBreakdownSetupResult? Result { get; private set; }

		/// <summary>
		/// Gets a value that indicates whether the accepted result is already applied through live mode.
		/// </summary>
		/// <value><see langword="true" /> if the current accepted result is already applied; otherwise, <see langword="false" />.</value>
		internal bool IsResultAppliedLive => Result is { } result && _lastAppliedLiveConfiguration == result;

		/// <summary>
		/// Gets the command that accepts the staged values and closes the dialog.
		/// </summary>
		/// <value>The command that accepts valid staged values.</value>
		public TaskCommand OkCommand => _okCommand ??= new TaskCommand(OkAsync, CanOk);

		/// <summary>
		/// Gets the command that discards the staged values and closes the dialog.
		/// </summary>
		/// <value>The command that cancels the dialog.</value>
		public TaskCommand CancelCommand => _cancelCommand ??= new TaskCommand(CancelDialogAsync);

		/// <inheritdoc />
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			ValidateRange(RestingCoarseValue, RestingCoarseValueProperty, 0m, byte.MaxValue, "Resting coarse value must be between 0 and 255.", validationResults);
			ValidateRange(RestingFineValue, RestingFineValueProperty, 0m, byte.MaxValue, "Resting fine value must be between 0 and 255.", validationResults);
		}

		private bool CanOk() => !HasErrors;

		private Task OkAsync()
		{
			Validate(true);
			if (HasErrors)
			{
				return Task.CompletedTask;
			}

			Result = new CoarseFineBreakdownSetupResult(
				EnableDefaultValueMapping,
				(byte)RestingCoarseValue,
				(byte)RestingFineValue);
			return this.SaveAndCloseViewModelAsync();
		}

		private Task CancelDialogAsync()
		{
			RestoreOriginalConfiguration();
			return this.CancelAndCloseViewModelAsync();
		}

		internal void RestoreOriginalConfiguration()
		{
			if (_lastAppliedLiveConfiguration is not null)
			{
				_applyLiveConfiguration?.Invoke(_originalConfiguration);
				_lastAppliedLiveConfiguration = null;
			}
		}

		private void ApplyLiveConfiguration()
		{
			if (!IsLiveMode || !TryGetValidConfiguration(out var configuration))
			{
				return;
			}

			_applyLiveConfiguration?.Invoke(configuration);
			_lastAppliedLiveConfiguration = configuration;
		}

		private bool TryGetValidConfiguration(out CoarseFineBreakdownSetupResult configuration)
		{
			if (RestingCoarseValue is < 0m or > byte.MaxValue || RestingFineValue is < 0m or > byte.MaxValue)
			{
				configuration = default;
				return false;
			}

			configuration = new CoarseFineBreakdownSetupResult(
				EnableDefaultValueMapping,
				(byte)RestingCoarseValue,
				(byte)RestingFineValue);
			return true;
		}

		private static void ValidateRange(
			decimal value,
			IPropertyData property,
			decimal minimum,
			decimal maximum,
			string message,
			List<IFieldValidationResult> validationResults)
		{
			if (value < minimum || value > maximum)
			{
				validationResults.Add(FieldValidationResult.CreateError(property, message));
			}
		}
	}
}

using Catel.Data;
using Catel.MVVM;
using WPFCommon.Extensions;

namespace VixenModules.OutputFilter.CoarseFineBreakdown.Setup.ViewModels
{
	/// <summary>
	/// Provides staged editing and validation for coarse/fine default-value mapping.
	/// </summary>
	internal sealed class CoarseFineBreakdownSetupViewModel : ViewModelBase
	{
		private TaskCommand _okCommand;
		private TaskCommand _cancelCommand;

		/// <summary>
		/// Initializes a new instance of the <see cref="CoarseFineBreakdownSetupViewModel"/> class.
		/// </summary>
		/// <param name="enableDefaultValueMapping"><see langword="true" /> to initially enable default-value mapping; otherwise, <see langword="false" />.</param>
		/// <param name="defaultInputValue">The initial default input value.</param>
		/// <param name="restingCoarseValue">The initial resting coarse value.</param>
		/// <param name="restingFineValue">The initial resting fine value.</param>
		public CoarseFineBreakdownSetupViewModel(
			bool enableDefaultValueMapping,
			ushort defaultInputValue,
			byte restingCoarseValue,
			byte restingFineValue)
		{
			DeferValidationUntilFirstSaveCall = false;
			EnableDefaultValueMapping = enableDefaultValueMapping;
			DefaultInputValue = defaultInputValue;
			RestingCoarseValue = restingCoarseValue;
			RestingFineValue = restingFineValue;
			Validate(true);
		}

		/// <summary>
		/// Gets or sets a value that indicates whether the default input value is mapped to the resting values.
		/// </summary>
		/// <value><see langword="true" /> to enable mapping; otherwise, <see langword="false" />.</value>
		public bool EnableDefaultValueMapping
		{
			get => GetValue<bool>(EnableDefaultValueMappingProperty);
			set => SetValue(EnableDefaultValueMappingProperty, value);
		}

		/// <summary>
		/// Identifies the <see cref="EnableDefaultValueMapping"/> property.
		/// </summary>
		public static readonly IPropertyData EnableDefaultValueMappingProperty =
			RegisterProperty<bool>(nameof(EnableDefaultValueMapping), false);

		/// <summary>
		/// Gets or sets the staged 16-bit default input value.
		/// </summary>
		/// <value>A value from <c>0</c> through <c>65535</c>.</value>
		public decimal DefaultInputValue
		{
			get => GetValue<decimal>(DefaultInputValueProperty);
			set
			{
				SetValue(DefaultInputValueProperty, value);
				_okCommand?.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Identifies the <see cref="DefaultInputValue"/> property.
		/// </summary>
		public static readonly IPropertyData DefaultInputValueProperty =
			RegisterProperty<decimal>(nameof(DefaultInputValue), 0m);

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
			}
		}

		/// <summary>
		/// Identifies the <see cref="RestingCoarseValue"/> property.
		/// </summary>
		public static readonly IPropertyData RestingCoarseValueProperty =
			RegisterProperty<decimal>(nameof(RestingCoarseValue), 0m);

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
			}
		}

		/// <summary>
		/// Identifies the <see cref="RestingFineValue"/> property.
		/// </summary>
		public static readonly IPropertyData RestingFineValueProperty =
			RegisterProperty<decimal>(nameof(RestingFineValue), 0m);

		/// <summary>
		/// Gets the accepted configuration, if the dialog was accepted.
		/// </summary>
		/// <value>The accepted configuration; otherwise, <see langword="null" />.</value>
		public CoarseFineBreakdownSetupResult? Result { get; private set; }

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
			ValidateRange(DefaultInputValue, DefaultInputValueProperty, 0m, ushort.MaxValue, "Default input value must be between 0 and 65535.", validationResults);
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
				(ushort)DefaultInputValue,
				(byte)RestingCoarseValue,
				(byte)RestingFineValue);
			return this.SaveAndCloseViewModelAsync();
		}

		private Task CancelDialogAsync() => this.CancelAndCloseViewModelAsync();

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

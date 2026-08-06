using Catel.Data;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.BackgroundImageScaling;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	/// <summary>
	/// Provides the state and validation for the background image scaling dialog.
	/// </summary>
	internal sealed class BackgroundImageScaleViewModel : ViewModelBase
	{
		private TaskCommand _cancelCommand;
		private TaskCommand _okCommand;
		private bool _isUpdatingInputs;
		private bool _lastEditedWidth = true;
		private int _lockBaseHeight;
		private int _lockBaseWidth;
		private int _targetHeight;
		private int _targetWidth;

		/// <summary>
		/// Initializes a new instance of the <see cref="BackgroundImageScaleViewModel" /> class.
		/// </summary>
		/// <param name="sourceWidth">The source bitmap width in pixels.</param>
		/// <param name="sourceHeight">The source bitmap height in pixels.</param>
		/// <param name="currentWidth">The current logical canvas width in editor pixels.</param>
		/// <param name="currentHeight">The current logical canvas height in editor pixels.</param>
		/// <param name="hasLights"><see langword="true" /> when the prop has one or more unique lights; otherwise, <see langword="false" />.</param>
		/// <exception cref="ArgumentOutOfRangeException">A source or current dimension is outside the supported editor range.</exception>
		public BackgroundImageScaleViewModel(int sourceWidth, int sourceHeight, int currentWidth, int currentHeight, bool hasLights)
		{
			if (!BackgroundImageScaleCalculator.AreValidDimensions(sourceWidth, sourceHeight))
			{
				throw new ArgumentOutOfRangeException(nameof(sourceWidth));
			}

			if (!BackgroundImageScaleCalculator.AreValidDimensions(currentWidth, currentHeight))
			{
				throw new ArgumentOutOfRangeException(nameof(currentWidth));
			}

			SourceWidth = sourceWidth;
			SourceHeight = sourceHeight;
			CurrentWidth = currentWidth;
			CurrentHeight = currentHeight;
			HasLights = hasLights;
			Units = Enum.GetValues<BackgroundImageScaleUnit>();
			_targetWidth = currentWidth;
			_targetHeight = currentHeight;
			Unit = BackgroundImageScaleUnit.Pixels;
			IsAspectRatioLocked = true;
			ScaleExistingLightPositions = hasLights;
			DeferValidationUntilFirstSaveCall = false;
			UpdateInputsFromTarget();
			Validate(true);
		}

		/// <summary>
		/// Gets the source bitmap width in pixels.
		/// </summary>
		public int SourceWidth { get; }

		/// <summary>
		/// Gets the source bitmap height in pixels.
		/// </summary>
		public int SourceHeight { get; }

		/// <summary>
		/// Gets the current logical canvas width in editor pixels.
		/// </summary>
		public int CurrentWidth { get; }

		/// <summary>
		/// Gets the current logical canvas height in editor pixels.
		/// </summary>
		public int CurrentHeight { get; }

		/// <summary>
		/// Gets a value that indicates whether the prop contains a light whose position can be scaled.
		/// </summary>
		public bool HasLights { get; }

		/// <summary>
		/// Gets the unit values available for dimension entry.
		/// </summary>
		public IReadOnlyList<BackgroundImageScaleUnit> Units { get; }

		/// <summary>
		/// Gets or sets the unit used for the editable dimensions.
		/// </summary>
		public BackgroundImageScaleUnit Unit
		{
			get => GetValue<BackgroundImageScaleUnit>(UnitProperty);
			set
			{
				if (Unit == value)
				{
					return;
				}

				SetValue(UnitProperty, value);
				UpdateInputsFromTarget();
				ValidateAndUpdateCommand();
			}
		}

		/// <summary>
		/// Identifies the <see cref="Unit" /> property.
		/// </summary>
		public static readonly IPropertyData UnitProperty = RegisterProperty<BackgroundImageScaleUnit>(nameof(Unit));

		/// <summary>
		/// Gets or sets the entered width in the selected <see cref="Unit" />.
		/// </summary>
		public double WidthInput
		{
			get => GetValue<double>(WidthInputProperty);
			set
			{
				SetValue(WidthInputProperty, value);
				if (!_isUpdatingInputs)
				{
					_lastEditedWidth = true;
					UpdateTargetFromWidth();
				}

				ValidateAndUpdateCommand();
			}
		}

		/// <summary>
		/// Identifies the <see cref="WidthInput" /> property.
		/// </summary>
		public static readonly IPropertyData WidthInputProperty = RegisterProperty<double>(nameof(WidthInput));

		/// <summary>
		/// Gets or sets the entered height in the selected <see cref="Unit" />.
		/// </summary>
		public double HeightInput
		{
			get => GetValue<double>(HeightInputProperty);
			set
			{
				SetValue(HeightInputProperty, value);
				if (!_isUpdatingInputs)
				{
					_lastEditedWidth = false;
					UpdateTargetFromHeight();
				}

				ValidateAndUpdateCommand();
			}
		}

		/// <summary>
		/// Identifies the <see cref="HeightInput" /> property.
		/// </summary>
		public static readonly IPropertyData HeightInputProperty = RegisterProperty<double>(nameof(HeightInput));

		/// <summary>
		/// Gets or sets a value that indicates whether changing one dimension updates the other to preserve the current logical canvas aspect ratio.
		/// </summary>
		public bool IsAspectRatioLocked
		{
			get => GetValue<bool>(IsAspectRatioLockedProperty);
			set
			{
				var wasLocked = IsAspectRatioLocked;
				SetValue(IsAspectRatioLockedProperty, value);
				if (!wasLocked && value)
				{
					CaptureAspectLockRatio();
					UpdateLockedTargetFromLastEdit();
				}

				ValidateAndUpdateCommand();
			}
		}

		/// <summary>
		/// Identifies the <see cref="IsAspectRatioLocked" /> property.
		/// </summary>
		public static readonly IPropertyData IsAspectRatioLockedProperty = RegisterProperty<bool>(nameof(IsAspectRatioLocked));

		/// <summary>
		/// Gets or sets a value that indicates whether accepted scaling also moves existing light centers.
		/// </summary>
		public bool ScaleExistingLightPositions
		{
			get => GetValue<bool>(ScaleExistingLightPositionsProperty);
			set => SetValue(ScaleExistingLightPositionsProperty, HasLights && value);
		}

		/// <summary>
		/// Identifies the <see cref="ScaleExistingLightPositions" /> property.
		/// </summary>
		public static readonly IPropertyData ScaleExistingLightPositionsProperty = RegisterProperty<bool>(nameof(ScaleExistingLightPositions));

		/// <summary>
		/// Gets the validated target width in editor pixels.
		/// </summary>
		public int ResultWidth => _targetWidth;

		/// <summary>
		/// Gets the validated target height in editor pixels.
		/// </summary>
		public int ResultHeight => _targetHeight;

		/// <summary>
		/// Gets the display text for the original bitmap dimensions.
		/// </summary>
		public string OriginalDimensions => $"{SourceWidth} × {SourceHeight} px";

		/// <summary>
		/// Gets the display text for the current canvas dimensions.
		/// </summary>
		public string CurrentDimensions => $"{CurrentWidth} × {CurrentHeight} px";

		/// <summary>
		/// Gets the display text for the validated resulting canvas dimensions.
		/// </summary>
		public string ResultDimensions => $"{ResultWidth} × {ResultHeight} px";

		/// <summary>
		/// Gets the options selected when the dialog is accepted.
		/// </summary>
		public BackgroundImageScaleOptions Options { get; private set; }

		/// <summary>
		/// Gets the command that accepts valid scale options and closes the dialog.
		/// </summary>
		public TaskCommand OkCommand => _okCommand ??= new TaskCommand(OkAsync, CanOk);

		/// <summary>
		/// Gets the command that cancels the dialog without producing options.
		/// </summary>
		public TaskCommand CancelCommand => _cancelCommand ??= new TaskCommand(CancelDialogAsync);

		/// <inheritdoc />
		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			if (!TryGetValidatedTarget(out _, out _, out var widthError, out var heightError))
			{
				if (widthError != null)
				{
					validationResults.Add(FieldValidationResult.CreateError(WidthInputProperty, widthError));
				}

				if (heightError != null)
				{
					validationResults.Add(FieldValidationResult.CreateError(HeightInputProperty, heightError));
				}
			}
		}

		private bool CanOk() => !HasErrors;

		private Task OkAsync()
		{
			Validate(true);
			if (!TryGetValidatedTarget(out var width, out var height, out _, out _))
			{
				return Task.CompletedTask;
			}

			Options = new BackgroundImageScaleOptions(width, height, ScaleExistingLightPositions);
			return this.SaveAndCloseViewModelAsync();
		}

		private Task CancelDialogAsync() => this.CancelAndCloseViewModelAsync();

		private void UpdateTargetFromWidth()
		{
			if (!BackgroundImageScaleCalculator.TryConvertToPixels(WidthInput, Unit, SourceWidth, out var width))
			{
				return;
			}

			if (IsAspectRatioLocked)
			{
				if (!BackgroundImageScaleCalculator.TryCalculateLockedHeight(width, _lockBaseWidth, _lockBaseHeight, out var height))
				{
					return;
				}

				SetTarget(width, height, updateHeightInput: true);
				return;
			}

			_targetWidth = width;
			RaiseResultPropertiesChanged();
		}

		private void UpdateTargetFromHeight()
		{
			if (!BackgroundImageScaleCalculator.TryConvertToPixels(HeightInput, Unit, SourceHeight, out var height))
			{
				return;
			}

			if (IsAspectRatioLocked)
			{
				if (!BackgroundImageScaleCalculator.TryCalculateLockedWidth(height, _lockBaseWidth, _lockBaseHeight, out var width))
				{
					return;
				}

				SetTarget(width, height, updateWidthInput: true);
				return;
			}

			_targetHeight = height;
			RaiseResultPropertiesChanged();
		}

		private void UpdateLockedTargetFromLastEdit()
		{
			if (_lastEditedWidth)
			{
				UpdateTargetFromWidth();
			}
			else
			{
				UpdateTargetFromHeight();
			}
		}

		private void CaptureAspectLockRatio()
		{
			_lockBaseWidth = _targetWidth;
			_lockBaseHeight = _targetHeight;
		}

		private void SetTarget(int width, int height, bool updateWidthInput = false, bool updateHeightInput = false)
		{
			_targetWidth = width;
			_targetHeight = height;
			if (updateWidthInput || updateHeightInput)
			{
				_isUpdatingInputs = true;
				try
				{
					if (updateWidthInput)
					{
						WidthInput = ConvertPixelsToDisplayValue(_targetWidth, SourceWidth);
					}

					if (updateHeightInput)
					{
						HeightInput = ConvertPixelsToDisplayValue(_targetHeight, SourceHeight);
					}
				}
				finally
				{
					_isUpdatingInputs = false;
				}
			}

			RaiseResultPropertiesChanged();
		}

		private void UpdateInputsFromTarget()
		{
			_isUpdatingInputs = true;
			try
			{
				WidthInput = ConvertPixelsToDisplayValue(_targetWidth, SourceWidth);
				HeightInput = ConvertPixelsToDisplayValue(_targetHeight, SourceHeight);
			}
			finally
			{
				_isUpdatingInputs = false;
			}
		}

		private double ConvertPixelsToDisplayValue(int targetDimension, int sourceDimension) => Unit switch
		{
			BackgroundImageScaleUnit.Pixels => targetDimension,
			BackgroundImageScaleUnit.Percent => (double)targetDimension * 100d / sourceDimension,
			_ => throw new ArgumentOutOfRangeException(nameof(Unit))
		};

		private bool TryGetValidatedTarget(out int width, out int height, out string widthError, out string heightError)
		{
			width = default;
			height = default;
			widthError = null;
			heightError = null;
			var hasWidth = BackgroundImageScaleCalculator.TryConvertToPixels(WidthInput, Unit, SourceWidth, out width);
			var hasHeight = BackgroundImageScaleCalculator.TryConvertToPixels(HeightInput, Unit, SourceHeight, out height);

			if (!hasWidth)
			{
				widthError = "Enter a finite width that results in 1 through 100,000 pixels.";
			}

			if (!hasHeight)
			{
				heightError = "Enter a finite height that results in 1 through 100,000 pixels.";
			}

			if (!hasWidth || !hasHeight)
			{
				return false;
			}

			if (IsAspectRatioLocked)
			{
				if (_lastEditedWidth)
				{
					if (!BackgroundImageScaleCalculator.TryCalculateLockedHeight(width, _lockBaseWidth, _lockBaseHeight, out height))
					{
						heightError = "The aspect-ratio height must result in 1 through 100,000 pixels.";
						return false;
					}
				}
				else if (!BackgroundImageScaleCalculator.TryCalculateLockedWidth(height, _lockBaseWidth, _lockBaseHeight, out width))
				{
					widthError = "The aspect-ratio width must result in 1 through 100,000 pixels.";
					return false;
				}
			}

			return BackgroundImageScaleCalculator.AreValidDimensions(width, height);
		}

		private void RaiseResultPropertiesChanged()
		{
			RaisePropertyChanged(nameof(ResultWidth));
			RaisePropertyChanged(nameof(ResultHeight));
			RaisePropertyChanged(nameof(ResultDimensions));
		}

		private void ValidateAndUpdateCommand()
		{
			Validate(true);
			_okCommand?.RaiseCanExecuteChanged();
		}
	}
}

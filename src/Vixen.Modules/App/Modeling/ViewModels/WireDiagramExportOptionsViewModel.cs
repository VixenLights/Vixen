using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;

namespace VixenModules.App.Modeling.ViewModels;

/// <summary>
/// Provides validated wire diagram export settings and captures accepted options.
/// </summary>
public sealed class WireDiagramExportOptionsViewModel : ViewModelBase
{
	private TaskCommand _cancelCommand;
	private TaskCommand _okCommand;

	/// <summary>
	/// Initializes a new instance of the <see cref="WireDiagramExportOptionsViewModel" /> class.
	/// </summary>
	public WireDiagramExportOptionsViewModel()
	{
		DeferValidationUntilFirstSaveCall = false;
		Validate(true);
	}

	/// <summary>
	/// Gets or sets a value that indicates whether every node number is displayed.
	/// </summary>
	/// <value><see langword="true" /> to display every number; otherwise, <see langword="false" />. The default is <see langword="true" />.</value>
	public bool AllNumbers
	{
		get => GetValue<bool>(AllNumbersProperty);
		set
		{
			SetValue(AllNumbersProperty, value);
			ValidateAndUpdateCommand();
		}
	}

	/// <summary>
	/// Identifies the <see cref="AllNumbers" /> property.
	/// </summary>
	public static readonly IPropertyData AllNumbersProperty = RegisterProperty<bool>(nameof(AllNumbers), true);

	/// <summary>
	/// Gets or sets the interval between displayed node numbers when <see cref="AllNumbers" /> is <see langword="false" />.
	/// </summary>
	/// <value>A value from 1 through 1000. The default is 50 and is ignored when all numbers are shown.</value>
	public int LabelInterval
	{
		get => GetValue<int>(LabelIntervalProperty);
		set
		{
			SetValue(LabelIntervalProperty, value);
			ValidateAndUpdateCommand();
		}
	}

	/// <summary>
	/// Identifies the <see cref="LabelInterval" /> property.
	/// </summary>
	public static readonly IPropertyData LabelIntervalProperty = RegisterProperty<int>(nameof(LabelInterval), 50);

	/// <summary>
	/// Gets or sets the node marker radius in SVG drawing units.
	/// </summary>
	/// <value>A value from 1 through 30. The default is 6.</value>
	public int NodeRadius
	{
		get => GetValue<int>(NodeRadiusProperty);
		set
		{
			SetValue(NodeRadiusProperty, value);
			ValidateAndUpdateCommand();
		}
	}

	/// <summary>
	/// Identifies the <see cref="NodeRadius" /> property.
	/// </summary>
	public static readonly IPropertyData NodeRadiusProperty = RegisterProperty<int>(nameof(NodeRadius), 6);

	/// <summary>
	/// Gets or sets the whole-image enlargement percentage.
	/// </summary>
	/// <value>A value from 100 through 400 percent. The default is 100 percent.</value>
	public int ExportScalePercent
	{
		get => GetValue<int>(ExportScalePercentProperty);
		set
		{
			SetValue(ExportScalePercentProperty, value);
			ValidateAndUpdateCommand();
		}
	}

	/// <summary>
	/// Identifies the <see cref="ExportScalePercent" /> property.
	/// </summary>
	public static readonly IPropertyData ExportScalePercentProperty = RegisterProperty<int>(nameof(ExportScalePercent), 100);

	/// <summary>
	/// Gets the options produced by a successful acceptance of the dialog.
	/// </summary>
	/// <value>The accepted options, or <see langword="null" /> until the view model is saved.</value>
	public WireDiagramExportOptions Options { get; private set; }

	/// <summary>
	/// Gets the command that accepts valid options and closes the dialog.
	/// </summary>
	public TaskCommand OkCommand => _okCommand ??= new TaskCommand(AcceptAsync, CanAccept);

	/// <summary>
	/// Gets the command that cancels the dialog without producing options.
	/// </summary>
	public TaskCommand CancelCommand => _cancelCommand ??= new TaskCommand(CancelDialogAsync);

	/// <inheritdoc />
	protected override void ValidateFields(List<IFieldValidationResult> validationResults)
	{
		if (!AllNumbers && LabelInterval is < 1 or > 1000)
		{
			validationResults.Add(FieldValidationResult.CreateError(LabelIntervalProperty, "Enter a label interval from 1 through 1000."));
		}

		if (NodeRadius is < 1 or > 30)
		{
			validationResults.Add(FieldValidationResult.CreateError(NodeRadiusProperty, "Enter a node radius from 1 through 30."));
		}

		if (ExportScalePercent is < 100 or > 400)
		{
			validationResults.Add(FieldValidationResult.CreateError(ExportScalePercentProperty, "Enter an enlargement percentage from 100 through 400."));
		}
	}

	/// <inheritdoc />
	protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
	{
		try
		{
			CreateOptions().Validate();
		}
		catch (ArgumentOutOfRangeException)
		{
			validationResults.Add(BusinessRuleValidationResult.CreateError("The wire diagram export settings are outside their supported ranges."));
		}
	}

	private bool CanAccept() => !HasErrors;

	private async Task AcceptAsync()
	{
		Validate(true);
		if (!CanAccept())
		{
			return;
		}

		Options = CreateOptions();
		await this.SaveAndCloseViewModelAsync();
	}

	private Task CancelDialogAsync()
	{
		Options = null;
		return this.CancelAndCloseViewModelAsync();
	}

	private WireDiagramExportOptions CreateOptions() => new()
	{
		LabelInterval = AllNumbers ? 1 : LabelInterval,
		NodeRadius = NodeRadius,
		ExportScale = ExportScalePercent / 100f
	};

	private void ValidateAndUpdateCommand()
	{
		Validate(true);
		_okCommand?.RaiseCanExecuteChanged();
	}
}

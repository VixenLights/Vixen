using System.Windows.Input;
using VixenModules.App.Modeling.ViewModels;
using Xunit;

namespace Vixen.Tests.App.Modeling;

public sealed class WireDiagramExportOptionsViewModelTests
{
	[Fact]
	public void Constructor_InitializesDesignDefaults()
	{
		var viewModel = new WireDiagramExportOptionsViewModel();

		Assert.True(viewModel.AllNumbers);
		Assert.Equal(50, viewModel.LabelInterval);
		Assert.Equal("50", viewModel.LabelIntervalText);
		Assert.Equal(6, viewModel.NodeRadius);
		Assert.Equal(100, viewModel.ExportScalePercent);
		Assert.Null(viewModel.Options);
		Assert.True(viewModel.OkCommand.CanExecute());
	}

	[Fact]
	public void AllNumbersMode_IgnoresDisabledIntervalWhenAccepting()
	{
		var viewModel = new WireDiagramExportOptionsViewModel
		{
			AllNumbers = true,
			LabelInterval = 0
		};

		((ICommand)viewModel.OkCommand).Execute(null);

		Assert.Equal(1, viewModel.Options.LabelInterval);
	}

	[Fact]
	public void IntervalMode_MapsSelectedIntervalWhenAccepting()
	{
		var viewModel = new WireDiagramExportOptionsViewModel
		{
			AllNumbers = false,
			LabelInterval = 50
		};

		((ICommand)viewModel.OkCommand).Execute(null);

		Assert.Equal(50, viewModel.Options.LabelInterval);
	}

	[Theory]
	[InlineData("50", 50)]
	[InlineData("100", 100)]
	public void IntervalMode_TextInputParsesCommonValues(string input, int expectedInterval)
	{
		var viewModel = new WireDiagramExportOptionsViewModel { AllNumbers = false, LabelIntervalText = input };

		((ICommand)viewModel.OkCommand).Execute(null);

		Assert.Equal(expectedInterval, viewModel.LabelInterval);
		Assert.Equal(expectedInterval, viewModel.Options.LabelInterval);
	}

	[Fact]
	public void Accepting_MapsTwoHundredPercentToScaleFactorTwo()
	{
		var viewModel = new WireDiagramExportOptionsViewModel { ExportScalePercent = 200 };

		((ICommand)viewModel.OkCommand).Execute(null);

		Assert.Equal(2f, viewModel.Options.ExportScale);
	}

	[Theory]
	[InlineData(0)]
	[InlineData(1001)]
	public void IntervalMode_InvalidIntervalPreventsAcceptance(int interval)
	{
		var viewModel = new WireDiagramExportOptionsViewModel
		{
			AllNumbers = false,
			LabelInterval = interval
		};

		Assert.False(viewModel.OkCommand.CanExecute());
		Assert.Null(viewModel.Options);
	}

	[Theory]
	[InlineData("")]
	[InlineData("abc")]
	[InlineData("0")]
	[InlineData("1001")]
	[InlineData("50.5")]
	public void IntervalMode_InvalidTextPreventsAcceptance(string input)
	{
		var viewModel = new WireDiagramExportOptionsViewModel { AllNumbers = false, LabelIntervalText = input };

		Assert.False(viewModel.OkCommand.CanExecute());
		Assert.Null(viewModel.Options);
	}

	[Fact]
	public void AllNumbersMode_InvalidDisabledIntervalDoesNotPreventAcceptance()
	{
		var viewModel = new WireDiagramExportOptionsViewModel
		{
			AllNumbers = true,
			LabelInterval = 0
		};

		Assert.True(viewModel.OkCommand.CanExecute());
	}

	[Theory]
	[InlineData(0)]
	[InlineData(31)]
	public void InvalidNodeRadiusPreventsAcceptance(int radius)
	{
		var viewModel = new WireDiagramExportOptionsViewModel { NodeRadius = radius };

		Assert.False(viewModel.OkCommand.CanExecute());
	}

	[Theory]
	[InlineData(99)]
	[InlineData(401)]
	public void InvalidExportScalePercentPreventsAcceptance(int percent)
	{
		var viewModel = new WireDiagramExportOptionsViewModel { ExportScalePercent = percent };

		Assert.False(viewModel.OkCommand.CanExecute());
	}

	[Fact]
	public void Cancel_DoesNotProduceOptions()
	{
		var viewModel = new WireDiagramExportOptionsViewModel();

		((ICommand)viewModel.CancelCommand).Execute(null);

		Assert.Null(viewModel.Options);
	}
}

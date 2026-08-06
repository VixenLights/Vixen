using VixenModules.App.CustomPropEditor.BackgroundImageScaling;
using VixenModules.App.CustomPropEditor.ViewModels;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.BackgroundImageScaling;

[Collection("CustomPropEditor")]
public sealed class BackgroundImageScaleViewModelTests
{
	[Fact]
	public void Constructor_InitializesDefaultsAndCurrentDimensions()
	{
		var viewModel = new BackgroundImageScaleViewModel(4032, 3024, 1008, 756, hasLights: true);

		Assert.Equal(BackgroundImageScaleUnit.Pixels, viewModel.Unit);
		Assert.True(viewModel.IsAspectRatioLocked);
		Assert.True(viewModel.ScaleExistingLightPositions);
		Assert.Equal(1008, viewModel.ResultWidth);
		Assert.Equal(756, viewModel.ResultHeight);
		Assert.False(viewModel.HasErrors);
	}

	[Fact]
	public void UnitSwitch_DerivesDisplayValuesFromCanonicalPixels()
	{
		var viewModel = new BackgroundImageScaleViewModel(4032, 3024, 1008, 756, hasLights: true);

		viewModel.Unit = BackgroundImageScaleUnit.Percent;
		var percentWidth = viewModel.WidthInput;
		var percentHeight = viewModel.HeightInput;
		viewModel.Unit = BackgroundImageScaleUnit.Pixels;
		viewModel.Unit = BackgroundImageScaleUnit.Percent;

		Assert.Equal(25, percentWidth);
		Assert.Equal(25, percentHeight);
		Assert.Equal(percentWidth, viewModel.WidthInput);
		Assert.Equal(percentHeight, viewModel.HeightInput);
		Assert.Equal(1008, viewModel.ResultWidth);
		Assert.Equal(756, viewModel.ResultHeight);
	}

	[Fact]
	public void WidthInput_WithAspectLock_UpdatesMatchingHeight()
	{
		var viewModel = new BackgroundImageScaleViewModel(4, 3, 800, 600, hasLights: true);

		viewModel.WidthInput = 640;

		Assert.Equal(640, viewModel.ResultWidth);
		Assert.Equal(480, viewModel.ResultHeight);
		Assert.Equal(480, viewModel.HeightInput);
	}

	[Fact]
	public void UnlockedDimensions_PreserveIndependentInputs()
	{
		var viewModel = new BackgroundImageScaleViewModel(4, 3, 800, 600, hasLights: true)
		{
			IsAspectRatioLocked = false,
			WidthInput = 640,
			HeightInput = 400
		};

		viewModel.OkCommand.Execute(null);

		Assert.Equal(new BackgroundImageScaleOptions(640, 400, true), viewModel.Options);
	}

	[Fact]
	public void EnablingAspectLock_UsesLastEditedHeight()
	{
		var viewModel = new BackgroundImageScaleViewModel(4, 3, 800, 600, hasLights: true)
		{
			IsAspectRatioLocked = false,
			WidthInput = 640,
			HeightInput = 400
		};

		viewModel.IsAspectRatioLocked = true;

		Assert.Equal(533, viewModel.ResultWidth);
		Assert.Equal(400, viewModel.ResultHeight);
	}

	[Theory]
	[InlineData(double.NaN)]
	[InlineData(double.PositiveInfinity)]
	[InlineData(0)]
	[InlineData(-1)]
	[InlineData(100001)]
	public void InvalidWidth_DisablesOk(double width)
	{
		var viewModel = new BackgroundImageScaleViewModel(4, 3, 800, 600, hasLights: true)
		{
			WidthInput = width
		};

		Assert.True(viewModel.HasErrors);
		Assert.False(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public void PropWithoutLights_DisablesCoordinateScaling()
	{
		var viewModel = new BackgroundImageScaleViewModel(4, 3, 800, 600, hasLights: false)
		{
			ScaleExistingLightPositions = true
		};

		viewModel.OkCommand.Execute(null);

		Assert.False(viewModel.ScaleExistingLightPositions);
		Assert.Equal(new BackgroundImageScaleOptions(800, 600, false), viewModel.Options);
	}

	[Fact]
	public void Cancel_DoesNotProduceOptions()
	{
		var viewModel = new BackgroundImageScaleViewModel(4, 3, 800, 600, hasLights: true);

		viewModel.CancelCommand.Execute(null);

		Assert.Null(viewModel.Options);
	}
}

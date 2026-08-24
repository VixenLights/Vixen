using System.Reflection;
using VixenModules.OutputFilter.CoarseFineBreakdown.Setup.ViewModels;
using Xunit;

namespace Vixen.Tests.OutputFilter.CoarseFineBreakdown;

public sealed class CoarseFineBreakdownSetupViewModelTests
{
	[Fact]
	public void Constructor_StagesInitialValuesAndAcceptsInclusiveBoundaries()
	{
		var viewModel = new CoarseFineBreakdownSetupViewModel(true, byte.MaxValue, byte.MaxValue);

		Assert.True(viewModel.EnableDefaultValueMapping);
		Assert.Equal((decimal)byte.MaxValue, viewModel.RestingCoarseValue);
		Assert.Equal((decimal)byte.MaxValue, viewModel.RestingFineValue);
		Assert.False(viewModel.HasErrors);
		Assert.True(viewModel.OkCommand.CanExecute(null));
	}

	[Theory]
	[InlineData(-1, 0)]
	[InlineData(256, 0)]
	[InlineData(0, -1)]
	[InlineData(0, 256)]
	public void InvalidDraftValues_BlockOk(decimal restingCoarseValue, decimal restingFineValue)
	{
		var viewModel = new CoarseFineBreakdownSetupViewModel(false, 0, 0)
		{
			RestingCoarseValue = restingCoarseValue,
			RestingFineValue = restingFineValue
		};

		Assert.True(viewModel.HasErrors);
		Assert.False(viewModel.OkCommand.CanExecute(null));
	}

	[Fact]
	public async Task ValidOk_ProducesOnlyTheStagedResult()
	{
		var viewModel = new CoarseFineBreakdownSetupViewModel(false, 0, 0)
		{
			EnableDefaultValueMapping = true,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		await InvokeAsync(viewModel, "OkAsync");

		var result = Assert.IsType<CoarseFineBreakdownSetupResult>(viewModel.Result);
		Assert.True(result.EnableDefaultValueMapping);
		Assert.Equal((byte)0xA5, result.RestingCoarseValue);
		Assert.Equal((byte)0x3C, result.RestingFineValue);
	}

	[Fact]
	public async Task Cancel_DiscardsDraftWithoutProducingAResult()
	{
		const bool originalEnabled = false;
		const byte originalCoarse = 2;
		const byte originalFine = 3;
		var viewModel = new CoarseFineBreakdownSetupViewModel(
			originalEnabled,
			originalCoarse,
			originalFine)
		{
			EnableDefaultValueMapping = true,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		await InvokeAsync(viewModel, "CancelDialogAsync");

		Assert.False(originalEnabled);
		Assert.Equal((byte)2, originalCoarse);
		Assert.Equal((byte)3, originalFine);
		Assert.Null(viewModel.Result);
	}

	private static Task InvokeAsync(CoarseFineBreakdownSetupViewModel viewModel, string methodName)
	{
		var method = typeof(CoarseFineBreakdownSetupViewModel).GetMethod(
			methodName,
			BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);
		return Assert.IsAssignableFrom<Task>(method.Invoke(viewModel, null));
	}
}

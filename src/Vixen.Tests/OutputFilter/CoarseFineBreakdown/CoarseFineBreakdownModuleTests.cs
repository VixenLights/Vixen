using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Sys;
using VixenModules.OutputFilter.CoarseFineBreakdown;
using Xunit;

namespace Vixen.Tests.OutputFilter.CoarseFineBreakdown;

public sealed class CoarseFineBreakdownModuleTests
{
	[Fact]
	public void DisabledMapping_PreservesLegacyCommandSplitting()
	{
		var data = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = false,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		Assert.Equal((byte)0x12, ProcessCommand(data, 0x1234).Coarse);
		Assert.Equal((byte)0x34, ProcessCommand(data, 0x1234).Fine);
		Assert.Equal((byte)0, ProcessCommand(data, 0).Coarse);
		Assert.Equal((byte)0, ProcessCommand(data, 0).Fine);
	}

	[Fact]
	public void DisabledMapping_LeavesMissingValuesWithoutAnOutput()
	{
		var module = CreateModule(new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = false,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		});

		module.Handle(new CommandDataFlowData(null!));

		Assert.Empty(Assert.IsType<CommandsDataFlowData>(module.Outputs[0].Data).Value);
		Assert.Empty(Assert.IsType<CommandsDataFlowData>(module.Outputs[1].Data).Value);
	}

	[Fact]
	public void EnabledMapping_UsesRestingBytesWhenCommandValueIsMissing()
	{
		var data = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		Assert.Equal(((byte)0xA5, (byte)0x3C), ProcessMissingCommand(data));
		Assert.Equal(((byte)0x00, (byte)0x00), ProcessCommand(data, 0));
		Assert.Equal(((byte)0x12, (byte)0x34), ProcessCommand(data, 0x1234));
	}

	[Fact]
	public void EnabledMapping_InitializesOutputsWithRestingBytes()
	{
		var module = CreateModule(new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		});

		Assert.Equal(((byte)0xA5, (byte)0x3C), GetOutputBytes(module));
	}

	[Fact]
	public void EnabledMapping_UsesRestingBytesWhenNoIntentProducesAnOutput()
	{
		var data = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		Assert.Equal(((byte)0xA5, (byte)0x3C), ProcessMissingIntents(data));
		Assert.Equal(((byte)0x7F, (byte)0xFF), ProcessRangeValue(data, 0.5));
	}

	[Fact]
	public void RangeValue_UsesExistingTruncationWhenAnInputIsPresent()
	{
		Assert.Equal(((byte)0x7F, (byte)0xFF), ProcessRangeValue(new CoarseFineBreakdownData(), 0.5));
	}

	[Theory]
	[InlineData((ushort)0, (byte)0, (byte)0)]
	[InlineData(ushort.MaxValue, byte.MaxValue, byte.MaxValue)]
	public void EnabledMapping_HandlesInputAndRestingBoundaries(
		ushort inputValue,
		byte restingCoarseValue,
		byte restingFineValue)
	{
		var data = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			RestingCoarseValue = restingCoarseValue,
			RestingFineValue = restingFineValue
		};

		Assert.Equal((restingCoarseValue, restingFineValue), ProcessMissingCommand(data));
		Assert.Equal(((byte)(inputValue >> 8), (byte)(inputValue & 0xFF)), ProcessCommand(data, inputValue));
	}

	[Fact]
	public void ProgrammaticSetters_RebuildOutputsWithUpdatedConfiguration()
	{
		var module = CreateModule(new CoarseFineBreakdownData());
		var originalOutputs = module.Outputs;

		module.RestingCoarseValue = 0xA5;
		module.RestingFineValue = 0x3C;
		module.EnableDefaultValueMapping = true;

		Assert.Equal((byte)0xA5, module.RestingCoarseValue);
		Assert.Equal((byte)0x3C, module.RestingFineValue);
		Assert.True(module.EnableDefaultValueMapping);
		Assert.NotSame(originalOutputs, module.Outputs);

		Assert.Equal(((byte)0xA5, (byte)0x3C), GetOutputBytes(module));

		var enabledOutputs = module.Outputs;
		module.RestingFineValue = 0x5E;

		Assert.NotSame(enabledOutputs, module.Outputs);
		Assert.Equal(((byte)0xA5, (byte)0x5E), GetOutputBytes(module));
	}

	[Fact]
	public void Clone_PreservesIndependentDefaultMappingConfiguration()
	{
		var original = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		var clone = Assert.IsType<CoarseFineBreakdownData>(original.Clone());
		original.EnableDefaultValueMapping = false;
		original.RestingCoarseValue = 0;
		original.RestingFineValue = 0;

		Assert.True(clone.EnableDefaultValueMapping);
		Assert.Equal((byte)0xA5, clone.RestingCoarseValue);
		Assert.Equal((byte)0x3C, clone.RestingFineValue);
	}

	private static (byte Coarse, byte Fine) ProcessCommand(CoarseFineBreakdownData data, ushort value)
	{
		var module = CreateModule(data);
		module.Handle(new CommandDataFlowData(new _16BitCommand(value)));
		return GetOutputBytes(module);
	}

	private static (byte Coarse, byte Fine) ProcessMissingCommand(CoarseFineBreakdownData data)
	{
		var module = CreateModule(data);
		module.Handle(new CommandDataFlowData(null!));
		return GetOutputBytes(module);
	}

	private static (byte Coarse, byte Fine) ProcessMissingIntents(CoarseFineBreakdownData data)
	{
		var module = CreateModule(data);
		module.Handle(new IntentsDataFlowData(null!));
		return GetOutputBytes(module);
	}

	private static (byte Coarse, byte Fine) ProcessRangeValue(CoarseFineBreakdownData data, double value)
	{
		var module = CreateModule(data);
		var state = new StaticIntentState<RangeValue<FunctionIdentity>>(
			new RangeValue<FunctionIdentity>(FunctionIdentity.Pan, "Pan", value, string.Empty));
		module.Handle(new IntentsDataFlowData([state]));
		return GetOutputBytes(module);
	}

	private static CoarseFineBreakdownModule CreateModule(CoarseFineBreakdownData data)
	{
		return new CoarseFineBreakdownModule
		{
			ModuleData = data
		};
	}

	private static (byte Coarse, byte Fine) GetOutputBytes(CoarseFineBreakdownModule module)
	{
		var coarseCommands = Assert.IsType<CommandsDataFlowData>(module.Outputs[0].Data).Value;
		var fineCommands = Assert.IsType<CommandsDataFlowData>(module.Outputs[1].Data).Value;
		var coarse = Assert.IsType<_8BitCommand>(Assert.Single(coarseCommands)).CommandValue;
		var fine = Assert.IsType<_8BitCommand>(Assert.Single(fineCommands)).CommandValue;
		return (coarse, fine);
	}
}

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
			DefaultInputValue = 0,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		Assert.Equal((byte)0x12, ProcessCommand(data, 0x1234).Coarse);
		Assert.Equal((byte)0x34, ProcessCommand(data, 0x1234).Fine);
		Assert.Equal((byte)0, ProcessCommand(data, 0).Coarse);
		Assert.Equal((byte)0, ProcessCommand(data, 0).Fine);
	}

	[Fact]
	public void EnabledMapping_ChangesOnlyAnExactCommandMatch()
	{
		var data = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			DefaultInputValue = 0x1234,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		Assert.Equal(((byte)0xA5, (byte)0x3C), ProcessCommand(data, 0x1234));
		Assert.Equal(((byte)0x12, (byte)0x33), ProcessCommand(data, 0x1233));
		Assert.Equal(((byte)0x12, (byte)0x35), ProcessCommand(data, 0x1235));
	}

	[Fact]
	public void RangeValue_UsesTruncationBeforeExactMapping()
	{
		const ushort truncatedHalf = 0x7FFF;
		var data = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			DefaultInputValue = truncatedHalf,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		Assert.Equal(((byte)0xA5, (byte)0x3C), ProcessRangeValue(data, 0.5));

		data.DefaultInputValue = 0x8000;
		Assert.Equal(((byte)0x7F, (byte)0xFF), ProcessRangeValue(data, 0.5));
	}

	[Theory]
	[InlineData((ushort)0, (byte)0, (byte)0)]
	[InlineData(ushort.MaxValue, byte.MaxValue, byte.MaxValue)]
	public void EnabledMapping_HandlesInputAndRestingBoundaries(
		ushort defaultInputValue,
		byte restingCoarseValue,
		byte restingFineValue)
	{
		var data = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			DefaultInputValue = defaultInputValue,
			RestingCoarseValue = restingCoarseValue,
			RestingFineValue = restingFineValue
		};

		Assert.Equal((restingCoarseValue, restingFineValue), ProcessCommand(data, defaultInputValue));
	}

	[Fact]
	public void ProgrammaticSetters_RebuildOutputsWithUpdatedConfiguration()
	{
		var module = CreateModule(new CoarseFineBreakdownData());
		var originalOutputs = module.Outputs;

		module.DefaultInputValue = 0x1234;
		module.RestingCoarseValue = 0xA5;
		module.RestingFineValue = 0x3C;
		module.EnableDefaultValueMapping = true;

		Assert.Equal((ushort)0x1234, module.DefaultInputValue);
		Assert.Equal((byte)0xA5, module.RestingCoarseValue);
		Assert.Equal((byte)0x3C, module.RestingFineValue);
		Assert.True(module.EnableDefaultValueMapping);
		Assert.NotSame(originalOutputs, module.Outputs);

		module.Handle(new CommandDataFlowData(new _16BitCommand(0x1234)));
		Assert.Equal(((byte)0xA5, (byte)0x3C), GetOutputBytes(module));
	}

	[Fact]
	public void Clone_PreservesIndependentDefaultMappingConfiguration()
	{
		var original = new CoarseFineBreakdownData
		{
			EnableDefaultValueMapping = true,
			DefaultInputValue = 0x1234,
			RestingCoarseValue = 0xA5,
			RestingFineValue = 0x3C
		};

		var clone = Assert.IsType<CoarseFineBreakdownData>(original.Clone());
		original.EnableDefaultValueMapping = false;
		original.DefaultInputValue = 0;
		original.RestingCoarseValue = 0;
		original.RestingFineValue = 0;

		Assert.True(clone.EnableDefaultValueMapping);
		Assert.Equal((ushort)0x1234, clone.DefaultInputValue);
		Assert.Equal((byte)0xA5, clone.RestingCoarseValue);
		Assert.Equal((byte)0x3C, clone.RestingFineValue);
	}

	private static (byte Coarse, byte Fine) ProcessCommand(CoarseFineBreakdownData data, ushort value)
	{
		var module = CreateModule(data);
		module.Handle(new CommandDataFlowData(new _16BitCommand(value)));
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

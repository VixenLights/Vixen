using System.Reflection;
using Moq;
using Vixen.Data.Flow;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Output;
using Xunit;

namespace Vixen.Tests.Core;

/// <summary>
/// Verifies output indexes and adapter lookups stay synchronized through controller mutations.
/// </summary>
[Collection(OutputControllerOutputIndexTestCollection.Name)]
public sealed class OutputControllerOutputIndexTests : IDisposable
{
	private readonly OutputControllerManager _previousOutputControllers;
	private readonly DataFlowManager _previousDataFlow;
	private readonly OutputControllerManager _outputControllers;
	private readonly DataFlowManager _dataFlow;

	public OutputControllerOutputIndexTests()
	{
		_previousOutputControllers = VixenSystem.OutputControllers;
		_previousDataFlow = VixenSystem.DataFlow;
		_outputControllers = new OutputControllerManager(
			new OutputDeviceCollection<OutputController>(),
			new OutputDeviceExecution<OutputController>());
		_dataFlow = new DataFlowManager();
		SetVixenSystemProperty(nameof(VixenSystem.OutputControllers), _outputControllers);
		SetVixenSystemProperty(nameof(VixenSystem.DataFlow), _dataFlow);
	}

	/// <summary>
	/// Verifies removal immediately refreshes surviving output lookup indexes before notifying observers.
	/// </summary>
	[Fact]
	public void RemoveOutputs_UpdatesSurvivorIndexesBeforeOutputCountChanged()
	{
		var controller = CreateController();
		var outputs = AddOutputs(controller, 10);
		var adapters = outputs.Select(controller.GetDataFlowComponentForOutput).ToArray();
		var patchSource = CreatePatchSource();
		_dataFlow.AddComponent(patchSource);
		_dataFlow.SetComponentSource(adapters[8], patchSource, 0);
		_dataFlow.SetComponentSource(adapters[9], patchSource, 1);
		var output8Source = adapters[8].Source;
		var output9Source = adapters[9].Source;
		var eventIndexes = new List<int>();
		controller.OutputCountChanged += (_, _) =>
		{
			AssertLookup(controller, outputs[8], adapters[8], 6);
			AssertLookup(controller, outputs[9], adapters[9], 7);
			eventIndexes.Add(outputs[8].Index);
		};

		controller.RemoveOutputs([outputs[2], outputs[3]]);

		Assert.Single(eventIndexes);
		Assert.Equal(6, outputs[8].Index);
		Assert.Equal(7, outputs[9].Index);
		AssertLookup(controller, outputs[8], adapters[8], 6);
		AssertLookup(controller, outputs[9], adapters[9], 7);
		Assert.Same(output8Source, adapters[8].Source);
		Assert.Same(output9Source, adapters[9].Source);
		Assert.False(_outputControllers.getOutputDetailsForDataFlowComponent(adapters[2], out _, out _));
		Assert.False(_outputControllers.getOutputDetailsForDataFlowComponent(adapters[3], out _, out _));
		Assert.DoesNotContain(adapters[2], _dataFlow.GetAllComponents());
		Assert.DoesNotContain(adapters[3], _dataFlow.GetAllComponents());

		controller.ReIndexOutputs();
		controller.ReIndexOutputs();
		AssertLookup(controller, outputs[8], adapters[8], 6);
	}

	/// <summary>
	/// Verifies removals at controller boundaries keep every surviving lookup aligned with its output position.
	/// </summary>
	[Theory]
	[InlineData(0, 1)]
	[InlineData(8, 9)]
	[InlineData(0, 9)]
	public void RemoveOutputs_UpdatesAllSurvivorIndexes(int firstRemovedIndex, int secondRemovedIndex)
	{
		var controller = CreateController();
		var outputs = AddOutputs(controller, 10);
		var adapters = outputs.Select(controller.GetDataFlowComponentForOutput).ToArray();

		controller.RemoveOutputs([outputs[firstRemovedIndex], outputs[secondRemovedIndex]]);

		var expectedIndex = 0;
		foreach (var originalIndex in Enumerable.Range(0, outputs.Length).Except([firstRemovedIndex, secondRemovedIndex]))
		{
			AssertLookup(controller, outputs[originalIndex], adapters[originalIndex], expectedIndex++);
		}
	}

	/// <summary>
	/// Verifies removing all outputs clears registrations and leaves reindexing safe to repeat.
	/// </summary>
	[Fact]
	public void RemoveOutputs_AllOutputsRemoved_ClearsLookup()
	{
		var controller = CreateController();
		var outputs = AddOutputs(controller, 4);
		var adapters = outputs.Select(controller.GetDataFlowComponentForOutput).ToArray();

		controller.RemoveOutputs(outputs);
		controller.RemoveOutputs(outputs);
		controller.ReIndexOutputs();

		Assert.Empty(controller.Outputs);
		foreach (var adapter in adapters)
		{
			Assert.False(_outputControllers.getOutputDetailsForDataFlowComponent(adapter, out _, out _));
			Assert.DoesNotContain(adapter, _dataFlow.GetAllComponents());
		}
	}

	/// <summary>
	/// Verifies a removal that crosses the direct-output paging threshold refreshes the final survivor's lookup.
	/// </summary>
	[Fact]
	public void RemoveOutputs_CrossesPagedOutputBoundary_UpdatesFinalSurvivorLookup()
	{
		var controller = CreateController();
		var outputs = AddOutputs(controller, 5001);
		var finalOutput = outputs[^1];
		var finalAdapter = controller.GetDataFlowComponentForOutput(finalOutput);

		controller.RemoveOutputs([outputs[0], outputs[1]]);

		Assert.Equal(4998, finalOutput.Index);
		AssertLookup(controller, finalOutput, finalAdapter, 4998);
		Assert.Equal(4999, controller.OutputCount);
	}

	/// <summary>
	/// Verifies insertion before an existing output preserves its adapter registration and updates its index.
	/// </summary>
	[Fact]
	public void InsertOutputsAt_UpdatesShiftedSurvivorLookup()
	{
		var controller = CreateController();
		var outputs = AddOutputs(controller, 5);
		var survivor = outputs[3];
		var adapter = controller.GetDataFlowComponentForOutput(survivor);

		controller.InsertOutputsAt(1, 2);

		Assert.Same(survivor, controller.Outputs[5]);
		AssertLookup(controller, survivor, adapter, 5);
		Assert.Equal(7, _dataFlow.GetAllComponents().Count());
	}

	/// <summary>
	/// Verifies insertion at either array boundary keeps existing output registrations synchronized.
	/// </summary>
	[Theory]
	[InlineData(0, 2, 4)]
	[InlineData(5, 4, 4)]
	public void InsertOutputsAt_AtBoundary_UpdatesSurvivorLookup(int insertionIndex, int originalSurvivorIndex, int expectedIndex)
	{
		var controller = CreateController();
		var outputs = AddOutputs(controller, 5);
		var survivor = outputs[originalSurvivorIndex];
		var adapter = controller.GetDataFlowComponentForOutput(survivor);

		controller.InsertOutputsAt(insertionIndex, 2);

		AssertLookup(controller, survivor, adapter, expectedIndex);
		Assert.Equal(7, _dataFlow.GetAllComponents().Count());
	}

	/// <summary>
	/// Verifies survivors retain their registered indexes through a removal followed by an insertion.
	/// </summary>
	[Fact]
	public void RemoveThenInsert_UpdatesSurvivorLookup()
	{
		var controller = CreateController();
		var outputs = AddOutputs(controller, 6);
		var survivor = outputs[5];
		var adapter = controller.GetDataFlowComponentForOutput(survivor);
		var patchSource = CreatePatchSource();
		_dataFlow.AddComponent(patchSource);
		_dataFlow.SetComponentSource(adapter, patchSource, 0);
		var source = adapter.Source;

		controller.RemoveOutputs([outputs[1]]);
		controller.InsertOutputsAt(1, 2);

		AssertLookup(controller, survivor, adapter, 6);
		Assert.Same(source, adapter.Source);
		Assert.Equal(8, _dataFlow.GetAllComponents().Count());
	}

	public void Dispose()
	{
		SetVixenSystemProperty(nameof(VixenSystem.OutputControllers), _previousOutputControllers);
		SetVixenSystemProperty(nameof(VixenSystem.DataFlow), _previousDataFlow);
	}

	private static OutputController CreateController()
	{
		var dataPolicy = new Mock<IDataPolicy>();
		var dataPolicyFactory = new Mock<IDataPolicyFactory>();
		dataPolicyFactory.Setup(factory => factory.CreateDataPolicy()).Returns(dataPolicy.Object);
		var module = new Mock<IControllerModuleInstance>();
		module.SetupGet(instance => instance.DataPolicyFactory).Returns(dataPolicyFactory.Object);
		var consumer = new TestOutputModuleConsumer(module.Object);
		var mediator = new OutputMediator<CommandOutput>(
			new OutputCollection<CommandOutput>(),
			Mock.Of<IUpdatableOutputCount>());

		return new OutputController(Guid.NewGuid(), "Test controller", mediator, Mock.Of<IHardware>(), consumer);
	}

	private static IDataFlowComponent CreatePatchSource()
	{
		var source = new Mock<IDataFlowComponent>();
		source.SetupGet(component => component.DataFlowComponentId).Returns(Guid.NewGuid());
		source.SetupGet(component => component.Name).Returns("Patch source");
		source.SetupGet(component => component.Outputs).Returns([]);
		return source.Object;
	}

	private static CommandOutput[] AddOutputs(OutputController controller, int count)
	{
		var outputs = Enumerable.Range(0, count)
			.Select(index => new CommandOutput(Guid.NewGuid(), $"Output {index + 1}", index))
			.ToArray();
		foreach (var output in outputs)
		{
			controller.AddOutput(output);
		}
		return outputs;
	}

	private void AssertLookup(OutputController controller, CommandOutput output, IDataFlowComponent adapter, int expectedIndex)
	{
		Assert.Same(output, controller.Outputs[expectedIndex]);
		Assert.Equal(expectedIndex, output.Index);
		Assert.Same(adapter, controller.GetDataFlowComponentForOutput(output));
		Assert.True(_outputControllers.getOutputDetailsForDataFlowComponent(adapter, out var actualController, out var actualIndex));
		Assert.Same(controller, actualController);
		Assert.Equal(expectedIndex, actualIndex);
	}

	private sealed class TestOutputModuleConsumer(IControllerModuleInstance module) : IOutputModuleConsumer<IControllerModuleInstance>
	{
		public Guid ModuleId { get; } = Guid.NewGuid();
		public Guid ModuleInstanceId { get; } = Guid.NewGuid();
		public IControllerModuleInstance Module { get; } = module;
		public int UpdateInterval => 0;
		public IOutputDeviceUpdateSignaler UpdateSignaler => null!;
		public bool SupportsNamedOutputs => false;
		public bool IsRunning => false;
		public bool IsPaused => false;
		public bool HasSetup => false;
		public void NameOutputs() { }
		public void Start() { }
		public void Stop() { }
		public void Pause() { }
		public void Resume() { }
		public bool Setup() => false;
	}

	private static void SetVixenSystemProperty(string propertyName, object value)
	{
		var property = typeof(VixenSystem).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
		property!.GetSetMethod(true)!.Invoke(null, [value]);
	}
}

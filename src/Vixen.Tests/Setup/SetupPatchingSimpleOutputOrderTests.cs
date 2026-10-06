using System.Reflection;
using System.Windows.Forms;
using Common.Controls;
using Moq;
using Vixen.Data.Flow;
using Vixen.Module.Controller;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Output;
using Vixen.Tests.Core;
using VixenApplication.Setup;
using Xunit;

namespace Vixen.Tests.Setup;

/// <summary>
/// Verifies selected output bounds and patch destination order in Display Setup.
/// </summary>
[Collection(OutputControllerOutputIndexTestCollection.Name)]
public sealed class SetupPatchingSimpleOutputOrderTests : IDisposable
{
	private readonly OutputControllerManager _previousOutputControllers;
	private readonly DataFlowManager _previousDataFlow;
	private readonly OutputControllerManager _outputControllers;
	private readonly DataFlowManager _dataFlow;

	/// <summary>
	/// Initializes isolated in-memory Vixen managers for the Display Setup tests.
	/// </summary>
	public SetupPatchingSimpleOutputOrderTests()
	{
		_previousOutputControllers = VixenSystem.OutputControllers;
		_previousDataFlow = VixenSystem.DataFlow;
		_outputControllers = new OutputControllerManager(
			new OutputDeviceCollection<OutputController>(),
			new OutputDeviceExecution<OutputController>());
		_dataFlow = new DataFlowManager();
		try
		{
			SetVixenSystemProperty(nameof(VixenSystem.OutputControllers), _outputControllers);
			SetVixenSystemProperty(nameof(VixenSystem.DataFlow), _dataFlow);
		}
		catch
		{
			SetVixenSystemProperty(nameof(VixenSystem.OutputControllers), _previousOutputControllers);
			SetVixenSystemProperty(nameof(VixenSystem.DataFlow), _previousDataFlow);
			throw;
		}
	}

	/// <summary>
	/// Verifies an irregular selection is normalized and reversing changes destinations without changing bounds.
	/// </summary>
	[StaFact]
	public void IrregularSelection_OrdersDestinationsAndKeepsBoundsStableWhenReversed()
	{
		var controller = CreateController("Main", 1704);
		var selection = new ControllersAndOutputsSet { [controller] = [1700, 1638, 1703, 1650] };
		using var control = new SetupPatchingSimple();

		control.UpdateControllerSelection(selection);

		AssertControllerDetails(control, "Main #1639", "Main #1704", "4", "0", "4");
		OutputRadioButton(control, "radioButtonAllOutputs").Checked = true;
		AssertControllerInputIndexes(control, controller, [1638, 1650, 1700, 1703]);
		AssertDestinationIndexes(control, controller, [1638, 1650, 1700, 1703]);
		PatchOutput(controller, 1700);
		var adapter = controller.GetDataFlowComponentForOutput(controller.Outputs[1700]);
		var preservedSource = adapter.Source;

		ReverseOrderCheckBox(control).Checked = true;

		AssertControllerDetails(control, "Main #1639", "Main #1704", "4", "1", "3");
		AssertControllerInputIndexes(control, controller, [1703, 1700, 1650, 1638]);
		Assert.Same(preservedSource, adapter.Source);
		UpdateControllerDetailsForPatching(control, selection);
		AssertControllerInputIndexes(control, controller, [1703, 1700, 1650, 1638]);
		Assert.Same(preservedSource, adapter.Source);
		Assert.Equal("Main #1639", Label(control, "labelFirstOutput").Text);
		Assert.Equal("Main #1704", Label(control, "labelLastOutput").Text);

		ReverseOrderCheckBox(control).Checked = false;
		AssertControllerInputIndexes(control, controller, [1638, 1650, 1700, 1703]);
		UpdateControllerDetailsForPatching(control, selection);
		AssertControllerInputIndexes(control, controller, [1638, 1650, 1700, 1703]);
		Assert.Same(preservedSource, adapter.Source);
	}

	/// <summary>
	/// Verifies descending and ascending manual-like input both display and patch in normal output order.
	/// </summary>
	[StaFact]
	public void ContiguousAndAscendingSelections_UseAscendingOutputOrder()
	{
		var controller = CreateController("Main", 1704);
		using var control = new SetupPatchingSimple();
		OutputRadioButton(control, "radioButtonAllOutputs").Checked = true;
		var descending = new ControllersAndOutputsSet { [controller] = Enumerable.Range(1638, 66).Reverse().ToHashSet() };

		control.UpdateControllerDetails(descending);

		AssertControllerDetails(control, "Main #1639", "Main #1704", "66", "0", "66");
		AssertControllerInputIndexes(control, controller, Enumerable.Range(1638, 66).ToArray());
		AssertDestinationIndexes(control, controller, Enumerable.Range(1638, 66).ToArray());

		control.UpdateControllerSelection(new ControllersAndOutputsSet { [controller] = [2, 3, 4] });

		AssertControllerDetails(control, "Main #3", "Main #5", "3", "0", "3");
		AssertControllerInputIndexes(control, controller, [2, 3, 4]);
	}

	/// <summary>
	/// Verifies filtering unpatched destinations preserves their selected relative order and full-selection bounds.
	/// </summary>
	[StaFact]
	public void UnpatchedOnly_FiltersWithoutChangingSelectionBoundsOrExistingConnections()
	{
		var controller = CreateController("Main", 8);
		var selection = new ControllersAndOutputsSet { [controller] = [6, 1, 5, 2] };
		PatchOutput(controller, 5);
		var sourceReference = controller.GetDataFlowComponentForOutput(controller.Outputs[5]).Source;
		using var control = new SetupPatchingSimple();
		control.UpdateControllerSelection(selection);
		OutputRadioButton(control, "radioButtonUnpatchedOutputsOnly").Checked = true;

		AssertControllerDetails(control, "Main #2", "Main #7", "4", "1", "3");
		AssertDestinationIndexes(control, controller, [1, 2, 6]);

		ReverseOrderCheckBox(control).Checked = true;
		control.UpdateControllerDetails(selection);

		AssertControllerDetails(control, "Main #2", "Main #7", "4", "1", "3");
		AssertDestinationIndexes(control, controller, [6, 2, 1]);
		UpdateControllerDetailsForPatching(control, selection);
		AssertControllerInputIndexes(control, controller, [6, 5, 2, 1]);
		Assert.Same(sourceReference, controller.GetDataFlowComponentForOutput(controller.Outputs[5]).Source);
	}

	/// <summary>
	/// Verifies a single selected output appears at both bounds and an empty replacement clears them.
	/// </summary>
	[StaFact]
	public void SingletonAndEmptySelections_UpdateBoundsSafelyInBothReverseStates()
	{
		var controller = CreateController("Main", 6);
		using var control = new SetupPatchingSimple();
		control.UpdateControllerSelection(new ControllersAndOutputsSet { [controller] = [4] });

		AssertControllerDetails(control, "Main #5", "Main #5", "1", "0", "1");
		ReverseOrderCheckBox(control).Checked = true;
		AssertControllerDetails(control, "Main #5", "Main #5", "1", "0", "1");
		AssertDestinationIndexes(control, controller, [4]);

		control.UpdateControllerSelection(new ControllersAndOutputsSet());

		AssertControllerDetails(control, "", "", "0", "0", "0");
		Assert.Empty(Destinations(control));
		UpdateControllerDetailsForPatching(control, new ControllersAndOutputsSet());
		Assert.Empty(Destinations(control));
		Assert.Equal("", Label(control, "labelFirstOutput").Text);
		Assert.Equal("", Label(control, "labelLastOutput").Text);
	}

	/// <summary>
	/// Verifies controller pane order remains the outer destination order and reverse flips the complete sequence.
	/// </summary>
	[StaFact]
	public void MultipleControllers_UsesSuppliedPaneOrderAndReversesTheCompleteSequence()
	{
		var first = CreateController("First", 6);
		var second = CreateController("Second", 5);
		var selection = new ControllersAndOutputsSet
		{
			[first] = [5, 1],
			[second] = [3, 0]
		};
		using var control = new SetupPatchingSimple();
		control.UpdateControllerSelection(selection);

		Assert.Equal("First #2", Label(control, "labelFirstOutput").Text);
		Assert.Equal("Second #4", Label(control, "labelLastOutput").Text);
		AssertDestinationSequence(control, [(first, 1), (first, 5), (second, 0), (second, 3)]);

		ReverseOrderCheckBox(control).Checked = true;

		Assert.Equal("First #2", Label(control, "labelFirstOutput").Text);
		Assert.Equal("Second #4", Label(control, "labelLastOutput").Text);
		AssertDestinationSequence(control, [(second, 3), (second, 0), (first, 5), (first, 1)]);
	}

	/// <summary>
	/// Verifies paged logical selection exports into the same normalized destination sequence.
	/// </summary>
	[StaFact]
	public void PagedControllerSelection_ExportsAndDisplaysAscendingOutputIndexes()
	{
		var controller = CreateController("Paged", 5001);
		using var controllerTree = new ControllerTree();
		controllerTree.PopulateControllerTreeForTests([controller]);
		controllerTree.SetLogicalSelectionForTests(new Dictionary<IControllerDevice, HashSet<int>>
		{
			[controller] = [5000, 4999, 0]
		});
		var exported = Assert.Single(controllerTree.GetSelectedControllerOutputs());
		var selection = new ControllersAndOutputsSet { [exported.Key] = exported.Value.ToHashSet() };
		using var control = new SetupPatchingSimple();

		control.UpdateControllerSelection(selection);

		AssertControllerDetails(control, "Paged #1", "Paged #5001", "3", "0", "3");
		AssertDestinationIndexes(control, controller, [0, 4999, 5000]);
	}

	/// <summary>
	/// Restores the process-wide managers used by Vixen after each test.
	/// </summary>
	public void Dispose()
	{
		SetVixenSystemProperty(nameof(VixenSystem.OutputControllers), _previousOutputControllers);
		SetVixenSystemProperty(nameof(VixenSystem.DataFlow), _previousDataFlow);
	}

	private OutputController CreateController(string name, int outputCount)
	{
		var dataPolicy = new Mock<IDataPolicy>();
		var dataPolicyFactory = new Mock<IDataPolicyFactory>();
		dataPolicyFactory.Setup(factory => factory.CreateDataPolicy()).Returns(dataPolicy.Object);
		var module = new Mock<IControllerModuleInstance>();
		module.SetupGet(instance => instance.DataPolicyFactory).Returns(dataPolicyFactory.Object);
		var mediator = new OutputMediator<CommandOutput>(
			new OutputCollection<CommandOutput>(),
			Mock.Of<IUpdatableOutputCount>());
		var controller = new OutputController(
			Guid.NewGuid(),
			name,
			mediator,
			Mock.Of<IHardware>(),
			new SetupPatchingTestOutputModuleConsumer(module.Object));

		for (var index = 0; index < outputCount; index++)
		{
			controller.AddOutput(new CommandOutput(Guid.NewGuid(), $"Output {index + 1}", index));
		}
		_outputControllers.Add(controller);
		return controller;
	}

	private void PatchOutput(OutputController controller, int outputIndex)
	{
		var source = new Mock<IDataFlowComponent>();
		source.SetupGet(component => component.DataFlowComponentId).Returns(Guid.NewGuid());
		source.SetupGet(component => component.Name).Returns("Patch source");
		source.SetupGet(component => component.Outputs).Returns([]);
		_dataFlow.AddComponent(source.Object);
		_dataFlow.SetComponentSource(controller.GetDataFlowComponentForOutput(controller.Outputs[outputIndex]), source.Object, 0);
	}

	private static void AssertControllerDetails(
		SetupPatchingSimple control,
		string expectedFirst,
		string expectedLast,
		string expectedCount,
		string expectedPatchedCount,
		string expectedUnpatchedCount)
	{
		Assert.Equal(expectedFirst, Label(control, "labelFirstOutput").Text);
		Assert.Equal(expectedLast, Label(control, "labelLastOutput").Text);
		Assert.Equal(expectedCount, Label(control, "labelOutputCount").Text);
		Assert.Equal(expectedPatchedCount, Label(control, "labelPatchedOutputCount").Text);
		Assert.Equal(expectedUnpatchedCount, Label(control, "labelUnpatchedOutputCount").Text);
	}

	private void AssertControllerInputIndexes(SetupPatchingSimple control, OutputController controller, int[] expectedIndexes)
	{
		var actual = ControllerInputs(control).Select(item =>
		{
			Assert.True(_outputControllers.getOutputDetailsForDataFlowComponent(item.Item, out var owner, out var index));
			Assert.Same(controller, owner);
			return index;
		});
		Assert.Equal(expectedIndexes, actual);
	}

	private void AssertDestinationIndexes(SetupPatchingSimple control, OutputController controller, int[] expectedIndexes)
	{
		var actual = Destinations(control).Select(item =>
		{
			Assert.True(_outputControllers.getOutputDetailsForDataFlowComponent(item.Item, out var owner, out var index));
			Assert.Same(controller, owner);
			return index;
		});
		Assert.Equal(expectedIndexes, actual);
	}

	private void AssertDestinationSequence(SetupPatchingSimple control, (OutputController Controller, int Index)[] expected)
	{
		var actual = ControllerInputs(control).Select(item =>
		{
			Assert.True(_outputControllers.getOutputDetailsForDataFlowComponent(item.Item, out var controller, out var index));
			return (Assert.IsType<OutputController>(controller), index);
		});
		Assert.Equal(expected, actual);
	}

	private static List<PatchStatusItem<IDataFlowComponent>> ControllerInputs(SetupPatchingSimple control) =>
		GetField<List<PatchStatusItem<IDataFlowComponent>>>(control, "_controllerInputs");

	private static List<PatchStatusItem<IDataFlowComponent>> Destinations(SetupPatchingSimple control) =>
		GetField<List<PatchStatusItem<IDataFlowComponent>>>(control, "_selectedPatchDestinations");

	private static Label Label(SetupPatchingSimple control, string name) => FindControl<Label>(control, name);

	private static CheckBox ReverseOrderCheckBox(SetupPatchingSimple control) =>
		FindControl<CheckBox>(control, "checkBoxReverseOutputOrder");

	private static RadioButton OutputRadioButton(SetupPatchingSimple control, string name) => FindControl<RadioButton>(control, name);

	private static TControl FindControl<TControl>(Control control, string name) where TControl : Control
	{
		var matches = control.Controls.Find(name, true);
		var found = Assert.Single(matches);
		return Assert.IsType<TControl>(found);
	}

	private static TField GetField<TField>(object instance, string name)
	{
		var field = typeof(SetupPatchingSimple).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
		return Assert.IsType<TField>(field!.GetValue(instance));
	}

	private static void UpdateControllerDetailsForPatching(SetupPatchingSimple control, ControllersAndOutputsSet selection)
	{
		var method = typeof(SetupPatchingSimple).GetMethod("_updateControllerDetails", BindingFlags.Instance | BindingFlags.NonPublic);
		method!.Invoke(control, [selection, true]);
	}

	private static void SetVixenSystemProperty(string propertyName, object value)
	{
		var property = typeof(VixenSystem).GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
		property!.GetSetMethod(true)!.Invoke(null, [value]);
	}
}

using System.Drawing;
using System.Reflection;
using Moq;
using Vixen.Sys;
using VixenModules.Property.State;
using VixenModules.Property.State.Setup.Preview;
using VixenModules.Property.State.Setup.Services;
using VixenModules.Property.State.Setup.ViewModels;
using Xunit;

namespace Vixen.Tests.Property.State;

[Collection(StateMapperTestCollection.Name)]
public sealed class StateMapperPreviewTests
{
	[Fact]
	public void Constructor_DefaultsPreviewOffAndBuildsOrderedGroups()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, Array.Empty<Guid>()),
			("Closed", Color.Blue, Array.Empty<Guid>()),
			("Open", Color.Green, Array.Empty<Guid>()),
			("open", Color.Yellow, Array.Empty<Guid>()));

		// Assert
		Assert.False(fixture.ViewModel.IsPreviewEnabled);
		Assert.False(fixture.ViewModel.IsStateItemGroupPreviewMode);
		Assert.Equal("<ALL>", fixture.ViewModel.SelectedStateItemGroup);
		Assert.Equal(["<ALL>", "Open", "Closed", "open"], fixture.ViewModel.AvailableStateItemGroups);
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void Constructor_WithNoRows_OffersOnlyAllGroup()
	{
		// Arrange
		var fixture = CreateFixture();

		// Assert
		Assert.Equal(["<ALL>"], fixture.ViewModel.AvailableStateItemGroups);
	}

	[Fact]
	public void EnablingPreview_ActivatesSelectedRowImmediately()
	{
		// Arrange
		var fixture = CreateFixture(("Open", Color.Red, [Guid.Empty]));

		// Act
		fixture.ViewModel.IsPreviewEnabled = true;

		// Assert
		AssertTurnOn(fixture.Publisher, (fixture.LeafIds[0], "#FF0000"));
	}

	[Fact]
	public void EnablingPreview_WithNoRows_DoesNotPublish()
	{
		// Arrange
		var fixture = CreateFixture();

		// Act
		fixture.ViewModel.IsPreviewEnabled = true;

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void DisablingPreview_ClearsContext()
	{
		// Arrange
		var fixture = CreateFixture(("Open", Color.Red, [Guid.Empty]));
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.IsPreviewEnabled = false;

		// Assert
		Assert.Equal("Clear", Assert.Single(fixture.Publisher.Operations).Name);
	}

	[Fact]
	public void SelectingAnotherRow_RefreshesSelectedItemMode()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.SelectedItem = fixture.ViewModel.Items[1];

		// Assert
		Assert.Collection(
			fixture.Publisher.Operations,
			operation => Assert.Equal("TurnOff", operation.Name),
			operation =>
			{
				Assert.Equal("TurnOn", operation.Name);
				Assert.Equal([(fixture.LeafIds[1], "#0000FF")], ToValues(operation.Pairs));
			});
	}

	[Fact]
	public void SelectingAnotherRow_DoesNotRefreshGroupMode()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.SelectedItem = fixture.ViewModel.Items[1];

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void ChangingMode_ClearsBeforeActivatingGroup()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;

		// Assert
		Assert.Collection(
			fixture.Publisher.Operations,
			operation => Assert.Equal("Clear", operation.Name),
			operation => Assert.Equal(2, operation.Pairs.Count));
	}

	[Fact]
	public void ActiveAssignmentAndColorEdits_RefreshPreview()
	{
		// Arrange
		var fixture = CreateFixture(("Open", Color.Red, Array.Empty<Guid>()));
		fixture.ViewModel.IsPreviewEnabled = true;

		// Act
		fixture.ViewModel.Items[0].AssignmentRoots[0].Children[0].IsChecked = true;
		fixture.ViewModel.Items[0].Color = Color.Green;

		// Assert
		Assert.Collection(
			fixture.Publisher.Operations,
			operation => Assert.Equal([(fixture.LeafIds[0], "#FF0000")], ToValues(operation.Pairs)),
			operation => Assert.Equal("TurnOff", operation.Name),
			operation => Assert.Equal([(fixture.LeafIds[0], "#008000")], ToValues(operation.Pairs)));
	}

	[Fact]
	public void InactiveColorEdit_DoesNotRefreshPreview()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.Items[1].Color = Color.Green;

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void NamedGroup_PreviewsOnlyExactNameMatches()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Open", Color.Green, [Guid.Empty]),
			("open", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;
		fixture.ViewModel.SelectedStateItemGroup = "Open";

		// Act
		fixture.ViewModel.IsPreviewEnabled = true;

		// Assert
		Assert.Equal(
			new[] { (fixture.LeafIds[0], "#FF0000"), (fixture.LeafIds[1], "#008000") }
				.OrderBy(pair => pair.Item1),
			ToValues(Assert.Single(fixture.Publisher.Operations).Pairs));
	}

	[Fact]
	public void ChangingNamedGroupWhileEnabled_RefreshesIncrementally()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.SelectedStateItemGroup = "Open";

		// Assert
		var operation = Assert.Single(fixture.Publisher.Operations);
		Assert.Equal("TurnOff", operation.Name);
		Assert.Equal([fixture.LeafIds[1]], operation.ElementIds);
	}

	[Fact]
	public void Rename_RetainsNamedGroupUntilFinalMatchDisappears()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Open", Color.Green, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;
		fixture.ViewModel.SelectedStateItemGroup = "Open";
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.Items[0].Name = "Opening";
		var retainedGroup = fixture.ViewModel.SelectedStateItemGroup;
		fixture.ViewModel.Items[1].Name = "Opening";

		// Assert
		Assert.Equal("Open", retainedGroup);
		Assert.Equal("<ALL>", fixture.ViewModel.SelectedStateItemGroup);
		Assert.Equal(["<ALL>", "Opening", "Closed"], fixture.ViewModel.AvailableStateItemGroups);
		Assert.Contains(
			fixture.Publisher.Operations,
			operation => operation.Pairs.Any(pair => pair.ElementId == fixture.LeafIds[2]));
	}

	[Fact]
	public void Remove_RetainsNamedGroupUntilFinalMatchDisappears()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Open", Color.Green, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;
		fixture.ViewModel.SelectedStateItemGroup = "Open";
		fixture.ViewModel.IsPreviewEnabled = true;

		// Act
		fixture.Publisher.Operations.Clear();
		fixture.ViewModel.Items.RemoveAt(0);
		var retainedGroup = fixture.ViewModel.SelectedStateItemGroup;
		fixture.Publisher.Operations.Clear();
		fixture.ViewModel.Items.RemoveAt(0);

		// Assert
		Assert.Equal("Open", retainedGroup);
		Assert.Equal("<ALL>", fixture.ViewModel.SelectedStateItemGroup);
		Assert.Collection(
			fixture.Publisher.Operations,
			operation => Assert.Equal("TurnOff", operation.Name),
			operation => Assert.Equal([(fixture.LeafIds[2], "#0000FF")], ToValues(operation.Pairs)));
	}

	[Fact]
	public async Task GroupAll_RefreshesAfterAddAndRemove()
	{
		// Arrange
		var fixture = CreateFixture(("Open", Color.Red, [Guid.Empty]));
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		await InvokeTaskAsync(fixture.ViewModel, "AddItemAsync");
		fixture.ViewModel.Items[1].AssignmentRoots[0].Children[1].IsChecked = true;
		fixture.Publisher.Operations.Clear();
		fixture.ViewModel.SelectedItem = fixture.ViewModel.Items[1];
		await InvokeTaskAsync(fixture.ViewModel, "RemoveItemAsync");

		// Assert
		var operation = Assert.Single(fixture.Publisher.Operations);
		Assert.Equal("TurnOff", operation.Name);
		Assert.Equal([fixture.LeafIds[1]], operation.ElementIds);
	}

	[Fact]
	public void OverlappingColors_RepairRemainingColorAfterDeduplication()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Green, [Guid.Empty]));
		fixture.ViewModel.Items[1].AssignmentRoots[0].Children[0].IsChecked = true;
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.Items[0].Color = Color.Green;

		// Assert
		Assert.Collection(
			fixture.Publisher.Operations,
			operation => Assert.Equal([fixture.LeafIds[0]], operation.ElementIds),
			operation => Assert.Equal([(fixture.LeafIds[0], "#008000")], ToValues(operation.Pairs)));
	}

	[Fact]
	public void PreviewOff_SuppressesSelectionsModesGroupsAndEdits()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));

		// Act
		fixture.ViewModel.SelectedItem = fixture.ViewModel.Items[1];
		fixture.ViewModel.IsStateItemGroupPreviewMode = true;
		fixture.ViewModel.SelectedStateItemGroup = "Open";
		fixture.ViewModel.Items[0].Color = Color.Green;
		fixture.ViewModel.Items[0].AssignmentRoots[0].Children[0].IsChecked = false;

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void SwitchingStateDefinition_WhilePreviewIsOn_ClearsAndRendersNewDefinition()
	{
		// Arrange
		var fixture = CreateDefinitionFixture(
			[
				("Open", Color.Red, [Guid.Empty])
			],
			[
				("Closed", Color.Blue, [Guid.Empty])
			]);
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.SelectedStateDefinition = fixture.ViewModel.StateDefinitions[1];

		// Assert
		Assert.Collection(
			fixture.Publisher.Operations,
			operation => Assert.Equal("Clear", operation.Name),
			operation =>
			{
				Assert.Equal("TurnOn", operation.Name);
				Assert.Equal([(fixture.LeafIds[1], "#0000FF")], ToValues(operation.Pairs));
			});
	}

	[Fact]
	public void SwitchingStateDefinition_WhilePreviewIsOff_DoesNotPublish()
	{
		// Arrange
		var fixture = CreateDefinitionFixture(
			[
				("Open", Color.Red, [Guid.Empty])
			],
			[
				("Closed", Color.Blue, [Guid.Empty])
			]);

		// Act
		fixture.ViewModel.SelectedStateDefinition = fixture.ViewModel.StateDefinitions[1];

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
		Assert.Equal("Closed", fixture.ViewModel.Items[0].Name);
	}

	[Fact]
	public void InactiveStateDefinitionEdits_DoNotPublish()
	{
		// Arrange
		var fixture = CreateDefinitionFixture(
			[
				("Open", Color.Red, [Guid.Empty])
			],
			[
				("Closed", Color.Blue, [Guid.Empty])
			]);
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.StateDefinitions[1].Items[0].Color = Color.Green;
		fixture.ViewModel.StateDefinitions[1].Items[0].AssignmentRoots[0].Children[1].IsChecked = true;

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public async Task Closed_ReleasesContextWhenPreviewIsOff()
	{
		// Arrange
		var fixture = CreateFixture(("Open", Color.Red, [Guid.Empty]));

		// Act
		await InvokeOnClosedAsync(fixture.ViewModel);

		// Assert
		Assert.Equal("Release", Assert.Single(fixture.Publisher.Operations).Name);
	}

	private static MapperFixture CreateFixture(params (string Name, Color Color, Guid[] ElementIds)[] items)
	{
		var leafIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
		var rootNode = CreateNode(
			Guid.NewGuid(),
			"Root",
			leafIds.Select((id, index) => CreateNode(id, $"Leaf {index + 1}")).ToArray());
		var source = new StateData
		{
			Name = "Operating Mode",
			Items = items
				.Select((item, index) => new StateItemData
				{
					Name = item.Name,
					Color = item.Color,
					ElementNodeIds = item.ElementIds
						.Select(id => id == Guid.Empty ? leafIds[index] : id)
						.ToList()
				})
				.ToList()
		};
		var publisher = new RecordingStatePreviewPublisher();
		var viewModel = new StateMapperViewModel(
			rootNode,
			source,
			Mock.Of<IStateColorPickerService>(),
			publisher);
		return new MapperFixture(viewModel, publisher, leafIds);
	}

	private static MapperFixture CreateDefinitionFixture(
		params (string Name, Color Color, Guid[] ElementIds)[][] definitions)
	{
		var leafIds = new[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
		var rootNode = CreateNode(
			Guid.NewGuid(),
			"Root",
			leafIds.Select((id, index) => CreateNode(id, $"Leaf {index + 1}")).ToArray());
		var source = new StateData
		{
			StateDefinitions = definitions
				.Select((definition, definitionIndex) => new StateDefinitionData
				{
					Name = $"Definition {definitionIndex + 1}",
					Items = definition
						.Select((item, itemIndex) => new StateItemData
						{
							Name = item.Name,
							Color = item.Color,
							ElementNodeIds = item.ElementIds
								.Select(id => id == Guid.Empty ? leafIds[definitionIndex + itemIndex] : id)
								.ToList()
						})
						.ToList()
				})
				.ToList()
		};
		var publisher = new RecordingStatePreviewPublisher();
		var viewModel = new StateMapperViewModel(
			rootNode,
			source,
			Mock.Of<IStateColorPickerService>(),
			publisher);
		return new MapperFixture(viewModel, publisher, leafIds);
	}

	private static IElementNode CreateNode(Guid id, string name, params IElementNode[] children)
	{
		var node = new Mock<IElementNode>();
		node.SetupGet(elementNode => elementNode.Id).Returns(id);
		node.SetupGet(elementNode => elementNode.Name).Returns(name);
		node.SetupGet(elementNode => elementNode.Children).Returns(children);
		node.Setup(elementNode => elementNode.GetNodeEnumerator()).Returns(children);
		return node.Object;
	}

	private static Task InvokeTaskAsync(StateMapperViewModel viewModel, string methodName)
	{
		var method = typeof(StateMapperViewModel).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);
		return Assert.IsAssignableFrom<Task>(method.Invoke(viewModel, null));
	}

	private static async Task InvokeOnClosedAsync(StateMapperViewModel viewModel)
	{
		var method = typeof(StateMapperViewModel).GetMethod("OnClosedAsync", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);
		await Assert.IsAssignableFrom<Task>(method.Invoke(viewModel, [null]));
	}

	private static void AssertTurnOn(
		RecordingStatePreviewPublisher publisher,
		params (Guid ElementId, string Color)[] expectedPairs)
	{
		var operation = Assert.Single(publisher.Operations);
		Assert.Equal("TurnOn", operation.Name);
		Assert.Equal(expectedPairs.OrderBy(pair => pair.ElementId), ToValues(operation.Pairs));
	}

	private static (Guid ElementId, string Color)[] ToValues(IReadOnlyCollection<StatePreviewPair> pairs)
	{
		return pairs
			.Select(pair => (pair.ElementId, pair.Color))
			.OrderBy(pair => pair.ElementId)
			.ToArray();
	}

	private sealed record MapperFixture(
		StateMapperViewModel ViewModel,
		RecordingStatePreviewPublisher Publisher,
		Guid[] LeafIds);

	private sealed class RecordingStatePreviewPublisher : IStatePreviewPublisher
	{
		internal List<PreviewOperation> Operations { get; } = [];

		public void TurnOn(IReadOnlyCollection<StatePreviewPair> pairs)
		{
			Operations.Add(new PreviewOperation("TurnOn", pairs.ToArray(), []));
		}

		public void TurnOff(IReadOnlyCollection<Guid> elementIds)
		{
			Operations.Add(new PreviewOperation("TurnOff", [], elementIds.ToArray()));
		}

		public void Clear()
		{
			Operations.Add(new PreviewOperation("Clear", [], []));
		}

		public void Release()
		{
			Operations.Add(new PreviewOperation("Release", [], []));
		}
	}

	private sealed record PreviewOperation(
		string Name,
		IReadOnlyCollection<StatePreviewPair> Pairs,
		IReadOnlyCollection<Guid> ElementIds);
}

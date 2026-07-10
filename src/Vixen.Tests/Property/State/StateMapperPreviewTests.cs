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
	public void Constructor_DefaultsPreviewOffAndNoSelectedRows()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, Array.Empty<Guid>()),
			("Closed", Color.Blue, Array.Empty<Guid>()));

		// Assert
		Assert.False(fixture.ViewModel.IsPreviewEnabled);
		Assert.Empty(fixture.ViewModel.SelectedItems);
		Assert.Empty(fixture.ViewModel.AssignedElementRoots);
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void EnablingPreview_WithNoSelectedRows_DoesNotPublish()
	{
		// Arrange
		var fixture = CreateFixture(("Open", Color.Red, [Guid.Empty]));

		// Act
		fixture.ViewModel.IsPreviewEnabled = true;

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void SelectingOneRow_RefreshesPreviewAndShowsAssignments()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));
		fixture.ViewModel.IsPreviewEnabled = true;

		// Act
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);

		// Assert
		AssertTurnOn(fixture.Publisher, (fixture.LeafIds[0], "#FF0000"));
		Assert.Same(fixture.ViewModel.Items[0], fixture.ViewModel.SelectedItem);
		Assert.Same(fixture.ViewModel.Items[0].AssignmentRoots[0], Assert.Single(fixture.ViewModel.AssignedElementRoots));
	}

	[Fact]
	public void SelectingMultipleRows_PreviewsAllSelectedRowsAndHidesAssignments()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]),
			("Blink", Color.Green, [Guid.Empty]));
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[2]);

		// Assert
		var operation = Assert.Single(fixture.Publisher.Operations);
		Assert.Equal("TurnOn", operation.Name);
		Assert.Equal(
			[(fixture.LeafIds[2], "#008000")],
			ToValues(operation.Pairs));
		Assert.Empty(fixture.ViewModel.AssignedElementRoots);
	}

	[Fact]
	public void ClearingSelection_TurnsOffPreviewedRows()
	{
		// Arrange
		var fixture = CreateFixture(("Open", Color.Red, [Guid.Empty]));
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.SelectedItems.Clear();

		// Assert
		var operation = Assert.Single(fixture.Publisher.Operations);
		Assert.Equal("TurnOff", operation.Name);
		Assert.Equal([fixture.LeafIds[0]], operation.ElementIds);
		Assert.Null(fixture.ViewModel.SelectedItem);
		Assert.Empty(fixture.ViewModel.AssignedElementRoots);
	}

	[Fact]
	public void ActiveAssignmentAndColorEdits_RefreshPreview()
	{
		// Arrange
		var fixture = CreateFixture(("Open", Color.Red, Array.Empty<Guid>()));
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);
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
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.Items[1].Color = Color.Green;

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void PreviewOff_SuppressesSelectionsAndEdits()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Blue, [Guid.Empty]));

		// Act
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[1]);
		fixture.ViewModel.Items[0].Color = Color.Green;
		fixture.ViewModel.Items[0].AssignmentRoots[0].Children[0].IsChecked = false;

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
	}

	[Fact]
	public void SwitchingStateDefinition_WhilePreviewIsOn_ClearsSelectedItemPreview()
	{
		// Arrange
		var fixture = CreateDefinitionFixture(
			[
				("Open", Color.Red, [Guid.Empty])
			],
			[
				("Closed", Color.Blue, [Guid.Empty])
			]);
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);
		fixture.ViewModel.IsPreviewEnabled = true;
		fixture.Publisher.Operations.Clear();

		// Act
		fixture.ViewModel.SelectedStateDefinition = fixture.ViewModel.StateDefinitions[1];

		// Assert
		Assert.Equal("Clear", Assert.Single(fixture.Publisher.Operations).Name);
		Assert.Null(fixture.ViewModel.SelectedItem);
		Assert.Empty(fixture.ViewModel.SelectedItems);
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
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);

		// Act
		fixture.ViewModel.SelectedStateDefinition = fixture.ViewModel.StateDefinitions[1];

		// Assert
		Assert.Empty(fixture.Publisher.Operations);
		Assert.Equal("Closed", fixture.ViewModel.Items[0].Name);
		Assert.Null(fixture.ViewModel.SelectedItem);
		Assert.Empty(fixture.ViewModel.SelectedItems);
	}

	[Fact]
	public void OverlappingColors_RepairRemainingColorAfterDeduplication()
	{
		// Arrange
		var fixture = CreateFixture(
			("Open", Color.Red, [Guid.Empty]),
			("Closed", Color.Green, [Guid.Empty]));
		fixture.ViewModel.Items[1].AssignmentRoots[0].Children[0].IsChecked = true;
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[0]);
		fixture.ViewModel.SelectedItems.Add(fixture.ViewModel.Items[1]);
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
			StateDefinitions =
			[
				new StateDefinitionData
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
				}
			]
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

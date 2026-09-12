using System.Windows;
using GongSolutions.Wpf.DragDrop;
using Moq;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using VixenModules.App.CustomPropEditor.ViewModels;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor;

[Collection("CustomPropEditor")]
public sealed class ElementTreeDragDropTests : IDisposable
{
	public ElementTreeDragDropTests()
	{
		ElementModelLookUpService.Instance.Reset();
	}

	public void Dispose()
	{
		ElementModelLookUpService.Instance.Reset();
	}

	[Fact]
	public void TryMoveWithinParent_MovesMultipleItemsDownwardBeforeTarget()
	{
		var parent = CreateParent("A", "B", "C", "D", "E", "F", "G");
		var models = parent.Children.ToList();

		var moved = PropModelServices.Instance().TryMoveWithinParent(parent, [models[1], models[2], models[3]], 6);

		Assert.True(moved);
		Assert.Equal(["A", "E", "F", "B", "C", "D", "G"], parent.Children.Select(model => model.Name));
		AssertOriginalModelsRemainExactlyOnce(parent, models);
	}

	[Fact]
	public void TryMoveWithinParent_MovesMultipleItemsToEndWhenInsertionSlotEqualsCount()
	{
		var parent = CreateParent("A", "B", "C", "D", "E", "F", "G");
		var models = parent.Children.ToList();

		var moved = PropModelServices.Instance().TryMoveWithinParent(parent, [models[1], models[2], models[3]], parent.Children.Count);

		Assert.True(moved);
		Assert.Equal(["A", "E", "F", "G", "B", "C", "D"], parent.Children.Select(model => model.Name));
		AssertOriginalModelsRemainExactlyOnce(parent, models);
	}

	[Fact]
	public void TryMoveWithinParent_ReturnsTrueWithoutChangingOrderForSelectedBlock()
	{
		var parent = CreateParent("A", "B", "C", "D", "E", "F", "G");
		var models = parent.Children.ToList();

		var moved = PropModelServices.Instance().TryMoveWithinParent(parent, [models[1], models[2], models[3]], 3);

		Assert.True(moved);
		Assert.Equal(["A", "B", "C", "D", "E", "F", "G"], parent.Children.Select(model => model.Name));
		AssertOriginalModelsRemainExactlyOnce(parent, models);
	}

	[Theory]
	[InlineData(-1)]
	[InlineData(4)]
	public void TryMoveWithinParent_ReturnsFalseWithoutMutationForInvalidInsertionIndex(int insertionIndex)
	{
		var parent = CreateParent("A", "B", "C");
		var models = parent.Children.ToList();

		var moved = PropModelServices.Instance().TryMoveWithinParent(parent, [models[1]], insertionIndex);

		Assert.False(moved);
		AssertOriginalModelsRemainExactlyOnce(parent, models);
	}

	[Fact]
	public void TryMoveWithinParent_ReturnsFalseWithoutMutationForInvalidModels()
	{
		var parent = CreateParent("A", "B", "C");
		var models = parent.Children.ToList();
		var service = PropModelServices.Instance();

		Assert.False(service.TryMoveWithinParent(parent, [], 1));
		AssertOriginalModelsRemainExactlyOnce(parent, models);

		Assert.False(service.TryMoveWithinParent(parent, [models[1], models[1]], 1));
		AssertOriginalModelsRemainExactlyOnce(parent, models);

		var otherParent = CreateParent("Other");
		Assert.False(service.TryMoveWithinParent(parent, [otherParent.Children.Single()], 1));
		AssertOriginalModelsRemainExactlyOnce(parent, models);
	}

	[Fact]
	public void Drop_MovesThreeItemsDownwardBeforeTarget()
	{
		var tree = CreateSiblingTree("A", "B", "C", "D", "E", "F", "G");
		var draggedModels = new List<ElementModelViewModel> { tree.Children[1], tree.Children[2], tree.Children[3] };

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.Children[6],
			RelativeInsertPosition.BeforeTargetItem,
			6));

		AssertSameParentDrop(tree, ["A", "E", "F", "B", "C", "D", "G"], draggedModels);
	}

	[Fact]
	public void Drop_MovesThreeItemsAfterFinalItemWhenInsertionSlotEqualsCount()
	{
		var tree = CreateSiblingTree("A", "B", "C", "D", "E", "F", "G");
		var draggedModels = new List<ElementModelViewModel> { tree.Children[1], tree.Children[2], tree.Children[3] };

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.Children[6],
			RelativeInsertPosition.AfterTargetItem,
			7));

		AssertSameParentDrop(tree, ["A", "E", "F", "G", "B", "C", "D"], draggedModels);
	}

	[Fact]
	public void Drop_MovesSingleItemDownwardBeforeTarget()
	{
		var tree = CreateSiblingTree("A", "B", "C", "D", "E", "F", "G");
		var draggedModels = new List<ElementModelViewModel> { tree.Children[1] };

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.Children[4],
			RelativeInsertPosition.BeforeTargetItem,
			4));

		AssertSameParentDrop(tree, ["A", "C", "D", "B", "E", "F", "G"], draggedModels);
	}

	[Fact]
	public void Drop_MovesSingleItemUpwardAfterTarget()
	{
		var tree = CreateSiblingTree("A", "B", "C", "D", "E", "F", "G");
		var draggedModels = new List<ElementModelViewModel> { tree.Children[4] };

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.Children[1],
			RelativeInsertPosition.AfterTargetItem,
			2));

		AssertSameParentDrop(tree, ["A", "B", "E", "C", "D", "F", "G"], draggedModels);
	}

	[Fact]
	public void Drop_MovesNoncontiguousSelectionWithoutChangingInputOrder()
	{
		var tree = CreateSiblingTree("A", "B", "C", "D", "E", "F", "G");
		var draggedModels = new List<ElementModelViewModel> { tree.Children[1], tree.Children[3] };

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.Children[6],
			RelativeInsertPosition.BeforeTargetItem,
			6));

		AssertSameParentDrop(tree, ["A", "C", "E", "F", "B", "D", "G"], draggedModels);
	}

	[Fact]
	public void Drop_ReversesBlockWhenControlIsPressed()
	{
		var tree = CreateSiblingTree("A", "B", "C", "D", "E", "F", "G");
		var draggedModels = new List<ElementModelViewModel> { tree.Children[1], tree.Children[2], tree.Children[3] };

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.Children[6],
			RelativeInsertPosition.BeforeTargetItem,
			6,
			DragDropKeyStates.ControlKey));

		AssertSameParentDrop(tree, ["A", "E", "F", "D", "C", "B", "G"], draggedModels);
	}

	[Fact]
	public void Drop_LeavesOrderUnchangedWhenDroppingWithinSelectedBlock()
	{
		var tree = CreateSiblingTree("A", "B", "C", "D", "E", "F", "G");
		var draggedModels = new List<ElementModelViewModel> { tree.Children[1], tree.Children[2], tree.Children[3] };

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.Children[3],
			RelativeInsertPosition.BeforeTargetItem,
			3));

		AssertSameParentDrop(tree, ["A", "B", "C", "D", "E", "F", "G"], draggedModels);
	}

	[Fact]
	public void Drop_MovesMultipleItemsAcrossParentsWithoutLosingLightsOrIdentity()
	{
		var tree = CreateCrossParentTree();
		var draggedModels = new List<ElementModelViewModel> { tree.SourceChildren[1], tree.SourceChildren[2] };
		var movedModels = new[] { tree.SourceModels[1], tree.SourceModels[2] };
		var movedLights = movedModels.Select(model => model.Lights.Single()).ToList();

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.TargetChildren[2],
			RelativeInsertPosition.BeforeTargetItem,
			2));

		Assert.Equal(["A", "D"], tree.Source.ElementModel.Children.Select(model => model.Name));
		Assert.Equal(["E", "F", "B", "C", "G"], tree.Target.ElementModel.Children.Select(model => model.Name));
		AssertModelsAppearExactlyOnce(tree.Prop, tree.OriginalModels);

		for (var index = 0; index < movedModels.Length; index++)
		{
			var movedModel = movedModels[index];
			var targetViewModel = tree.Target.ChildrenViewModels.Single(viewModel => ReferenceEquals(viewModel.ElementModel, movedModel));
			Assert.Same(movedModel, tree.Target.ElementModel.Children[2 + index]);
			Assert.DoesNotContain(tree.Source.ElementModel.Id, movedModel.Parents);
			Assert.Contains(tree.Target.ElementModel.Id, movedModel.Parents);
			Assert.Same(movedLights[index], movedModel.Lights.Single());
			Assert.Equal(movedModel.Id, movedLights[index].ParentModelId);
			Assert.NotSame(draggedModels[index], targetViewModel);
			Assert.True(targetViewModel.IsSelected);
		}

		Assert.True(tree.Target.IsExpanded);
	}

	[Fact]
	public void Drop_PrioritizesCenterFlagAndAppendsToTargetGroup()
	{
		var tree = CreateCrossParentTree();
		var draggedModels = new List<ElementModelViewModel> { tree.SourceChildren[1], tree.SourceChildren[2] };
		var movedModels = new[] { tree.SourceModels[1], tree.SourceModels[2] };

		tree.ViewModel.Drop(CreateDropInfo(
			draggedModels,
			tree.Target,
			RelativeInsertPosition.TargetItemCenter | RelativeInsertPosition.BeforeTargetItem,
			0));

		Assert.Equal(["E", "F", "G", "B", "C"], tree.Target.ElementModel.Children.Select(model => model.Name));
		Assert.Equal(["A", "D"], tree.Source.ElementModel.Children.Select(model => model.Name));
		AssertModelsAppearExactlyOnce(tree.Prop, tree.OriginalModels);
		Assert.All(movedModels, model => Assert.Contains(tree.Target.ElementModel.Id, model.Parents));
		Assert.True(tree.Target.IsExpanded);
	}

	private static ElementModel CreateParent(params string[] childNames)
	{
		var parent = new ElementModel("Parent");
		foreach (var childName in childNames)
		{
			parent.AddChild(new ElementModel(childName, parent));
		}

		return parent;
	}

	private static void AssertOriginalModelsRemainExactlyOnce(ElementModel parent, IReadOnlyList<ElementModel> models)
	{
		Assert.Equal(models.Count, parent.Children.Count);
		Assert.All(models, model => Assert.Equal(1, parent.Children.Count(child => ReferenceEquals(child, model))));
	}

	private static (
		ElementTreeViewModel ViewModel,
		ElementModelViewModel Parent,
		List<ElementModelViewModel> Children,
		List<ElementModel> Models) CreateSiblingTree(params string[] childNames)
	{
		var prop = new Prop("Drag drop test");
		var parent = new ElementModel("Parent", prop.RootNode);
		foreach (var childName in childNames)
		{
			parent.AddChild(new ElementModel(childName, parent));
		}

		prop.RootNode.AddChild(parent);
		var viewModel = new ElementTreeViewModel(prop);
		var parentViewModel = Assert.Single(viewModel.RootNodesViewModels.Single().ChildrenViewModels);
		return (viewModel, parentViewModel, parentViewModel.ChildrenViewModels.ToList(), parent.Children.ToList());
	}

	private static (
		Prop Prop,
		ElementTreeViewModel ViewModel,
		ElementModelViewModel Source,
		ElementModelViewModel Target,
		List<ElementModelViewModel> SourceChildren,
		List<ElementModelViewModel> TargetChildren,
		List<ElementModel> SourceModels,
		List<ElementModel> OriginalModels) CreateCrossParentTree()
	{
		var prop = new Prop("Cross parent drag drop test");
		var source = new ElementModel("Source", prop.RootNode);
		var target = new ElementModel("Target", prop.RootNode);
		var sourceModels = new List<ElementModel>
		{
			CreateLightChild("A", source, 10),
			CreateLightChild("B", source, 20),
			CreateLightChild("C", source, 30),
			CreateLightChild("D", source, 40)
		};
		var targetModels = new List<ElementModel>
		{
			CreateLightChild("E", target, 50),
			CreateLightChild("F", target, 60),
			CreateLightChild("G", target, 70)
		};
		prop.RootNode.AddChild(source);
		prop.RootNode.AddChild(target);

		var viewModel = new ElementTreeViewModel(prop);
		var rootViewModel = viewModel.RootNodesViewModels.Single();
		var sourceViewModel = rootViewModel.ChildrenViewModels.Single(viewModel => ReferenceEquals(viewModel.ElementModel, source));
		var targetViewModel = rootViewModel.ChildrenViewModels.Single(viewModel => ReferenceEquals(viewModel.ElementModel, target));
		return (
			prop,
			viewModel,
			sourceViewModel,
			targetViewModel,
			sourceViewModel.ChildrenViewModels.ToList(),
			targetViewModel.ChildrenViewModels.ToList(),
			sourceModels,
			[source, target, .. sourceModels, .. targetModels]);
	}

	private static ElementModel CreateLightChild(string name, ElementModel parent, double x)
	{
		var child = new ElementModel(name, parent);
		child.Lights.Add(new Light(new Point(x, x), ElementModel.DefaultLightSize, child.Id));
		parent.AddChild(child);
		return child;
	}

	private static IDropInfo CreateDropInfo(
		IList<ElementModelViewModel> draggedModels,
		ElementModelViewModel targetModel,
		RelativeInsertPosition insertPosition,
		int unfilteredInsertIndex,
		DragDropKeyStates keyStates = default)
	{
		var dropInfo = new Mock<IDropInfo>();
		dropInfo.SetupGet(info => info.Data).Returns(draggedModels);
		dropInfo.SetupGet(info => info.TargetItem).Returns(targetModel);
		dropInfo.SetupGet(info => info.InsertPosition).Returns(insertPosition);
		dropInfo.SetupGet(info => info.UnfilteredInsertIndex).Returns(unfilteredInsertIndex);
		dropInfo.SetupGet(info => info.InsertIndex).Returns(0);
		dropInfo.SetupGet(info => info.Effects).Returns(DragDropEffects.Move);
		dropInfo.SetupGet(info => info.KeyStates).Returns(keyStates);
		return dropInfo.Object;
	}

	private static void AssertSameParentDrop(
		(ElementTreeViewModel ViewModel, ElementModelViewModel Parent, List<ElementModelViewModel> Children, List<ElementModel> Models) tree,
		IReadOnlyList<string> expectedOrder,
		IReadOnlyList<ElementModelViewModel> draggedModels)
	{
		Assert.Equal(expectedOrder, tree.Parent.ElementModel.Children.Select(model => model.Name));
		AssertOriginalModelsRemainExactlyOnce(tree.Parent.ElementModel, tree.Models);
		foreach (var draggedModel in draggedModels)
		{
			var currentViewModel = tree.Parent.ChildrenViewModels.Single(viewModel => ReferenceEquals(viewModel.ElementModel, draggedModel.ElementModel));
			Assert.Same(draggedModel, currentViewModel);
			Assert.True(draggedModel.IsSelected);
		}
	}

	private static void AssertModelsAppearExactlyOnce(Prop prop, IReadOnlyList<ElementModel> models)
	{
		var treeModels = prop.RootNode.GetNodeEnumerator().ToList();
		Assert.All(models, model => Assert.Equal(1, treeModels.Count(treeModel => ReferenceEquals(treeModel, model))));
	}
}

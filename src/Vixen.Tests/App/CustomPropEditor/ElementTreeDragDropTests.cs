using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor;

[Collection("CustomPropEditor")]
public sealed class ElementTreeDragDropTests
{
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
}

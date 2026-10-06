using System.Reflection;
using System.Windows.Forms;
using Common.Controls.DragDropListView;
using Moq;
using Vixen.Sys;
using Vixen.Tests.Core;
using VixenModules.Property.Order;
using Xunit;

namespace Vixen.Tests.Setup;

/// <summary>
/// Verifies Zig Zag reorders only selected rows in the Patching Order list.
/// </summary>
[Collection(OutputControllerOutputIndexTestCollection.Name)]
public sealed class OrderSetupHelperZigZagTests
{
	/// <summary>
	/// Verifies an offset contiguous selection reverses its second group only.
	/// </summary>
	[StaFact]
	public void ContiguousOffsetSelection_ReversesOnlySecondSelectedGroup()
	{
		AssertZigZag(
			10,
			[3, 4, 5, 6],
			2,
			[1, 2, 3, 4, 6, 5, 7, 8, 9, 10]);
	}

	/// <summary>
	/// Verifies a gapped selection reverses selected positions without moving unselected rows.
	/// </summary>
	[StaFact]
	public void GappedSelection_ReversesOnlySelectedPositions()
	{
		AssertZigZag(
			10,
			[2, 4, 6, 8],
			2,
			[1, 2, 3, 4, 5, 8, 7, 6, 9, 10],
			descendingSelectionOrder: true);
	}

	/// <summary>
	/// Verifies selecting every row preserves the existing alternating pair behavior.
	/// </summary>
	[StaFact]
	public void AllRowsSelection_PreservesAlternatingPairBehavior()
	{
		AssertZigZag(
			10,
			[1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
			2,
			[1, 2, 4, 3, 5, 6, 8, 7, 9, 10]);
	}

	/// <summary>
	/// Verifies an odd-length selected group reverses around its unchanged middle row.
	/// </summary>
	[StaFact]
	public void GappedOddLength_ReversesGroupAndKeepsMiddleIdentity()
	{
		AssertZigZag(
			10,
			[2, 4, 6, 7, 9, 10],
			3,
			[1, 2, 3, 4, 5, 6, 10, 8, 9, 7]);
	}

	/// <summary>
	/// Verifies each alternating group reverses across several gaps in the selection.
	/// </summary>
	[StaFact]
	public void MultipleGappedGroups_ReverseEverySecondGroup()
	{
		AssertZigZag(
			16,
			[2, 4, 6, 8, 10, 12, 14, 16],
			2,
			[1, 2, 3, 4, 5, 8, 7, 6, 9, 10, 11, 12, 13, 16, 15, 14]);
	}

	/// <summary>
	/// Verifies a Zig Zag length equal to the selected count leaves its single group unchanged.
	/// </summary>
	[StaFact]
	public void LengthEqualsSelectedCount_LeavesOneGroupUnchanged()
	{
		AssertZigZag(
			10,
			[2, 4, 6, 8],
			4,
			[1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);
	}

	private static void AssertZigZag(
		int itemCount,
		int[] selectedIdentities,
		int length,
		int[] expectedIdentities,
		bool descendingSelectionOrder = false)
	{
		using var helper = new OrderSetupHelper();
		var matches = helper.Controls.Find("elementList", true);
		var elementList = Assert.IsType<DragDropListView>(Assert.Single(matches));
		_ = elementList.Handle;

		var nodes = Enumerable.Range(1, itemCount).Select(identity =>
		{
			var node = new Mock<IElementNode>();
			node.SetupGet(element => element.Name).Returns($"Element {identity}");
			return node.Object;
		}).ToArray();
		var originalItems = new ListViewItem[itemCount];
		for (var index = 0; index < itemCount; index++)
		{
			var item = new ListViewItem((index + 1).ToString()) { Tag = nodes[index] };
			item.SubItems.Add(nodes[index].Name);
			elementList.Items.Add(item);
			originalItems[index] = item;
		}

		IEnumerable<int> selectionOrder = descendingSelectionOrder
			? selectedIdentities.OrderByDescending(identity => identity)
			: selectedIdentities;
		foreach (var identity in selectionOrder)
		{
			elementList.Items[identity - 1].Selected = true;
		}

		var selectedIndexes = selectedIdentities.Select(identity => identity - 1).Order().ToArray();
		Assert.Equal(selectedIndexes, elementList.SelectedIndices.Cast<int>().ToArray());

		var method = typeof(OrderSetupHelper).GetMethod("PerformZigZag", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(method);
		method.Invoke(helper, [length]);

		AssertListState(helper, elementList, nodes, originalItems, selectedIndexes, expectedIdentities);

		method.Invoke(helper, [length]);

		AssertListState(
			helper,
			elementList,
			nodes,
			originalItems,
			selectedIndexes,
			Enumerable.Range(1, itemCount).ToArray());
	}

	private static void AssertListState(
		OrderSetupHelper helper,
		DragDropListView elementList,
		IElementNode[] nodes,
		ListViewItem[] originalItems,
		int[] selectedIndexes,
		int[] expectedIdentities)
	{
		Assert.Equal(nodes.Length, elementList.Items.Count);
		Assert.Equal(expectedIdentities.Select(identity => $"Element {identity}"),
			elementList.Items.Cast<ListViewItem>().Select(item => item.SubItems[1].Text));

		var actualNodes = elementList.Items.Cast<ListViewItem>()
			.Select(item => Assert.IsType<IElementNode>(item.Tag, exactMatch: false))
			.ToArray();
		for (var index = 0; index < actualNodes.Length; index++)
		{
			Assert.Same(nodes[expectedIdentities[index] - 1], actualNodes[index]);
		}
		foreach (var node in nodes)
		{
			Assert.Single(actualNodes, actual => ReferenceEquals(actual, node));
		}

		for (var index = 0; index < elementList.Items.Count; index++)
		{
			Assert.Equal((index + 1).ToString(), elementList.Items[index].Text);
		}

		var selectedIndexesAfter = elementList.SelectedIndices.Cast<int>().ToArray();
		Assert.Equal(selectedIndexes, selectedIndexesAfter);
		for (var index = 0; index < elementList.Items.Count; index++)
		{
			var isSelected = Array.BinarySearch(selectedIndexes, index) >= 0;
			Assert.Equal(isSelected, elementList.Items[index].Selected);
			if (!isSelected)
			{
				Assert.Same(originalItems[index], elementList.Items[index]);
			}
		}

		var field = typeof(OrderSetupHelper).GetField("_elementOrderLookup", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(field);
		var elementOrderLookup = Assert.IsType<Dictionary<IElementNode, int>>(field.GetValue(helper));
		for (var index = 0; index < elementList.Items.Count; index++)
		{
			var node = Assert.IsType<IElementNode>(elementList.Items[index].Tag, exactMatch: false);
			Assert.Equal(index + 1, elementOrderLookup[node]);
		}
	}
}

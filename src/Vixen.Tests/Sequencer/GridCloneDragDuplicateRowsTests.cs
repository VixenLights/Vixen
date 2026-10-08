using System.Drawing;
using Common.Controls.Timeline;
using Xunit;
using FactAttribute = Xunit.WinFormsFactAttribute;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class GridCloneDragDuplicateRowsTests
{
	[Fact]
	public void MoveElementsVerticallyToLocation_WhenCloneCrossesDuplicateRows_TracksThePointerRow()
	{
		using Grid grid = CreateGrid(out Row[] rows);
		Element original = new();
		Element firstClone = new() { Selected = true };
		Element secondClone = new() { Selected = true };
		rows[0].AddElement(original);
		rows[0].AddElement(firstClone);
		rows[0].AddElement(secondClone);
		SetDragContext(grid, [original], rows[0], 0);

		MoveToRow(grid, [firstClone, secondClone], rows[1]);

		Assert.Same(rows[1], GetMouseDownElementRow(grid));
		Assert.True(rows[1].ContainsElement(firstClone));
		Assert.True(rows[3].ContainsElement(firstClone));
		Assert.True(rows[1].ContainsElement(secondClone));
		Assert.True(rows[3].ContainsElement(secondClone));

		MoveToRow(grid, [firstClone, secondClone], rows[2]);
		Assert.Same(rows[2], GetMouseDownElementRow(grid));

		MoveToRow(grid, [firstClone, secondClone], rows[3]);
		Assert.Same(rows[3], GetMouseDownElementRow(grid));

		MoveToRow(grid, [firstClone, secondClone], rows[2]);

		Assert.Same(rows[2], GetMouseDownElementRow(grid));
		Assert.True(rows[2].ContainsElement(firstClone));
		Assert.True(rows[2].ContainsElement(secondClone));
		Assert.False(rows[0].ContainsElement(firstClone));
		Assert.False(rows[1].ContainsElement(firstClone));
		Assert.False(rows[3].ContainsElement(firstClone));
		Assert.True(rows[0].ContainsElement(original));
	}

	[Fact]
	public void MoveElementsVerticallyToLocation_WhenSelectionSpansRows_ClipsDisplacementToLastValidRow()
	{
		using Grid grid = CreateGrid(out Row[] rows);
		Element anchorClone = new() { Selected = true };
		Element lowerEffect = new() { Selected = true };
		rows[0].AddElement(anchorClone);
		rows[2].AddElement(lowerEffect);
		SetDragContext(grid, [new Element()], rows[0], 0);

		MoveToRow(grid, [anchorClone, lowerEffect], rows[3]);

		Assert.Same(rows[1], GetMouseDownElementRow(grid));
		Assert.Equal(1, GetCurrentRowIndexUnderMouse(grid));
		Assert.True(rows[1].ContainsElement(anchorClone));
		Assert.True(rows[3].ContainsElement(lowerEffect));
	}

	[Fact]
	public void MoveElementsVerticallyToLocation_WhenRepeatedOccurrenceIsHidden_TracksVisibleOccurrence()
	{
		using Grid grid = CreateGrid(out Row[] rows);
		Element original = new();
		Element clone = new() { Selected = true };
		rows[1].AddElement(clone);
		rows[3].AddElement(clone);
		rows[3].Visible = false;
		SetVisibleRowsDirty(grid);
		SetDragContext(grid, [original], rows[1], 1);

		MoveToRow(grid, [clone], rows[2]);

		Assert.Same(rows[2], GetMouseDownElementRow(grid));
		Assert.Equal(2, GetCurrentRowIndexUnderMouse(grid));
		Assert.True(rows[2].ContainsElement(clone));
		Assert.False(rows[3].ContainsElement(clone));
	}

	[Fact]
	public void MoveElementsVerticallyToLocation_WhenDestinationIsDeprecated_LeavesAnchorAndSelectionUnchanged()
	{
		using Grid grid = CreateGrid(out Row[] rows);
		Element original = new();
		Element clone = new() { Selected = true };
		Vixen.Sys.ElementTagDefinition deprecatedDefinition = Vixen.Sys.BuiltInElementTags.CreateDefaults()
			.Single(tag => tag.Key == Vixen.Sys.BuiltInElementTags.DeprecatedKey);
		bool addedDefinition = !Vixen.Sys.VixenSystem.TagDefinitions.Any(tag => tag.Key == deprecatedDefinition.Key);
		if (addedDefinition)
		{
			Vixen.Sys.VixenSystem.TagDefinitions.Add(deprecatedDefinition);
		}

		try
		{
			Vixen.Sys.ElementNode deprecatedTarget = (Vixen.Sys.ElementNode)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(typeof(Vixen.Sys.ElementNode));
			Vixen.Sys.ElementTagCollection tags = new();
			tags.Add(deprecatedDefinition.Id);
			typeof(Vixen.Sys.ElementNode).GetProperty("Tags")!.SetValue(deprecatedTarget, tags);
			rows[0].AddElement(clone);
			rows[1].Tag = deprecatedTarget;
			SetDragContext(grid, [original], rows[0], 0);

			MoveToRow(grid, [clone], rows[1]);

			Assert.Same(rows[0], GetMouseDownElementRow(grid));
			Assert.Equal(0, GetCurrentRowIndexUnderMouse(grid));
			Assert.True(rows[0].ContainsElement(clone));
			Assert.False(rows[1].ContainsElement(clone));
		}
		finally
		{
			if (addedDefinition)
			{
				Vixen.Sys.VixenSystem.TagDefinitions.Remove(deprecatedDefinition);
			}
		}
	}

	[Fact]
	public void MoveElementsVerticallyToLocation_WhenDestinationIsUnchangedOrAtLastVisibleRow_OnlyAdvancesAfterAcceptedMove()
	{
		using Grid grid = CreateGrid(out Row[] rows);
		Element clone = new() { Selected = true };
		rows[0].AddElement(clone);
		SetDragContext(grid, [new Element()], rows[0], 0);

		MoveToRow(grid, [clone], rows[0]);
		Assert.Same(rows[0], GetMouseDownElementRow(grid));

		MoveToRow(grid, [clone], rows[3]);

		Assert.Same(rows[3], GetMouseDownElementRow(grid));
		Assert.True(rows[3].ContainsElement(clone));
		Assert.False(rows[0].ContainsElement(clone));
	}

	[Fact]
	public void MoveElementsVerticallyToLocation_WhenMovingAnExistingEffect_TracksThePointerRow()
	{
		using Grid grid = CreateGrid(out Row[] rows);
		Element element = new() { Selected = true };
		rows[0].AddElement(element);
		SetDragContext(grid, [element], rows[0], 0);

		MoveToRow(grid, [element], rows[1]);
		Assert.Same(rows[1], GetMouseDownElementRow(grid));

		MoveToRow(grid, [element], rows[2]);

		Assert.Same(rows[2], GetMouseDownElementRow(grid));
		Assert.True(rows[2].ContainsElement(element));
	}

	[Fact]
	public void MoveElementsVerticallyToLocation_WhenCloneStartsOnRepeatedOccurrence_TracksUpwardAndDownwardMoves()
	{
		using Grid grid = CreateGrid(out Row[] rows);
		Element original = new();
		Element clone = new() { Selected = true };
		rows[3].AddElement(original);
		rows[1].AddElement(clone);
		rows[3].AddElement(clone);
		SetDragContext(grid, [original], rows[3], 3);

		MoveToRow(grid, [clone], rows[2]);
		Assert.Same(rows[2], GetMouseDownElementRow(grid));

		MoveToRow(grid, [clone], rows[1]);
		Assert.Same(rows[1], GetMouseDownElementRow(grid));

		MoveToRow(grid, [clone], rows[2]);

		Assert.Same(rows[2], GetMouseDownElementRow(grid));
		Assert.True(rows[2].ContainsElement(clone));
		Assert.False(rows[1].ContainsElement(clone));
		Assert.False(rows[3].ContainsElement(clone));
		Assert.True(rows[3].ContainsElement(original));
	}

	private static Grid CreateGrid(out Row[] rows)
	{
		TimeInfo timeInfo = new()
		{
			TimePerPixel = TimeSpan.FromMilliseconds(100),
			TotalTime = TimeSpan.FromSeconds(60)
		};
		Grid grid = new(timeInfo, Guid.NewGuid());
		object sharedTarget = new();
		object[] targets = [new object(), sharedTarget, new object(), sharedTarget];
		rows = targets.Select((target, index) => new Row
		{
			Tag = target,
			Height = 20,
			DisplayTop = index * 20,
			Visible = true
		}).ToArray();
		foreach (Row row in rows)
		{
			grid.Rows.Add(row);
		}
		SetVisibleRowsDirty(grid);

		grid.ElementChangedRows += (_, args) =>
		{
			object oldTarget = args.OldRow.Tag;
			object newTarget = args.NewRow.Tag;
			foreach (Row row in grid.Rows)
			{
				if (ReferenceEquals(row.Tag, oldTarget) && row != args.OldRow)
				{
					row.RemoveElement(args.Element);
				}

				if (ReferenceEquals(row.Tag, newTarget) && row != args.NewRow && !row.ContainsElement(args.Element))
				{
					row.AddElement(args.Element);
				}
			}
		};

		return grid;
	}

	private static void SetVisibleRowsDirty(Grid grid)
	{
		typeof(Grid).GetField("_visibleRowsDirty", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.SetValue(grid, true);
	}

	private static int GetCurrentRowIndexUnderMouse(Grid grid)
	{
		return (int)typeof(Grid).GetProperty("CurrentRowIndexUnderMouse", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.GetValue(grid)!;
	}

	private static void MoveToRow(Grid grid, Element[] elements, Row destinationRow)
	{
		grid.MoveElementsVerticallyToLocation(elements, new Point(0, destinationRow.DisplayTop + 1));
	}

	private static void SetDragContext(Grid grid, List<Element> mouseDownElements, Row mouseDownRow, int currentRowIndex)
	{
		typeof(Grid).GetField("m_mouseDownElements", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.SetValue(grid, mouseDownElements);
		typeof(Grid).GetField("m_mouseDownElementRow", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.SetValue(grid, mouseDownRow);
		typeof(Grid).GetProperty("CurrentRowIndexUnderMouse", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.SetValue(grid, currentRowIndex);
	}

	private static Row GetMouseDownElementRow(Grid grid)
	{
		return (Row)typeof(Grid).GetField("m_mouseDownElementRow", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
			.GetValue(grid)!;
	}
}

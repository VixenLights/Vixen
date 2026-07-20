using Common.Controls.Timeline;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Forms;
using Vixen.Marks;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class RulerLockHeightTests
{
	[Fact]
	public void RulerLockHeight_DefaultsToFalse()
	{
		using Ruler ruler = CreateRuler();

		Assert.False(ruler.LockRulerHeight);
	}

	[Fact]
	public void RulerLockHeight_WhenSet_StoresState()
	{
		using Ruler ruler = CreateRuler();

		ruler.LockRulerHeight = true;

		Assert.True(ruler.LockRulerHeight);
	}

	[Fact]
	public void TimelineControlLockRulerHeight_ForwardsToRuler()
	{
		using TimelineControl timelineControl = new TimelineControl(Guid.NewGuid());

		timelineControl.LockRulerHeight = true;

		Assert.True(timelineControl.LockRulerHeight);
		Assert.True(timelineControl.ruler.LockRulerHeight);
	}

	[Fact]
	public void OnMouseMove_WhenUnlockedAndPointerIsOnBottomEdge_ShowsResizeCursor()
	{
		using Ruler ruler = CreateRuler();

		InvokeMouseMove(ruler, MouseButtons.None, 10, 55);

		Assert.Equal(Cursors.HSplit, ruler.Cursor);
	}

	[Fact]
	public void OnMouseMove_WhenLockedAndPointerIsOnBottomEdge_DoesNotShowResizeCursor()
	{
		using Ruler ruler = CreateRuler();
		ruler.LockRulerHeight = true;

		InvokeMouseMove(ruler, MouseButtons.None, 10, 55);

		Assert.Equal(Cursors.Hand, ruler.Cursor);
	}

	[Fact]
	public void ResizeDrag_WhenLockIsEnabledAfterDragStarts_DoesNotChangeHeight()
	{
		using Ruler ruler = CreateRuler();
		ruler.Cursor = Cursors.HSplit;

		InvokeMouseDown(ruler, MouseButtons.Left, 10, 55);
		ruler.LockRulerHeight = true;
		InvokeMouseMove(ruler, MouseButtons.Left, 10, 80);

		Assert.Equal(60, ruler.Height);
	}

	[Fact]
	public void OnMouseDoubleClick_WhenLocked_DoesNotResetHeight()
	{
		using Ruler ruler = CreateRuler();
		ruler.Height = 75;
		ruler.Cursor = Cursors.HSplit;
		ruler.LockRulerHeight = true;

		InvokeMouseDoubleClick(ruler, 10, 74);

		Assert.Equal(75, ruler.Height);
	}

	private static Ruler CreateRuler()
	{
		TimeInfo timeInfo = new TimeInfo
		{
			TimePerPixel = TimeSpan.FromMilliseconds(100),
			TotalTime = TimeSpan.FromSeconds(60)
		};
		Ruler ruler = new Ruler(timeInfo, Guid.NewGuid())
		{
			Height = 60,
			Width = 100,
			MarkCollections = new ObservableCollection<IMarkCollection>()
		};
		return ruler;
	}

	private static void InvokeMouseDown(Ruler ruler, MouseButtons button, int x, int y)
	{
		InvokeMouseMethod(ruler, "OnMouseDown", new MouseEventArgs(button, 1, x, y, 0));
	}

	private static void InvokeMouseMove(Ruler ruler, MouseButtons button, int x, int y)
	{
		InvokeMouseMethod(ruler, "OnMouseMove", new MouseEventArgs(button, 0, x, y, 0));
	}

	private static void InvokeMouseDoubleClick(Ruler ruler, int x, int y)
	{
		InvokeMouseMethod(ruler, "OnMouseDoubleClick", new MouseEventArgs(MouseButtons.Left, 2, x, y, 0));
	}

	private static void InvokeMouseMethod(Ruler ruler, string methodName, MouseEventArgs mouseEventArgs)
	{
		MethodInfo method = typeof(Ruler).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(ruler, [mouseEventArgs]);
	}
}

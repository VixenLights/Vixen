using Common.Controls.Timeline;
using System.Reflection;
using System.Windows.Forms;
using Xunit;

namespace Vixen.Tests.Sequencer;

[Collection(TimelineControlTestCollection.Name)]
public sealed class WaveformLockHeightTests
{
	[Fact]
	public void WaveformLockHeight_DefaultsToFalse()
	{
		using Waveform waveform = CreateWaveform();

		Assert.False(waveform.LockWaveformHeight);
	}

	[Fact]
	public void WaveformLockHeight_WhenSet_StoresState()
	{
		using Waveform waveform = CreateWaveform();

		waveform.LockWaveformHeight = true;

		Assert.True(waveform.LockWaveformHeight);
	}

	[Fact]
	public void TimelineControlLockWaveformHeight_ForwardsToWaveform()
	{
		using TimelineControl timelineControl = new TimelineControl(Guid.NewGuid());

		timelineControl.LockWaveformHeight = true;

		Assert.True(timelineControl.LockWaveformHeight);
		Assert.True(timelineControl.waveform.LockWaveformHeight);
	}

	[Fact]
	public void OnMouseMove_WhenUnlockedAndPointerIsOnBottomEdge_ShowsResizeCursor()
	{
		using Waveform waveform = CreateWaveform();

		InvokeMouseMove(waveform, MouseButtons.None, 10, 55);

		Assert.Equal(Cursors.HSplit, waveform.Cursor);
	}

	[Fact]
	public void OnMouseMove_WhenLockedAndPointerIsOnBottomEdge_DoesNotShowResizeCursor()
	{
		using Waveform waveform = CreateWaveform();
		waveform.LockWaveformHeight = true;

		InvokeMouseMove(waveform, MouseButtons.None, 10, 55);

		Assert.Equal(Cursors.Hand, waveform.Cursor);
	}

	[Fact]
	public void ResizeDrag_WhenLockIsEnabledAfterDragStarts_DoesNotChangeHeight()
	{
		using Waveform waveform = CreateWaveform();
		waveform.Cursor = Cursors.HSplit;
		waveform.LockWaveformHeight = true;

		InvokeMouseMove(waveform, MouseButtons.Left, 10, 80);

		Assert.Equal(60, waveform.Height);
	}

	[Fact]
	public void OnMouseDoubleClick_WhenLocked_DoesNotResetHeight()
	{
		using Waveform waveform = CreateWaveform();
		waveform.Height = 75;
		waveform.Cursor = Cursors.HSplit;
		waveform.LockWaveformHeight = true;

		InvokeMouseDoubleClick(waveform, 10, 74);

		Assert.Equal(75, waveform.Height);
	}

	private static Waveform CreateWaveform()
	{
		TimeInfo timeInfo = new TimeInfo
		{
			TimePerPixel = TimeSpan.FromMilliseconds(100),
			TotalTime = TimeSpan.FromSeconds(60)
		};
		Waveform waveform = new Waveform(timeInfo, Guid.NewGuid())
		{
			Height = 60,
			Width = 100
		};
		return waveform;
	}

	private static void InvokeMouseMove(Waveform waveform, MouseButtons button, int x, int y)
	{
		InvokeMouseMethod(waveform, "OnMouseMove", new MouseEventArgs(button, 0, x, y, 0));
	}

	private static void InvokeMouseDoubleClick(Waveform waveform, int x, int y)
	{
		InvokeMouseMethod(waveform, "OnMouseDoubleClick", new MouseEventArgs(MouseButtons.Left, 2, x, y, 0));
	}

	private static void InvokeMouseMethod(Waveform waveform, string methodName, MouseEventArgs mouseEventArgs)
	{
		MethodInfo method = typeof(Waveform).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(waveform, [mouseEventArgs]);
	}
}

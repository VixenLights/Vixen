using Common.Controls.Timeline;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class RowEventScopeTests
{
	[Fact]
	public void RowToggled_WhenDifferentRowToggled_DoesNotNotifyOtherRow()
	{
		Row changedRow = new Row();
		Row otherRow = new Row();
		int changedRowNotifications = 0;
		int otherRowNotifications = 0;
		changedRow.RowToggled += (_, _) => changedRowNotifications++;
		otherRow.RowToggled += (_, _) => otherRowNotifications++;

		changedRow.TreeOpen = !changedRow.TreeOpen;

		Assert.Equal(1, changedRowNotifications);
		Assert.Equal(0, otherRowNotifications);
	}

	[Fact]
	public void RowVisibilityChanged_WhenDifferentRowVisibilityChanges_DoesNotNotifyOtherRow()
	{
		Row changedRow = new Row();
		Row otherRow = new Row();
		int changedRowNotifications = 0;
		int otherRowNotifications = 0;
		changedRow.RowVisibilityChanged += (_, _) => changedRowNotifications++;
		otherRow.RowVisibilityChanged += (_, _) => otherRowNotifications++;

		changedRow.Visible = true;

		Assert.Equal(1, changedRowNotifications);
		Assert.Equal(0, otherRowNotifications);
	}

	[Fact]
	public void RowChanged_WhenDifferentRowChanges_DoesNotNotifyOtherRow()
	{
		Row changedRow = new Row();
		Row otherRow = new Row();
		int changedRowNotifications = 0;
		int otherRowNotifications = 0;
		changedRow.RowChanged += (_, _) => changedRowNotifications++;
		otherRow.RowChanged += (_, _) => otherRowNotifications++;

		changedRow.Selected = true;

		Assert.Equal(1, changedRowNotifications);
		Assert.Equal(0, otherRowNotifications);
	}
}

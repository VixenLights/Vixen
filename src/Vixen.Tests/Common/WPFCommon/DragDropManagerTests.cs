using System.Windows.Input;
using Common.WPFCommon.Input;
using Xunit;

namespace Vixen.Tests.Common.WPFCommon;

public sealed class DragDropManagerTests
{
	[Fact]
	public void ShouldStartDrag_WhenMouseIsDownAndButtonIsPressedAndMovementIsDragGesture_ReturnsTrue()
	{
		var result = DragDropManager.ShouldStartDrag(true, MouseButtonState.Pressed, true);

		Assert.True(result);
	}

	[Theory]
	[InlineData(false, MouseButtonState.Pressed, true)]
	[InlineData(true, MouseButtonState.Released, true)]
	[InlineData(true, MouseButtonState.Pressed, false)]
	public void ShouldStartDrag_WhenAnyRequiredConditionIsMissing_ReturnsFalse(
		bool isMouseDown,
		MouseButtonState leftButtonState,
		bool isDragGesture)
	{
		var result = DragDropManager.ShouldStartDrag(isMouseDown, leftButtonState, isDragGesture);

		Assert.False(result);
	}
}

using System.Windows;
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

	[Fact]
	public void ResolveDropEffect_WhenDataIsInvalid_ReturnsNone()
	{
		var result = DragDropManager.ResolveDropEffect(
			false,
			DragDropEffects.Copy | DragDropEffects.Move,
			DragDropEffects.Copy | DragDropEffects.Move,
			DragDropKeyStates.ControlKey);

		Assert.Equal(DragDropEffects.None, result);
	}

	[Fact]
	public void ResolveDropEffect_WhenMoveOnlySourceTargetsCopyOnly_ReturnsNone()
	{
		var result = DragDropManager.ResolveDropEffect(
			true,
			DragDropEffects.Move,
			DragDropEffects.Copy,
			DragDropKeyStates.None);

		Assert.Equal(DragDropEffects.None, result);
	}

	[Theory]
	[InlineData(DragDropKeyStates.None)]
	[InlineData(DragDropKeyStates.ControlKey)]
	public void ResolveDropEffect_WhenCopyAndMoveSourceTargetsCopyOnly_ReturnsCopy(DragDropKeyStates keyStates)
	{
		var result = DragDropManager.ResolveDropEffect(
			true,
			DragDropEffects.Copy | DragDropEffects.Move,
			DragDropEffects.Copy,
			keyStates);

		Assert.Equal(DragDropEffects.Copy, result);
	}

	[Fact]
	public void ResolveDropEffect_WhenCopyAndMoveSourceTargetsMoveOnly_ReturnsMove()
	{
		var result = DragDropManager.ResolveDropEffect(
			true,
			DragDropEffects.Copy | DragDropEffects.Move,
			DragDropEffects.Move,
			DragDropKeyStates.ControlKey);

		Assert.Equal(DragDropEffects.Move, result);
	}

	[Fact]
	public void ResolveDropEffect_WhenBothEffectsAreAcceptedAndControlIsPressed_ReturnsCopy()
	{
		var result = DragDropManager.ResolveDropEffect(
			true,
			DragDropEffects.Copy | DragDropEffects.Move,
			DragDropEffects.Copy | DragDropEffects.Move,
			DragDropKeyStates.ControlKey | DragDropKeyStates.ShiftKey);

		Assert.Equal(DragDropEffects.Copy, result);
	}

	[Fact]
	public void ResolveDropEffect_WhenBothEffectsAreAcceptedAndControlIsNotPressed_ReturnsMove()
	{
		var result = DragDropManager.ResolveDropEffect(
			true,
			DragDropEffects.Copy | DragDropEffects.Move,
			DragDropEffects.Copy | DragDropEffects.Move,
			DragDropKeyStates.None);

		Assert.Equal(DragDropEffects.Move, result);
	}
}

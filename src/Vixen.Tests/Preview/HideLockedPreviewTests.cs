using VixenModules.Preview.VixenPreview;
using Xunit;

namespace Vixen.Tests.Preview
{
	public class HideLockedPreviewTests
	{
		[Theory]
		[InlineData(false, false, true)]
		[InlineData(false, true, true)]
		[InlineData(true, false, true)]
		[InlineData(true, true, false)]
		public void IsDisplayItemVisible_ReturnsExpectedVisibility(bool hideLockedDisplayItems, bool displayItemLocked, bool expected)
		{
			var result = PreviewCanvasVisibility.IsDisplayItemVisible(hideLockedDisplayItems, displayItemLocked);

			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData(false, false, false)]
		[InlineData(false, true, true)]
		[InlineData(true, false, true)]
		[InlineData(true, true, true)]
		public void IsHideLockedCommandEnabled_ReturnsExpectedState(bool hideLockedDisplayItems, bool anyLockedDisplayItems, bool expected)
		{
			var result = PreviewCanvasVisibility.IsHideLockedCommandEnabled(hideLockedDisplayItems, anyLockedDisplayItems);

			Assert.Equal(expected, result);
		}

		[Theory]
		[InlineData(false, false, true)]
		[InlineData(false, true, true)]
		[InlineData(true, false, true)]
		[InlineData(true, true, false)]
		public void ShouldRetainSelection_ReturnsExpectedState(bool hideLockedDisplayItems, bool displayItemLocked, bool expected)
		{
			var result = PreviewCanvasVisibility.ShouldRetainSelection(hideLockedDisplayItems, displayItemLocked);

			Assert.Equal(expected, result);
		}
	}
}

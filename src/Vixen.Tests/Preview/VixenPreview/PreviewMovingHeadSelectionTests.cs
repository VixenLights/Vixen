using System.Drawing;
using VixenModules.Preview.VixenPreview.Shapes;
using Xunit;

namespace Vixen.Tests.Preview.VixenPreview
{
	/// <summary>
	/// Verifies marquee selection behavior for moving-head preview fixtures.
	/// </summary>
	public sealed class PreviewMovingHeadSelectionTests
	{
		/// <summary>
		/// Verifies that a marquee containing a displayed fixture selects it at each supported zoom level.
		/// </summary>
		/// <param name="zoomLevel">A preview scale factor.</param>
		[Theory]
		[InlineData(0.5)]
		[InlineData(1.0)]
		[InlineData(2.0)]
		[InlineData(4.0)]
		public void ShapeInRect_WhenMarqueeContainsDisplayedFixture_ReturnsTrue(double zoomLevel)
		{
			var shape = CreateShape(zoomLevel);
			var displayedBounds = GetDisplayedBounds(zoomLevel);
			var marquee = CreateRectangle(
				displayedBounds.Left - 5,
				displayedBounds.Top - 5,
				displayedBounds.Right + 5,
				displayedBounds.Bottom + 5);

			var result = shape.ShapeInRect(marquee, allIn: true);

			Assert.True(result);
		}

		/// <summary>
		/// Verifies that an intersection marquee selects a fixture when it crosses an edge without containing a corner.
		/// </summary>
		/// <param name="zoomLevel">A preview scale factor.</param>
		[Theory]
		[InlineData(0.5)]
		[InlineData(1.0)]
		[InlineData(2.0)]
		[InlineData(4.0)]
		public void ShapeInRect_WhenIntersectionMarqueeCrossesFixtureEdge_ReturnsTrue(double zoomLevel)
		{
			var shape = CreateShape(zoomLevel);
			var displayedBounds = GetDisplayedBounds(zoomLevel);
			var marquee = CreateRectangle(
				displayedBounds.Right - 5,
				displayedBounds.Top + 5,
				displayedBounds.Right + 5,
				displayedBounds.Bottom - 5);

			var result = shape.ShapeInRect(marquee, allIn: false);

			Assert.True(result);
		}

		/// <summary>
		/// Verifies that a full-containment marquee rejects a fixture that only partially overlaps it.
		/// </summary>
		/// <param name="zoomLevel">A preview scale factor.</param>
		[Theory]
		[InlineData(0.5)]
		[InlineData(1.0)]
		[InlineData(2.0)]
		[InlineData(4.0)]
		public void ShapeInRect_WhenContainmentMarqueeOnlyPartiallyOverlapsFixture_ReturnsFalse(double zoomLevel)
		{
			var shape = CreateShape(zoomLevel);
			var displayedBounds = GetDisplayedBounds(zoomLevel);
			var marquee = CreateRectangle(
				displayedBounds.Right - 5,
				displayedBounds.Top + 5,
				displayedBounds.Right + 5,
				displayedBounds.Bottom - 5);

			var result = shape.ShapeInRect(marquee, allIn: true);

			Assert.False(result);
		}

		/// <summary>
		/// Verifies that an intersection marquee selects a fixture when the marquee is completely inside the fixture.
		/// </summary>
		/// <param name="zoomLevel">A preview scale factor.</param>
		[Theory]
		[InlineData(0.5)]
		[InlineData(1.0)]
		[InlineData(2.0)]
		[InlineData(4.0)]
		public void ShapeInRect_WhenIntersectionMarqueeIsInsideFixture_ReturnsTrue(double zoomLevel)
		{
			var shape = CreateShape(zoomLevel);
			var displayedBounds = GetDisplayedBounds(zoomLevel);
			var marquee = CreateRectangle(
				displayedBounds.Left + 5,
				displayedBounds.Top + 5,
				displayedBounds.Right - 5,
				displayedBounds.Bottom - 5);

			var result = shape.ShapeInRect(marquee, allIn: false);

			Assert.True(result);
		}

		/// <summary>
		/// Verifies that shared rectangle edges count as an intersection.
		/// </summary>
		/// <param name="zoomLevel">A preview scale factor.</param>
		[Theory]
		[InlineData(0.5)]
		[InlineData(1.0)]
		[InlineData(2.0)]
		[InlineData(4.0)]
		public void ShapeInRect_WhenIntersectionMarqueeTouchesFixtureEdge_ReturnsTrue(double zoomLevel)
		{
			var shape = CreateShape(zoomLevel);
			var displayedBounds = GetDisplayedBounds(zoomLevel);
			var marquee = CreateRectangle(
				displayedBounds.Right,
				displayedBounds.Top + 5,
				displayedBounds.Right + 5,
				displayedBounds.Bottom - 5);

			var result = shape.ShapeInRect(marquee, allIn: false);

			Assert.True(result);
		}

		/// <summary>
		/// Verifies that a marquee with no intersection does not select a fixture.
		/// </summary>
		/// <param name="zoomLevel">A preview scale factor.</param>
		/// <param name="allIn"><see langword="true" /> to request full containment; otherwise, <see langword="false" />.</param>
		[Theory]
		[InlineData(0.5, false)]
		[InlineData(1.0, false)]
		[InlineData(2.0, false)]
		[InlineData(4.0, false)]
		[InlineData(0.5, true)]
		[InlineData(1.0, true)]
		[InlineData(2.0, true)]
		[InlineData(4.0, true)]
		public void ShapeInRect_WhenMarqueeDoesNotOverlapFixture_ReturnsFalse(double zoomLevel, bool allIn)
		{
			var shape = CreateShape(zoomLevel);
			var displayedBounds = GetDisplayedBounds(zoomLevel);
			var marquee = CreateRectangle(
				displayedBounds.Right + 10,
				displayedBounds.Bottom + 10,
				displayedBounds.Right + 20,
				displayedBounds.Bottom + 20);

			var result = shape.ShapeInRect(marquee, allIn);

			Assert.False(result);
		}

		/// <summary>
		/// Verifies that a marquee at the old unscaled location does not select a fixture displayed at a scaled location.
		/// </summary>
		/// <param name="zoomLevel">A preview scale factor greater than one.</param>
		[Theory]
		[InlineData(2.0)]
		[InlineData(4.0)]
		public void ShapeInRect_WhenMarqueeUsesObsoleteUnscaledLocation_ReturnsFalse(double zoomLevel)
		{
			var shape = CreateShape(zoomLevel);
			var marquee = CreateRectangle(95, 95, 145, 145);

			var result = shape.ShapeInRect(marquee, allIn: false);

			Assert.False(result);
		}

		private static PreviewMovingHead CreateShape(double zoomLevel)
		{
			var shape = new PreviewMovingHead(new PreviewPoint(0, 0), null, zoomLevel)
			{
				TopLeftPoint = new Point(100, 100),
				TopRightPoint = new Point(140, 100),
				BottomLeftPoint = new Point(100, 140),
				BottomRightPoint = new Point(140, 140)
			};

			return shape;
		}

		private static Rectangle GetDisplayedBounds(double zoomLevel)
		{
			return CreateRectangle(
				(int)(100 * zoomLevel),
				(int)(100 * zoomLevel),
				(int)(140 * zoomLevel),
				(int)(140 * zoomLevel));
		}

		private static Rectangle CreateRectangle(int left, int top, int right, int bottom)
		{
			return new Rectangle(left, top, right - left, bottom - top);
		}
	}
}

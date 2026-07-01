using System.ComponentModel;
using System.Reflection;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.Effect.Spiral;
using Xunit;

namespace Vixen.Tests.Effects;

public class SpiralLocationRenderTests
{
	[Fact]
	public void Spiral_DefaultConstructor_EnablesTargetPositioning()
	{
		// Arrange
		var effect = new Spiral();

		// Act
		var property = TypeDescriptor.GetProperties(effect)[nameof(PixelEffectBase.TargetPositioning)];

		// Assert
		Assert.True(property?.IsBrowsable);
	}

	[Fact]
	public void Spiral_RenderEffectByLocation_DoesNotThrow()
	{
		// Arrange
		var effect = new Spiral
		{
			TargetPositioning = TargetPositioningType.Locations
		};
		var frameBuffer = new PixelLocationFrameBuffer([], 1);
		var renderByLocation = typeof(Spiral).GetMethod(
			"RenderEffectByLocation",
			BindingFlags.Instance | BindingFlags.NonPublic);

		// Act
		var exception = Record.Exception(() => renderByLocation?.Invoke(effect, [1, frameBuffer]));

		// Assert
		Assert.Null(exception);
	}
}

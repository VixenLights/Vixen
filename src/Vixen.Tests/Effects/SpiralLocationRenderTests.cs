using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Moq;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.Effect.Spiral;
using Xunit;

namespace Vixen.Tests.Effects;

public class SpiralLocationRenderTests
{
	private const int DefaultWidth = 8;
	private const int DefaultHeight = 5;

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
		var effect = CreateDeterministicSpiral();
		var frameBuffer = new PixelLocationFrameBuffer([], 1);
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(effect);

		// Act
		var exception = Record.Exception(() => InvokeRenderByLocation(effect, 1, frameBuffer));

		// Assert
		Assert.Null(exception);
	}

	[Fact]
	public void Spiral_LocationRender_RectangularGridMatchesStringRender()
	{
		// Arrange
		var stringEffect = CreateDeterministicSpiral();
		var locationEffect = CreateDeterministicSpiral();
		SetVirtualBuffer(stringEffect, DefaultWidth, DefaultHeight);
		SetVirtualBuffer(locationEffect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(stringEffect);
		InvokeSetupRender(locationEffect);
		var stringBuffer = new PixelFrameBuffer(DefaultWidth, DefaultHeight);
		var locationBuffer = CreateLocationBuffer(CreateGridLocations(DefaultWidth, DefaultHeight), 1);

		// Act
		InvokeRenderEffect(stringEffect, 0, stringBuffer);
		InvokeRenderByLocation(locationEffect, 1, locationBuffer);

		// Assert
		foreach (var location in locationBuffer.ElementLocations)
		{
			var expectedLocalY = DefaultHeight - 1 - location.Y;
			AssertSameRgb(stringBuffer.GetColorAt(location.X, expectedLocalY), locationBuffer.GetColorAt(location.X, location.Y));
		}
	}

	[Fact]
	public void Spiral_LocationRender_SparseCoordinatesSampleVirtualRectangle()
	{
		// Arrange
		const int xOffset = 10;
		const int yOffset = 20;
		var locations = new List<ElementLocation>
		{
			CreateElementLocation(xOffset, yOffset),
			CreateElementLocation(xOffset + 3, yOffset + 1),
			CreateElementLocation(xOffset + 7, yOffset + 4)
		};
		var effect = CreateDeterministicSpiral();
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight, xOffset, yOffset);
		InvokeSetupRender(effect);
		var locationBuffer = CreateLocationBuffer(locations, 1);

		// Act
		InvokeRenderByLocation(effect, 1, locationBuffer);

		// Assert
		Assert.Equal(locations.Count, locationBuffer.ElementLocations.Count());
		Assert.Contains(locations, location => locationBuffer.GetColorAt(location.X, location.Y).ToArgb() != Color.Black.ToArgb());
	}

	[Fact]
	public void Spiral_LocationRender_SupportsMovementAndLevelControls()
	{
		// Arrange
		var locations = CreateGridLocations(DefaultWidth, DefaultHeight);
		var movingEffect = CreateDeterministicSpiral(100, SpiralDirection.Forward, 4);
		SetVirtualBuffer(movingEffect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(movingEffect);
		var movingBuffer = CreateLocationBuffer(locations, 2);

		var fullLevelEffect = CreateDeterministicSpiral(100);
		var dimLevelEffect = CreateDeterministicSpiral(50);
		SetVirtualBuffer(fullLevelEffect, DefaultWidth, DefaultHeight);
		SetVirtualBuffer(dimLevelEffect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(fullLevelEffect);
		InvokeSetupRender(dimLevelEffect);
		var fullLevelBuffer = CreateLocationBuffer(locations, 1);
		var dimLevelBuffer = CreateLocationBuffer(locations, 1);

		// Act
		InvokeRenderByLocation(movingEffect, 2, movingBuffer);
		InvokeRenderByLocation(fullLevelEffect, 1, fullLevelBuffer);
		InvokeRenderByLocation(dimLevelEffect, 1, dimLevelBuffer);

		// Assert
		Assert.Contains(locations, location => movingBuffer.GetFrameDataAt(location.X, location.Y)[0].FullColor != movingBuffer.GetFrameDataAt(location.X, location.Y)[1].FullColor);
		var litLocation = locations.First(location => fullLevelBuffer.GetColorAt(location.X, location.Y).ToArgb() != Color.Black.ToArgb());
		Assert.True(dimLevelBuffer.GetColorAt(litLocation.X, litLocation.Y).GetBrightness() < fullLevelBuffer.GetColorAt(litLocation.X, litLocation.Y).GetBrightness());
	}

	[Fact]
	public void Spiral_LocationRender_EmptyColorsDoesNotThrow()
	{
		// Arrange
		var locations = CreateGridLocations(DefaultWidth, DefaultHeight);
		var effect = CreateDeterministicSpiral();
		effect.Colors = [];
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(effect);
		var locationBuffer = CreateLocationBuffer(locations, 1);

		// Act
		var exception = Record.Exception(() => InvokeRenderByLocation(effect, 1, locationBuffer));

		// Assert
		Assert.Null(exception);
		Assert.All(locations, location => Assert.Equal(Color.Black.ToArgb(), locationBuffer.GetColorAt(location.X, location.Y).ToArgb()));
	}

	private static Spiral CreateDeterministicSpiral(int level = 100, SpiralDirection direction = SpiralDirection.None, int speed = 1)
	{
		return new Spiral
		{
			TargetPositioning = TargetPositioningType.Locations,
			TimeSpan = TimeSpan.FromMilliseconds(1000),
			Colors = [new ColorGradient(Color.Red), new ColorGradient(Color.Lime)],
			Direction = direction,
			MovementType = MovementType.Iterations,
			Speed = speed,
			Repeat = 1,
			Blend = false,
			Show3D = false,
			Grow = false,
			Shrink = false,
			LevelCurve = new Curve(level),
			ThicknessCurve = new Curve(60),
			RotationCurve = new Curve(57)
		};
	}

	private static List<ElementLocation> CreateGridLocations(int width, int height, int xOffset = 0, int yOffset = 0)
	{
		var locations = new List<ElementLocation>();
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				locations.Add(CreateElementLocation(x + xOffset, y + yOffset));
			}
		}

		return locations;
	}

	private static ElementLocation CreateElementLocation(int x, int y)
	{
		var node = new Mock<IElementNode>();
		node.SetupGet(elementNode => elementNode.Properties).Returns(new PropertyManager(node.Object));
		return new ElementLocation(node.Object)
		{
			X = x,
			Y = y
		};
	}

	private static PixelLocationFrameBuffer CreateLocationBuffer(List<ElementLocation> locations, int numFrames)
	{
		return new PixelLocationFrameBuffer(locations, numFrames);
	}

	private static void SetVirtualBuffer(Spiral effect, int width, int height, int xOffset = 0, int yOffset = 0)
	{
		SetPixelEffectBaseField(effect, "_bufferHt", width);
		SetPixelEffectBaseField(effect, "_bufferWi", height);
		SetPixelEffectBaseField(effect, "_bufferHtOffset", xOffset);
		SetPixelEffectBaseField(effect, "_bufferWiOffset", yOffset);
	}

	private static void SetPixelEffectBaseField(Spiral effect, string fieldName, int value)
	{
		var field = typeof(PixelEffectBase).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(field);
		field.SetValue(effect, value);
	}

	private static void InvokeSetupRender(Spiral effect)
	{
		var setupRender = typeof(Spiral).GetMethod("SetupRender", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(setupRender);
		setupRender.Invoke(effect, []);
	}

	private static void InvokeRenderEffect(Spiral effect, int frame, IPixelFrameBuffer frameBuffer)
	{
		var renderEffect = typeof(Spiral).GetMethod(
			"RenderEffect",
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(int), typeof(IPixelFrameBuffer)],
			null);
		Assert.NotNull(renderEffect);
		renderEffect.Invoke(effect, [frame, frameBuffer]);
	}

	private static void InvokeRenderByLocation(Spiral effect, int numFrames, PixelLocationFrameBuffer frameBuffer)
	{
		var renderByLocation = typeof(Spiral).GetMethod(
			"RenderEffectByLocation",
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(int), typeof(PixelLocationFrameBuffer)],
			null);
		Assert.NotNull(renderByLocation);
		renderByLocation.Invoke(effect, [numFrames, frameBuffer]);
	}

	private static void AssertSameRgb(Color expected, Color actual)
	{
		Assert.Equal(expected.R, actual.R);
		Assert.Equal(expected.G, actual.G);
		Assert.Equal(expected.B, actual.B);
	}
}

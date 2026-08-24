using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using Moq;
using Vixen.Sys;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.Effect.Fire;
using Xunit;

namespace Vixen.Tests.Effects;

/// <summary>
/// Characterizes Fire's current target-positioning and dense string-rendering behavior.
/// </summary>
public sealed class FireLocationRenderTests
{
	private const int DefaultWidth = 4;
	private const int DefaultHeight = 3;

	/// <summary>
	/// Verifies that Fire exposes the inherited target-positioning setup property.
	/// </summary>
	[Fact]
	public void Fire_DefaultConstructor_EnablesTargetPositioning()
	{
		// Arrange
		var effect = new Fire();

		// Act
		var property = TypeDescriptor.GetProperties(effect)[nameof(PixelEffectBase.TargetPositioning)];

		// Assert
		Assert.True(property?.IsBrowsable);
		Assert.Equal(TargetPositioningType.Strings, effect.TargetPositioning);
	}

	/// <summary>
	/// Verifies that switching target positioning updates the StringOrientation property visibility.
	/// </summary>
	[Fact]
	public void Fire_TargetPositioning_TogglesStringOrientationVisibility()
	{
		// Arrange
		var effect = new Fire();

		// Act and Assert
		Assert.True(GetProperty(effect, nameof(PixelEffectBase.StringOrientation)).IsBrowsable);
		effect.TargetPositioning = TargetPositioningType.Locations;
		Assert.False(GetProperty(effect, nameof(PixelEffectBase.StringOrientation)).IsBrowsable);
		effect.TargetPositioning = TargetPositioningType.Strings;
		Assert.True(GetProperty(effect, nameof(PixelEffectBase.StringOrientation)).IsBrowsable);
	}

	/// <summary>
	/// Verifies that replacing Fire data configured for locations refreshes StringOrientation visibility.
	/// </summary>
	[Fact]
	public void Fire_ModuleData_PreservesLocationAttributeState()
	{
		// Arrange
		var effect = new Fire();
		var data = new FireData
		{
			TargetPositioning = TargetPositioningType.Locations
		};

		// Act
		effect.ModuleData = data;

		// Assert
		Assert.True(GetProperty(effect, nameof(PixelEffectBase.TargetPositioning)).IsBrowsable);
		Assert.False(GetProperty(effect, nameof(PixelEffectBase.StringOrientation)).IsBrowsable);
	}

	/// <summary>
	/// Verifies that Fire implements location rendering without throwing.
	/// </summary>
	[Fact]
	public void Fire_RenderEffectByLocation_DoesNotThrow()
	{
		// Arrange
		var effect = new Fire();
		var locations = CreateGridLocations(DefaultWidth, DefaultHeight, 10, 20);
		var frameBuffer = new PixelLocationFrameBuffer(locations, 1);
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight, 10, 20);
		InvokeSetupRender(effect);

		// Act
		var exception = Record.Exception(() => InvokeRenderByLocation(effect, 1, frameBuffer));

		// Assert
		Assert.Null(exception);
	}

	/// <summary>
	/// Verifies that location rendering samples its generated dense heat field for every direction.
	/// </summary>
	/// <param name="direction">The selected Fire source edge.</param>
	[Theory]
	[InlineData(FireDirection.Bottom)]
	[InlineData(FireDirection.Top)]
	[InlineData(FireDirection.Left)]
	[InlineData(FireDirection.Right)]
	public void Fire_LocationProjection_RectangularGridSamplesGeneratedHeatField(FireDirection direction)
	{
		// Arrange
		const int xOffset = 10;
		const int yOffset = 20;
		var effect = new Fire
		{
			Location = direction,
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		var locations = CreateGridLocations(DefaultWidth, DefaultHeight, xOffset, yOffset);
		var frameBuffer = new PixelLocationFrameBuffer(locations, 1);
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight, xOffset, yOffset);
		InvokeSetupRender(effect);

		// Act
		InvokeRenderByLocation(effect, 1, frameBuffer);
		var heat = GetFireBuffer(effect);

		// Assert
		foreach (var location in frameBuffer.ElementLocations)
		{
			var outputX = location.X - xOffset;
			var outputY = yOffset + DefaultHeight - 1 - location.Y;
			var (simulationX, simulationY) = GetSimulationCoordinate(direction, outputX, outputY, DefaultWidth, DefaultHeight);
			var simulationWidth = direction is FireDirection.Left or FireDirection.Right ? DefaultHeight : DefaultWidth;
			var colorIndex = heat[simulationY * simulationWidth + simulationX];
			AssertSameRgb(FirePalette.GetColor(colorIndex).ToRGB(), frameBuffer.GetColorAt(location.X, location.Y));
		}
	}

	/// <summary>
	/// Verifies that location rendering projects the generated source row to each selected origin edge.
	/// </summary>
	/// <param name="direction">The selected Fire source edge.</param>
	[Theory]
	[InlineData(FireDirection.Bottom)]
	[InlineData(FireDirection.Top)]
	[InlineData(FireDirection.Left)]
	[InlineData(FireDirection.Right)]
	public void Fire_LocationRender_OriginatesAtSelectedEdge(FireDirection direction)
	{
		// Arrange
		const int xOffset = 10;
		const int yOffset = 20;
		var effect = new Fire { Location = direction, TimeSpan = TimeSpan.FromMilliseconds(1000) };
		var locations = CreateGridLocations(DefaultWidth, DefaultHeight, xOffset, yOffset);
		var frameBuffer = new PixelLocationFrameBuffer(locations, 1);
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight, xOffset, yOffset);
		InvokeSetupRender(effect);

		// Act
		InvokeRenderByLocation(effect, 1, frameBuffer);
		var heat = GetFireBuffer(effect);

		// Assert
		var simulationWidth = direction is FireDirection.Left or FireDirection.Right ? DefaultHeight : DefaultWidth;
		var simulationHeight = direction is FireDirection.Left or FireDirection.Right ? DefaultWidth : DefaultHeight;
		for (var x = 0; x < simulationWidth; x++)
		{
			var (outputX, outputY) = GetSourceOutputCoordinate(direction, x, simulationHeight);
			var absoluteX = xOffset + outputX;
			var absoluteY = yOffset + DefaultHeight - outputY - 1;
			AssertSameRgb(FirePalette.GetColor(heat[x]).ToRGB(), frameBuffer.GetColorAt(absoluteX, absoluteY));
		}
	}

	/// <summary>
	/// Verifies that sparse preview locations retain their absolute keys while sampling the complete virtual heat field.
	/// </summary>
	[Fact]
	public void Fire_LocationRender_SparseCoordinatesSampleDenseHeatField()
	{
		// Arrange
		const int width = 8;
		const int height = 5;
		const int xOffset = 10;
		const int yOffset = 20;
		var locations = new List<ElementLocation>
		{
			CreateElementLocation(xOffset, yOffset),
			CreateElementLocation(xOffset + 3, yOffset + 2),
			CreateElementLocation(xOffset + width - 1, yOffset + height - 1)
		};
		var effect = new Fire { TimeSpan = TimeSpan.FromMilliseconds(1000) };
		var frameBuffer = new PixelLocationFrameBuffer(locations, 1);
		SetVirtualBuffer(effect, width, height, xOffset, yOffset);
		InvokeSetupRender(effect);

		// Act
		InvokeRenderByLocation(effect, 1, frameBuffer);
		var heat = GetFireBuffer(effect);

		// Assert
		Assert.Equal(locations.Count, frameBuffer.ElementLocations.Count());
		var target = locations[1];
		var outputX = target.X - xOffset;
		var outputY = yOffset + height - 1 - target.Y;
		AssertSameRgb(FirePalette.GetColor(heat[outputY * width + outputX]).ToRGB(), frameBuffer.GetColorAt(target.X, target.Y));
		Assert.ThrowsAny<Exception>(() => frameBuffer.GetColorAt(xOffset + 1, yOffset + 1));
	}

	/// <summary>
	/// Verifies that location rendering applies level controls and retains generated heat for height-controlled frames.
	/// </summary>
	[Fact]
	public void Fire_LocationRender_AppliesHeightHueAndLevel()
	{
		// Arrange
		var locations = CreateGridLocations(DefaultWidth, DefaultHeight);
		var effect = new Fire
		{
			Height = new Curve(0d),
			HueShiftCurve = new Curve(50),
			LevelCurve = new Curve(50),
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		var frameBuffer = new PixelLocationFrameBuffer(locations, 1);
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(effect);

		// Act
		InvokeRenderByLocation(effect, 1, frameBuffer);
		var heat = GetFireBuffer(effect);
		var expected = FirePalette.GetColor(heat[0]);
		expected.H += 0.5f;
		expected.V *= 0.5;

		// Assert
		Assert.Equal(255 * 100 / DefaultHeight, GetFrameStep(effect, 0, DefaultHeight));
		AssertSameRgb(expected.ToRGB(), frameBuffer.GetColorAt(0, DefaultHeight - 1));
	}

	/// <summary>
	/// Verifies that location rendering supports multiple frames, narrow virtual rectangles, and duplicate locations.
	/// </summary>
	/// <param name="direction">The selected Fire source edge.</param>
	/// <param name="width">The virtual rectangle width.</param>
	/// <param name="height">The virtual rectangle height.</param>
	[Theory]
	[InlineData(FireDirection.Bottom, 1, 4)]
	[InlineData(FireDirection.Top, 1, 4)]
	[InlineData(FireDirection.Left, 4, 1)]
	[InlineData(FireDirection.Right, 4, 1)]
	public void Fire_LocationRender_MultipleFramesSupportNarrowBuffersAndDuplicateLocations(FireDirection direction, int width, int height)
	{
		// Arrange
		var location = CreateElementLocation(10, 20);
		var locations = new List<ElementLocation> { location, location };
		var effect = new Fire { Location = direction, TimeSpan = TimeSpan.FromMilliseconds(1000) };
		var frameBuffer = new PixelLocationFrameBuffer(locations, 2);
		SetVirtualBuffer(effect, width, height, 10, 20);
		InvokeSetupRender(effect);

		// Act
		var exception = Record.Exception(() => InvokeRenderByLocation(effect, 2, frameBuffer));

		// Assert
		Assert.Null(exception);
		Assert.Single(frameBuffer.ElementLocations);
		Assert.Equal(2, frameBuffer.GetFrameDataAt(location.X, location.Y).Length);
	}

	/// <summary>
	/// Verifies that each direction projects the generated source row to its selected edge.
	/// </summary>
	/// <param name="direction">The selected Fire source edge.</param>
	[Theory]
	[InlineData(FireDirection.Bottom)]
	[InlineData(FireDirection.Top)]
	[InlineData(FireDirection.Left)]
	[InlineData(FireDirection.Right)]
	public void Fire_StringRender_ProjectsGeneratedSourceRowAtSelectedEdge(FireDirection direction)
	{
		// Arrange
		var effect = new Fire
		{
			Location = direction,
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(effect);
		var frameBuffer = new PixelFrameBuffer(DefaultWidth, DefaultHeight);

		// Act
		InvokeRenderEffect(effect, 0, frameBuffer);
		var heat = GetFireBuffer(effect);

		// Assert
		var simulationWidth = direction is FireDirection.Left or FireDirection.Right
			? DefaultHeight
			: DefaultWidth;
		var simulationHeight = direction is FireDirection.Left or FireDirection.Right
			? DefaultWidth
			: DefaultHeight;
		for (var x = 0; x < simulationWidth; x++)
		{
			var expected = FirePalette.GetColor(heat[x]).ToRGB();
			var (outputX, outputY) = GetSourceOutputCoordinate(direction, x, simulationHeight);
			AssertSameRgb(expected, frameBuffer.GetColorAt(outputX, outputY));
		}
	}

	/// <summary>
	/// Verifies that every lit heat cell is projected to its existing string-render position.
	/// </summary>
	/// <param name="direction">The selected Fire source edge.</param>
	[Theory]
	[InlineData(FireDirection.Bottom)]
	[InlineData(FireDirection.Top)]
	[InlineData(FireDirection.Left)]
	[InlineData(FireDirection.Right)]
	public void Fire_StringRender_ProjectsEveryLitGeneratedHeatCell(FireDirection direction)
	{
		// Arrange
		var effect = new Fire
		{
			Location = direction,
			TimeSpan = TimeSpan.FromMilliseconds(1000)
		};
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(effect);
		var frameBuffer = new PixelFrameBuffer(DefaultWidth, DefaultHeight);

		// Act
		InvokeRenderEffect(effect, 0, frameBuffer);
		var heat = GetFireBuffer(effect);

		// Assert
		var simulationWidth = direction is FireDirection.Left or FireDirection.Right
			? DefaultHeight
			: DefaultWidth;
		var simulationHeight = direction is FireDirection.Left or FireDirection.Right
			? DefaultWidth
			: DefaultHeight;
		for (var y = 0; y < simulationHeight; y++)
		{
			for (var x = 0; x < simulationWidth; x++)
			{
				var colorIndex = heat[y * simulationWidth + x];
				if (colorIndex == 0) continue;

				var (outputX, outputY) = GetOutputCoordinate(direction, x, y, simulationHeight);
				AssertSameRgb(FirePalette.GetColor(colorIndex).ToRGB(), frameBuffer.GetColorAt(outputX, outputY));
			}
		}
	}

	private static (int X, int Y) GetSourceOutputCoordinate(FireDirection direction, int simulationX, int simulationHeight)
	{
		return direction switch
		{
			FireDirection.Bottom => (simulationX, 0),
			FireDirection.Top => (simulationX, simulationHeight - 1),
			FireDirection.Left => (0, simulationX),
			FireDirection.Right => (simulationHeight - 1, simulationX),
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
		};
	}

	private static (int X, int Y) GetOutputCoordinate(FireDirection direction, int simulationX, int simulationY, int simulationHeight)
	{
		var outputY = direction is FireDirection.Top or FireDirection.Right
			? simulationHeight - simulationY - 1
			: simulationY;
		return direction is FireDirection.Left or FireDirection.Right
			? (outputY, simulationX)
			: (simulationX, outputY);
	}

	private static (int X, int Y) GetSimulationCoordinate(FireDirection direction, int outputX, int outputY, int width, int height)
	{
		return direction switch
		{
			FireDirection.Bottom => (outputX, outputY),
			FireDirection.Top => (outputX, height - outputY - 1),
			FireDirection.Left => (outputY, outputX),
			FireDirection.Right => (outputY, width - outputX - 1),
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
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

	private static PropertyDescriptor GetProperty(Fire effect, string propertyName)
	{
		var property = TypeDescriptor.GetProperties(effect)[propertyName];
		Assert.NotNull(property);
		return property;
	}

	private static int[] GetFireBuffer(Fire effect)
	{
		var field = typeof(Fire).GetField("_fireBuffer", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(field);
		return Assert.IsType<int[]>(field.GetValue(effect));
	}

	private static int GetFrameStep(Fire effect, int frame, int simulationHeight)
	{
		var createFrameState = typeof(Fire).GetMethod("CreateFrameState", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(createFrameState);
		var frameState = createFrameState.Invoke(effect, [frame, simulationHeight]);
		Assert.NotNull(frameState);
		var step = frameState.GetType().GetProperty("Step", BindingFlags.Instance | BindingFlags.Public);
		Assert.NotNull(step);
		return Assert.IsType<int>(step.GetValue(frameState));
	}

	private static void SetVirtualBuffer(Fire effect, int width, int height, int xOffset = 0, int yOffset = 0)
	{
		SetPixelEffectBaseField(effect, "_bufferHt", width);
		SetPixelEffectBaseField(effect, "_bufferWi", height);
		SetPixelEffectBaseField(effect, "_bufferHtOffset", xOffset);
		SetPixelEffectBaseField(effect, "_bufferWiOffset", yOffset);
	}

	private static void SetPixelEffectBaseField(Fire effect, string fieldName, int value)
	{
		var field = typeof(PixelEffectBase).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(field);
		field.SetValue(effect, value);
	}

	private static void InvokeSetupRender(Fire effect)
	{
		var setupRender = typeof(Fire).GetMethod("SetupRender", BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(setupRender);
		setupRender.Invoke(effect, []);
	}

	private static void InvokeRenderEffect(Fire effect, int frame, IPixelFrameBuffer frameBuffer)
	{
		var renderEffect = typeof(Fire).GetMethod(
			"RenderEffect",
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(int), typeof(IPixelFrameBuffer)],
			null);
		Assert.NotNull(renderEffect);
		renderEffect.Invoke(effect, [frame, frameBuffer]);
	}

	private static void InvokeRenderByLocation(Fire effect, int numFrames, PixelLocationFrameBuffer frameBuffer)
	{
		var renderByLocation = typeof(Fire).GetMethod(
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

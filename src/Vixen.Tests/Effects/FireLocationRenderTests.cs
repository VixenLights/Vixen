using System.ComponentModel;
using System.Drawing;
using System.Reflection;
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
		var frameBuffer = new PixelLocationFrameBuffer([], 1);
		SetVirtualBuffer(effect, DefaultWidth, DefaultHeight);
		InvokeSetupRender(effect);

		// Act
		var exception = Record.Exception(() => InvokeRenderByLocation(effect, 1, frameBuffer));

		// Assert
		Assert.Null(exception);
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

	private static void SetVirtualBuffer(Fire effect, int width, int height)
	{
		SetPixelEffectBaseField(effect, "_bufferHt", width);
		SetPixelEffectBaseField(effect, "_bufferWi", height);
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

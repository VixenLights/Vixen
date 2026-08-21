using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Moq;
using Vixen.Sys;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.Effect.PinWheel;
using Xunit;

namespace Vixen.Tests.Effects;

public sealed class PinWheelSizeScaleBasisTests
{
	private const int RenderFrame = 0;
	private const int RenderFrameCount = 1;

	[Fact]
	public void PinWheelData_DefaultsSizeScaleBasisToLargestDimension()
	{
		// Arrange and Act
		var data = new PinWheelData();

		// Assert
		Assert.Equal(PinWheelSizeScaleBasis.LargestDimension, data.SizeScaleBasis);
	}

	[Fact]
	public void PinWheelData_OldSerializedPayloadDefaultsSizeScaleBasisToHeight()
	{
		// Arrange
		var data = new PinWheelData { SizeScaleBasis = PinWheelSizeScaleBasis.LargestDimension };
		var oldPayload = RemoveSizeScaleBasis(Serialize(data));

		// Act
		var deserialized = Deserialize(oldPayload);

		// Assert
		Assert.Equal(PinWheelSizeScaleBasis.Height, deserialized.SizeScaleBasis);
	}

	[Theory]
	[InlineData(PinWheelSizeScaleBasis.Height)]
	[InlineData(PinWheelSizeScaleBasis.Width)]
	[InlineData(PinWheelSizeScaleBasis.LargestDimension)]
	public void PinWheelData_RoundTripsSizeScaleBasis(PinWheelSizeScaleBasis sizeScaleBasis)
	{
		// Arrange
		var data = new PinWheelData { SizeScaleBasis = sizeScaleBasis };

		// Act
		var payload = Serialize(data);
		var deserialized = Deserialize(payload);

		// Assert
		Assert.Contains("SizeScaleBasis", payload);
		Assert.Equal(sizeScaleBasis, deserialized.SizeScaleBasis);
	}

	[Theory]
	[InlineData(PinWheelSizeScaleBasis.Height)]
	[InlineData(PinWheelSizeScaleBasis.Width)]
	[InlineData(PinWheelSizeScaleBasis.LargestDimension)]
	public void PinWheelData_ClonePreservesSizeScaleBasis(PinWheelSizeScaleBasis sizeScaleBasis)
	{
		// Arrange
		var data = new PinWheelData { SizeScaleBasis = sizeScaleBasis };

		// Act
		var clone = Assert.IsType<PinWheelData>(data.Clone());

		// Assert
		Assert.Equal(sizeScaleBasis, clone.SizeScaleBasis);
	}

	[Fact]
	public void PinWheel_SizeScaleBasisIsBrowsableOnlyForPercentageOffsets()
	{
		// Arrange
		var effect = new PinWheel();

		// Act
		var percentageProperty = TypeDescriptor.GetProperties(effect)[nameof(PinWheel.SizeScaleBasis)];
		effect.OffsetPercentage = false;
		var absoluteOffsetProperty = TypeDescriptor.GetProperties(effect)[nameof(PinWheel.SizeScaleBasis)];

		// Assert
		Assert.True(percentageProperty?.IsBrowsable);
		Assert.False(absoluteOffsetProperty?.IsBrowsable);
	}

	[Fact]
	public void PinWheel_WideBuffer_UsesConfiguredSizeScaleBasis()
	{
		// Arrange
		const int width = 20;
		const int height = 10;
		var target = CreateElementLocation(19, 5);

		// Act
		var heightColor = RenderLocation(width, height, [target], PinWheelSizeScaleBasis.Height).GetColorAt(target.X, target.Y);
		var widthColor = RenderLocation(width, height, [target], PinWheelSizeScaleBasis.Width).GetColorAt(target.X, target.Y);
		var largestDimensionColor = RenderLocation(width, height, [target], PinWheelSizeScaleBasis.LargestDimension).GetColorAt(target.X, target.Y);

		// Assert
		AssertUnlit(heightColor);
		AssertLit(widthColor);
		AssertLit(largestDimensionColor);
	}

	[Fact]
	public void PinWheel_TallBuffer_UsesHeightForLargestDimension()
	{
		// Arrange
		const int width = 10;
		const int height = 20;
		var target = CreateElementLocation(5, 19);

		// Act
		var heightColor = RenderLocation(width, height, [target], PinWheelSizeScaleBasis.Height).GetColorAt(target.X, target.Y);
		var widthColor = RenderLocation(width, height, [target], PinWheelSizeScaleBasis.Width).GetColorAt(target.X, target.Y);
		var largestDimensionColor = RenderLocation(width, height, [target], PinWheelSizeScaleBasis.LargestDimension).GetColorAt(target.X, target.Y);

		// Assert
		AssertLit(heightColor);
		AssertUnlit(widthColor);
		AssertLit(largestDimensionColor);
	}

	[Fact]
	public void PinWheel_SquareBuffer_RendersIdenticallyForEverySizeScaleBasis()
	{
		// Arrange
		const int dimension = 12;
		var locations = CreateGridLocations(dimension, dimension);

		// Act
		var height = RenderLocation(dimension, dimension, locations, PinWheelSizeScaleBasis.Height);
		var width = RenderLocation(dimension, dimension, locations, PinWheelSizeScaleBasis.Width);
		var largestDimension = RenderLocation(dimension, dimension, locations, PinWheelSizeScaleBasis.LargestDimension);

		// Assert
		AssertSameLocationBuffer(locations, height, width);
		AssertSameLocationBuffer(locations, height, largestDimension);
	}

	[Theory]
	[InlineData(PinWheelSizeScaleBasis.Height)]
	[InlineData(PinWheelSizeScaleBasis.Width)]
	[InlineData(PinWheelSizeScaleBasis.LargestDimension)]
	public void PinWheel_StringAndLocationRenderingUseTheSameSizeScaleBasis(PinWheelSizeScaleBasis sizeScaleBasis)
	{
		// Arrange
		const int width = 13;
		const int height = 7;
		var locations = CreateGridLocations(width, height);
		var stringEffect = CreateDeterministicPinWheel(sizeScaleBasis);
		var locationEffect = CreateDeterministicPinWheel(sizeScaleBasis);
		SetVirtualBuffer(stringEffect, width, height);
		SetVirtualBuffer(locationEffect, width, height);
		var stringBuffer = new PixelFrameBuffer(width, height);
		var locationBuffer = new PixelLocationFrameBuffer(locations, RenderFrameCount);

		// Act
		InvokeRenderEffect(stringEffect, RenderFrame, stringBuffer);
		InvokeRenderByLocation(locationEffect, RenderFrameCount, locationBuffer);

		// Assert
		foreach (var location in locations)
		{
			AssertSameRgb(stringBuffer.GetColorAt(location.X, height - 1 - location.Y), locationBuffer.GetColorAt(location.X, location.Y));
		}
	}

	[Fact]
	public void PinWheel_UnknownSizeScaleBasisFallsBackToHeight()
	{
		// Arrange
		const int width = 20;
		const int height = 10;
		var locations = CreateGridLocations(width, height);

		// Act
		var heightBuffer = RenderLocation(width, height, locations, PinWheelSizeScaleBasis.Height);
		var unknownBuffer = RenderLocation(width, height, locations, (PinWheelSizeScaleBasis)999);

		// Assert
		AssertSameLocationBuffer(locations, heightBuffer, unknownBuffer);
	}

	[Fact]
	public void PinWheel_AbsoluteOffsetsIgnoreSizeScaleBasis()
	{
		// Arrange
		const int width = 13;
		const int height = 7;
		var locations = CreateGridLocations(width, height);

		// Act
		var heightBuffer = RenderLocation(width, height, locations, PinWheelSizeScaleBasis.Height, offsetPercentage: false);
		var widthBuffer = RenderLocation(width, height, locations, PinWheelSizeScaleBasis.Width, offsetPercentage: false);
		var largestDimensionBuffer = RenderLocation(width, height, locations, PinWheelSizeScaleBasis.LargestDimension, offsetPercentage: false);

		// Assert
		AssertSameLocationBuffer(locations, heightBuffer, widthBuffer);
		AssertSameLocationBuffer(locations, heightBuffer, largestDimensionBuffer);
	}

	private static PinWheel CreateDeterministicPinWheel(PinWheelSizeScaleBasis sizeScaleBasis, bool offsetPercentage = true)
	{
		return new PinWheel
		{
			TimeSpan = TimeSpan.FromMilliseconds(1000),
			Arms = 1,
			ColorType = PinWheelColorType.Standard,
			Colors = [new GradientLevelPair(Color.Red, CurveType.Flat100)],
			ThicknessCurve = new Curve(100),
			SizeCurve = new Curve(20),
			CenterHubCurve = new Curve(0d),
			LevelCurve = new Curve(100),
			RotationCurve = new Curve(50),
			TwistCurve = new Curve(50),
			SpeedCurve = new Curve(50),
			XOffsetCurve = new Curve(50),
			YOffsetCurve = new Curve(50),
			SizeScaleBasis = sizeScaleBasis,
			OffsetPercentage = offsetPercentage
		};
	}

	private static PixelLocationFrameBuffer RenderLocation(int width, int height, List<ElementLocation> locations, PinWheelSizeScaleBasis sizeScaleBasis, bool offsetPercentage = true)
	{
		var effect = CreateDeterministicPinWheel(sizeScaleBasis, offsetPercentage);
		SetVirtualBuffer(effect, width, height);
		var buffer = new PixelLocationFrameBuffer(locations, RenderFrameCount);

		InvokeRenderByLocation(effect, RenderFrameCount, buffer);

		return buffer;
	}

	private static List<ElementLocation> CreateGridLocations(int width, int height)
	{
		var locations = new List<ElementLocation>();
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				locations.Add(CreateElementLocation(x, y));
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

	private static void SetVirtualBuffer(PinWheel effect, int width, int height)
	{
		SetPixelEffectBaseField(effect, "_bufferHt", width);
		SetPixelEffectBaseField(effect, "_bufferWi", height);
		SetPixelEffectBaseField(effect, "_bufferHtOffset", 0);
		SetPixelEffectBaseField(effect, "_bufferWiOffset", 0);
	}

	private static void SetPixelEffectBaseField(PinWheel effect, string fieldName, int value)
	{
		var field = typeof(PixelEffectBase).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
		Assert.NotNull(field);
		field.SetValue(effect, value);
	}

	private static void InvokeRenderEffect(PinWheel effect, int frame, IPixelFrameBuffer frameBuffer)
	{
		var renderEffect = typeof(PinWheel).GetMethod(
			"RenderEffect",
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(int), typeof(IPixelFrameBuffer)],
			null);
		Assert.NotNull(renderEffect);
		renderEffect.Invoke(effect, [frame, frameBuffer]);
	}

	private static void InvokeRenderByLocation(PinWheel effect, int frameCount, PixelLocationFrameBuffer frameBuffer)
	{
		var renderByLocation = typeof(PinWheel).GetMethod(
			"RenderEffectByLocation",
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[typeof(int), typeof(PixelLocationFrameBuffer)],
			null);
		Assert.NotNull(renderByLocation);
		renderByLocation.Invoke(effect, [frameCount, frameBuffer]);
	}

	private static string Serialize(PinWheelData data)
	{
		var serializer = new DataContractSerializer(typeof(PinWheelData));
		using var stream = new MemoryStream();
		serializer.WriteObject(stream, data);
		return System.Text.Encoding.UTF8.GetString(stream.ToArray());
	}

	private static PinWheelData Deserialize(string payload)
	{
		var serializer = new DataContractSerializer(typeof(PinWheelData));
		using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(payload));
		return Assert.IsType<PinWheelData>(serializer.ReadObject(stream));
	}

	private static string RemoveSizeScaleBasis(string payload)
	{
		var document = XDocument.Parse(payload);
		document.Descendants().Where(element => element.Name.LocalName == nameof(PinWheelData.SizeScaleBasis)).Remove();
		return document.ToString(SaveOptions.DisableFormatting);
	}

	private static void AssertSameLocationBuffer(IEnumerable<ElementLocation> locations, PixelLocationFrameBuffer expected, PixelLocationFrameBuffer actual)
	{
		foreach (var location in locations)
		{
			AssertSameRgb(expected.GetColorAt(location.X, location.Y), actual.GetColorAt(location.X, location.Y));
		}
	}

	private static void AssertLit(Color color)
	{
		Assert.NotEqual(Color.Black.ToArgb(), color.ToArgb());
	}

	private static void AssertUnlit(Color color)
	{
		Assert.Equal(Color.Black.ToArgb(), color.ToArgb());
	}

	private static void AssertSameRgb(Color expected, Color actual)
	{
		Assert.Equal(expected.R, actual.R);
		Assert.Equal(expected.G, actual.G);
		Assert.Equal(expected.B, actual.B);
	}
}

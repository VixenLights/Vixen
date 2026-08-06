using System.Drawing;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Editor.TimedSequenceEditor;
using Xunit;

namespace Vixen.Tests.Sequencer;

public sealed class LibraryDragPayloadFactoryTests
{
	[Fact]
	public void Create_WhenLinkedCurve_ReturnsIndependentLinkedCopy()
	{
		var source = new Curve();
		source.Points.Add(25, 75);
		source.IsCurrentLibraryCurve = true;

		var payload = Assert.IsType<Curve>(LibraryDragPayloadFactory.Create(source, "Twinkle", true));

		Assert.NotSame(source, payload);
		Assert.Equal("Twinkle", payload.LibraryReferenceName);
		Assert.False(payload.IsCurrentLibraryCurve);
		Assert.Equal(source.Points.Count, payload.Points.Count);
		Assert.NotSame(source.Points, payload.Points);
		Assert.Empty(source.LibraryReferenceName);
		Assert.True(source.IsCurrentLibraryCurve);

		payload.Points[0].Y = 42;

		Assert.NotEqual(payload.Points[0].Y, source.Points[0].Y);
	}

	[Fact]
	public void Create_WhenUnlinkedCurve_ReturnsIndependentUnlinkedCopy()
	{
		var source = new Curve();
		source.Points.Add(50, 25);
		source.IsCurrentLibraryCurve = true;

		var payload = Assert.IsType<Curve>(LibraryDragPayloadFactory.Create(source, "Twinkle", false));

		Assert.NotSame(source, payload);
		Assert.Empty(payload.LibraryReferenceName);
		Assert.False(payload.IsCurrentLibraryCurve);
		Assert.Equal(source.Points.Count, payload.Points.Count);
		Assert.True(source.IsCurrentLibraryCurve);
	}

	[Theory]
	[InlineData(true, "Warm fade")]
	[InlineData(false, "")]
	public void Create_WhenGradient_ReturnsIndependentCopyWithExpectedLibraryState(bool linkToLibrary, string expectedLibraryReferenceName)
	{
		var source = new ColorGradient();
		source.Colors.Add(new ColorPoint(Color.Red, 0.5));
		source.Alphas.Add(new AlphaPoint(128, 0.5));
		source.IsCurrentLibraryGradient = true;

		var payload = Assert.IsType<ColorGradient>(LibraryDragPayloadFactory.Create(source, "Warm fade", linkToLibrary));

		Assert.NotSame(source, payload);
		Assert.Equal(expectedLibraryReferenceName, payload.LibraryReferenceName);
		Assert.False(payload.IsCurrentLibraryGradient);
		if (linkToLibrary) {
			payload.UnlinkFromLibrary();
		}

		Assert.Equal(source.Colors.Count, payload.Colors.Count);
		Assert.Equal(source.Alphas.Count, payload.Alphas.Count);
		Assert.NotSame(source.Colors[0], payload.Colors[0]);
		Assert.NotSame(source.Alphas[0], payload.Alphas[0]);
		Assert.Empty(source.LibraryReferenceName);
		Assert.True(source.IsCurrentLibraryGradient);
	}

	[Fact]
	public void Create_WhenColor_ReturnsEquivalentColorValue()
	{
		var source = Color.FromArgb(128, 10, 20, 30);

		var payload = Assert.IsType<Color>(LibraryDragPayloadFactory.Create(source, "Blue", true));

		Assert.Equal(source, payload);
	}

	[Fact]
	public void Create_WhenSourceValueIsNull_ThrowsArgumentNullException()
	{
		Assert.Throws<ArgumentNullException>(() => LibraryDragPayloadFactory.Create(null!, "Twinkle", false));
	}

	[Fact]
	public void Create_WhenSourceValueIsUnsupported_ThrowsArgumentException()
	{
		Assert.Throws<ArgumentException>(() => LibraryDragPayloadFactory.Create("unsupported", "Twinkle", false));
	}

	[Theory]
	[InlineData("")]
	[InlineData(" ")]
	public void Create_WhenLinkedPayloadHasNoLibraryItemName_ThrowsArgumentException(string libraryItemName)
	{
		Assert.Throws<ArgumentException>(() => LibraryDragPayloadFactory.Create(new Curve(), libraryItemName, true));
	}
}

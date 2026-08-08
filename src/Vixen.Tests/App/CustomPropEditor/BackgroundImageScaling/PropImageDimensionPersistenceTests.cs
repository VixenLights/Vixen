using System.Windows.Media;
using System.Windows.Media.Imaging;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Xunit;

namespace Vixen.Tests.App.CustomPropEditor.BackgroundImageScaling;

[Collection("CustomPropEditor")]
public sealed class PropImageDimensionPersistenceTests
{
	[Fact]
	public void ImageAssignment_PreservesValidLogicalDimensions()
	{
		var prop = new Prop("Persistence test")
		{
			Width = 640,
			Height = 400
		};

		prop.Image = CreateBitmap(4032, 3024);

		Assert.Equal(640, prop.Width);
		Assert.Equal(400, prop.Height);
	}

	[Theory]
	[InlineData(0, 400)]
	[InlineData(640, 0)]
	[InlineData(-1, 400)]
	[InlineData(640, -1)]
	[InlineData(100001, 400)]
	[InlineData(640, 100001)]
	public void ImageAssignment_InvalidLogicalDimensionsFallBackToNativePixels(double width, double height)
	{
		var prop = new Prop("Legacy test")
		{
			Width = width,
			Height = height
		};

		prop.Image = CreateBitmap(320, 240);

		Assert.Equal(320, prop.Width);
		Assert.Equal(240, prop.Height);
	}

	[Fact]
	public void SetImage_ResetsLogicalDimensionsToNewImageNativePixels()
	{
		var path = Path.Combine(Path.GetTempPath(), $"VIX-2499-{Guid.NewGuid():N}.png");
		try
		{
			SavePng(CreateBitmap(320, 240), path);
			var service = PropModelServices.Instance();
			var prop = service.CreateProp("New image test", 4032, 3024);
			prop.Width = 1008;
			prop.Height = 756;

			service.SetImage(path);

			Assert.Equal(320, prop.Width);
			Assert.Equal(240, prop.Height);
		}
		finally
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}

	[Fact]
	public async Task SaveAndLoad_PreservesScaledLogicalDimensions()
	{
		var path = Path.Combine(Path.GetTempPath(), $"VIX-2499-{Guid.NewGuid():N}.prp");
		try
		{
			var prop = new Prop("Persistence test", 4032, 3024)
			{
				Width = 1008,
				Height = 756
			};

			var persistence = new PropModelPersistenceService();
			await persistence.SaveAsync(prop, path, TestContext.Current.CancellationToken);
			var loaded = await persistence.LoadAsync(path, TestContext.Current.CancellationToken);

			Assert.NotNull(loaded);
			Assert.Equal(1008, loaded.Width);
			Assert.Equal(756, loaded.Height);
			Assert.Equal(4032, loaded.Image.PixelWidth);
			Assert.Equal(3024, loaded.Image.PixelHeight);
		}
		finally
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
	}

	private static BitmapSource CreateBitmap(int width, int height)
	{
		var pixels = new byte[width * height * 4];
		return BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, width * 4);
	}

	private static void SavePng(BitmapSource bitmap, string path)
	{
		var encoder = new PngBitmapEncoder();
		encoder.Frames.Add(BitmapFrame.Create(bitmap));
		using var stream = File.Create(path);
		encoder.Save(stream);
	}
}

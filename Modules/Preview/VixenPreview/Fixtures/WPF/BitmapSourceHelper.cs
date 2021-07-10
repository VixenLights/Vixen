using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;

namespace VixenModules.Preview.VixenPreview.Fixtures.WPF
{
	/// <summary>
	/// Maintains the colors of a pixel.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct PixelColor
	{
		public byte Blue;
		public byte Green;
		public byte Red;
		public byte Alpha;
	}

	/// <summary>
	/// Convert a BitmapSource to pixels.
	/// </summary>
	/// <remarks>
	/// This code is from the following URL:
	/// https://stackoverflow.com/questions/1176910/finding-specific-pixel-colors-of-a-bitmapimage
	/// </remarks>
	public static class BitmapSourceHelper
	{
#if UNSAFE
  public unsafe static void CopyPixels(this BitmapSource source, PixelColor[,] pixels, int stride, int offset)
  {
    fixed(PixelColor* buffer = &pixels[0, 0])
      source.CopyPixels(
        new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight),
        (IntPtr)(buffer + offset),
        pixels.GetLength(0) * pixels.GetLength(1) * sizeof(PixelColor),
        stride);
  }
#else
		/// <summary>
		/// Copies the BitmapSource to the specified pixel array.
		/// </summary>
		/// <param name="source">BitmapSource to convert</param>
		/// <param name="pixels">Destination pixel array</param>
		/// <param name="stride">Number of bytes per pixel</param>
		/// <param name="offset">Offset into the destination pixel array</param>
		public static void CopyPixelsVixen(this BitmapSource source, PixelColor[,] pixels, int stride, int offset)
		{
			var height = source.PixelHeight;
			var width = source.PixelWidth;
			var pixelBytes = new byte[height * width * 4];
			source.CopyPixels(pixelBytes, stride, 0);
			int y0 = offset / width;
			int x0 = offset - width * y0;
			for (int y = 0; y < height; y++)
			for (int x = 0; x < width; x++)
				pixels[x + x0, y + y0] = new PixelColor
				{
					Blue = pixelBytes[(y * width + x) * 4 + 0],
					Green = pixelBytes[(y * width + x) * 4 + 1],
					Red = pixelBytes[(y * width + x) * 4 + 2],
					Alpha = pixelBytes[(y * width + x) * 4 + 3],
				};
		}		
#endif
	}
}

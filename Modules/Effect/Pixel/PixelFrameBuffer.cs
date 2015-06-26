using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace VixenModules.Effect.Pixel
{
	public struct PixelFrameBuffer
	{
		private Color[][] _pixels;
		private readonly int _bufferWi;
		private readonly int _bufferHt;

		public PixelFrameBuffer(int width, int height) : this()
		{
			_bufferWi = width;
			_bufferHt = height;
			InitBuffer();
		}

		private void InitBuffer()
		{
			_pixels = new Color[_bufferWi][];
			for (int i = 0; i < _bufferWi; i++)
			{
				_pixels[i] = new Color[_bufferHt];
			}
		}

		public void ClearBuffer()
		{
			for (int i = 0; i < _bufferWi; i++)
			{
				for (int z = 0; z < _bufferHt; z++)
				{
					_pixels[i][z] = Color.Transparent;
				}
			}
		}

		// 0,0 is lower left
		public void SetPixel(int x, int y, Color color)
		{
			if (x >= 0 && x < _bufferWi && y >= 0 && y < _bufferHt)
			{
				_pixels[x][y] = color;
			}
		}

		// 0,0 is lower left
		public void SetPixel(int x, int y, HSV hsv)
		{
			Color color = hsv.ToRGB().ToArgb();
			SetPixel(x, y, color);
		}

		public Color GetColorAt(int x, int y)
		{
			return _pixels[x][y];
		}
	}
}

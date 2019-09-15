using System.Collections.Generic;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace VixenModules.Effect.Effect
{
	public class PixelFrameBuffer:IPixelFrameBuffer
	{
		private Color[][] _pixels;
		private readonly int _bufferWi;
		private readonly int _bufferHt;
		private readonly Color _baseColor;

		public PixelFrameBuffer(int width, int height) :this(width, height, Color.Transparent)
		{
			
		}

		public PixelFrameBuffer(int width, int height, Color baseColor)
		{
			_bufferWi = width;
			_bufferHt = height;
			_baseColor = baseColor;
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
					_pixels[i][z] = _baseColor;
				}
			}
		}

		public void ClearBuffer(double level)
		{
			var hsv = HSV.FromRGB(_baseColor);
			hsv.V = hsv.V * level;
			for (int i = 0; i < _bufferWi; i++)
			{
				for (int z = 0; z < _bufferHt; z++)
				{
					_pixels[i][z] = hsv.ToRGB();
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
			Color color = hsv.ToRGB();
			SetPixel(x, y, color);
		}

		public bool ContainsRow(int x)
		{
			//TODO implement this!!
			return true;
		}

		public List<int> XIndexes { get; private set; }
		public List<int> YIndexes { get; private set; }

		public Color GetColorAt(int x, int y)
		{
			return _pixels[x][y];
		}
	}
}

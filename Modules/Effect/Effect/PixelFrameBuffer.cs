using System;
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
			if (_pixels.Length > 0)
			{
				InitArray(_pixels[0], _baseColor);
				for (int i = 1; i < _bufferWi; i++)
				{
					Array.Copy(_pixels[0],_pixels[i], _pixels[i].Length);
				}
			}
		}

		public void ClearBuffer(double level)
		{
			var hsv = HSV.FromRGB(_baseColor);
			hsv.V = hsv.V * level;
			if (_pixels.Length > 0)
			{
				InitArray(_pixels[0], hsv.ToRGB());
				for (int i = 1; i < _bufferWi; i++)
				{
					Array.Copy(_pixels[0],_pixels[i], _pixels[i].Length);
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

		/// <summary>
		/// Initializes an array with a specific value in a highly optimized fashion.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="array"></param>
		/// <param name="value"></param>
		private static void InitArray<T>(T[] array, T value) 
		{
			int length = array.Length;
			if (length == 0) return;
			array[0] = value;
			int count;
			for (count = 1; count <= length / 2; count *= 2)
			{
				Array.Copy(array, 0, array, count, count);
			}
			Array.Copy(array, 0, array, count, length - count);
		}
	}
}

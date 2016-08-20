using System;
using System.Collections.Generic;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;

namespace VixenModules.Effect.Effect.Location
{
	public class PixelLocationFrameBuffer:IPixelFrameBuffer
	{
		private readonly SparseMatrix<Tuple<ElementLocation,List<RGBValue>>> _data;
		
		public PixelLocationFrameBuffer(List<ElementLocation> nodes, int numFrames)
		{
			ElementLocations = nodes;
			_data = new SparseMatrix<Tuple<ElementLocation, List<RGBValue>>>();
			AddData(nodes, numFrames);
		}
		
		public List<ElementLocation> ElementLocations { get; private set; }

		/// <summary>
		/// Add a frame and set the pixel to the given color. This cannot be used to update a pixel that 
		/// has already been set. See update functions.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="c"></param>
		public void SetPixel(int x, int y, Color c)
		{
			Tuple<ElementLocation, List<RGBValue>> elementData;
			if (_data.TryGetAt(x,y, out elementData))
			{
				elementData.Item2.Add(new RGBValue(c));
			}
		}

		/// <summary>
		/// Add a frame and set the pixel to the given color. This cannot be used to update a pixel that 
		/// has already been set. See update functions.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="hsv"></param>
		public void SetPixel(int x, int y, HSV hsv)
		{
			var color = hsv.ToRGB();
			SetPixel(x, y, color);
		}

		public Color GetColorAt(int x, int y)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// This adds a frame worth of data to the buffer in a transparent color.
		/// </summary>
		public void InitializeNextFrame()
		{
			foreach (var tuple in _data.GetData())
			{
				tuple.Item2.Add(new RGBValue(Color.Transparent));
			}
		}

		/// <summary>
		/// Update a existing pixel in the specified frame. The pixel must exist from either the InitializeNextFrame
		/// or the SetPixel methods or an exception will occur.
		/// </summary>
		/// <param name="frame"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="c"></param>
		public void UpdatePixel(int frame, int x, int y, Color c)
		{
			Tuple<ElementLocation, List<RGBValue>> elementData;
			if (_data.TryGetAt(x, y, out elementData))
			{
				elementData.Item2[frame]= new RGBValue(c);
			}
		}

		/// <summary>
		/// Update a existing pixel in the specified frame. The pixel must exist from either the InitializeNextFrame
		/// or the SetPixel methods or an exception will occur.
		/// </summary>
		/// <param name="frame"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="hsv"></param>
		public void UpdatePixel(int frame, int x, int y, HSV hsv)
		{
			var color = hsv.ToRGB();
			UpdatePixel(frame, x, y, color);
		}


		public List<Tuple<ElementLocation, List<RGBValue>>> GetElementData()
		{
			return _data.GetData();
		} 

		private void AddData(List<ElementLocation> nodes, int numFrames)
		{
			foreach (var elementLocation in nodes)
			{
				_data.SetAt(elementLocation.X, elementLocation.Y, new Tuple<ElementLocation, List<RGBValue>>(elementLocation, new List<RGBValue>(numFrames)));
			}

		}
	}
}

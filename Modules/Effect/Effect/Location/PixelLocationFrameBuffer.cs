using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;

namespace VixenModules.Effect.Effect.Location
{
	public class PixelLocationFrameBuffer:IPixelFrameBuffer
	{
		private readonly SparseMatrix<RGBValue[]> _data;
		
		public PixelLocationFrameBuffer(List<ElementLocation> elementLocations, int numFrames)
		{
			CurrentFrame = 0;
			ElementLocations = elementLocations.Distinct();
			_data = new SparseMatrix<RGBValue[]>();
			AddData(elementLocations, numFrames);
		}
		
		/// <summary>
		/// Retrieves the list of all the distinct element locations
		/// </summary>
		public IEnumerable<ElementLocation> ElementLocations { get; private set; }

		/// <summary>
		/// Holds the current frame for data within the buffer. Any updates or retrievals will be made based on this frame setting.
		/// Consumers should advance this as needed to keep the buffer on the current frame
		/// </summary>
		public int CurrentFrame { get; set; }

		/// <summary>
		/// Add a frame and set the pixel to the given color. This cannot be used to update a pixel that 
		/// has already been set. See update functions.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="c"></param>
		public void SetPixel(int x, int y, Color c)
		{
			RGBValue[] elementData;
			if (_data.TryGetAt(x,y, out elementData))
			{
				elementData[CurrentFrame] = new RGBValue(c);
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
			var frameData = GetFrameDataAt(x, y);
			return frameData[CurrentFrame].FullColor;
		}

		/// <summary>
		/// Sets all the locations in the current frame with a transparent color.
		/// </summary>
		public void ClearFrame()
		{
			foreach (var tuple in _data.GetData())
			{
				tuple[CurrentFrame] = new RGBValue(Color.Transparent);
			}
		}

		public RGBValue[] GetFrameDataAt(int x, int y)
		{
			return _data.GetAt(x, y);
		}

		private void AddData(List<ElementLocation> nodes, int numFrames)
		{
			foreach (var elementLocation in nodes)
			{
				_data.SetAt(elementLocation.X, elementLocation.Y, new RGBValue[numFrames]);
			}

		}
	}
}

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

		public void SetPixel(int x, int y, Color c)
		{
			Tuple<ElementLocation, List<RGBValue>> elementData;
			if (_data.TryGetAt(x,y, out elementData))
			{
				elementData.Item2.Add(new RGBValue(c));
			}
		}

		public void SetPixel(int x, int y, HSV hsv)
		{
			var color = hsv.ToRGB();
			SetPixel(x, y, color);
		}
		
		public List<Tuple<ElementLocation, List<RGBValue>>> GetElementData()
		{
			return _data.GetData();
		} 

		private void AddData(List<ElementLocation> nodes, int numFrames)
		{
			nodes.Sort(new LocationComparer(true));
			foreach (var elementLocation in nodes)
			{
				_data.SetAt(elementLocation.X, elementLocation.Y, new Tuple<ElementLocation, List<RGBValue>>(elementLocation, new List<RGBValue>(numFrames)));
			}

		}
	}
}

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
		private readonly int _count;
		
		public PixelLocationFrameBuffer(List<ElementLocation> nodes, int numFrames, int xOffset, int yOffset, int height)
		{
			_count = nodes.Count;
			XOffset = xOffset;
			YOffset = yOffset;
			Height = height-1;
			ElementLocations = nodes;
			_data = new SparseMatrix<Tuple<ElementLocation, List<RGBValue>>>();
			AddData(nodes, numFrames);
		}

		public List<ElementLocation> ElementLocations { get; private set; }

		public int Height { get; private set; }

		public int XOffset { get; private set; }

		public int YOffset { get; private set; }

		public void SetPixel(int x, int y, Color c)
		{
			Tuple<ElementLocation, List<RGBValue>> elementData;
			if (_data.TryGetAt(XOffset + x,Height-y+YOffset, out elementData))
			{
				elementData.Item2.Add(new RGBValue(c));
			}
		}

		public void SetPixel(int x, int y, HSV hsv)
		{
			var color = hsv.ToRGB();
			SetPixel(x, y, color);
		}

		public bool ContainsRow(int x)
		{
			return _data.ContainsRow(XOffset + x);
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

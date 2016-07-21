using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Data.Value;

namespace VixenModules.Effect.Effect.Location
{
	public class PixelLocationFrameBuffer:IPixelFrameBuffer
	{
		private readonly SparseMatrix<Tuple<ElementLocation,List<RGBValue>>> _data;
		private List<int> _yIndexes = new List<int>(); 
		private List<int> _xIndexes = new List<int>();
		
		public PixelLocationFrameBuffer(List<ElementLocation> nodes, int numFrames, int xOffset, int yOffset, int height)
		{
			XOffset = xOffset;
			YOffset = yOffset;
			Height = height-1;
			ElementLocations = nodes;
			_data = new SparseMatrix<Tuple<ElementLocation, List<RGBValue>>>();
			AddData(nodes, numFrames);
			CleanseIndexes();
		}

		private void CleanseIndexes()
		{
			_yIndexes.Sort();
			_yIndexes = _yIndexes.Distinct().ToList();
			_xIndexes.Sort();
			_xIndexes = _xIndexes.Distinct().ToList();
		}

		public void OrderByY()
		{
			ElementLocations.Sort(new LocationComparer(true));
		}

		public List<ElementLocation> ElementLocations { get; private set; }

		public int Height { get; private set; }

		public int XOffset { get; private set; }

		public int YOffset { get; private set; }

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
			nodes.Sort(new LocationComparer(true));
			foreach (var elementLocation in nodes)
			{
				_data.SetAt(elementLocation.X, elementLocation.Y, new Tuple<ElementLocation, List<RGBValue>>(elementLocation, new List<RGBValue>(numFrames)));
			}

		}
	}
}

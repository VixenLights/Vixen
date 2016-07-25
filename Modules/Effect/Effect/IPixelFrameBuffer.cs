using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Common.Controls.ColorManagement.ColorModels;
using ZedGraph;

namespace VixenModules.Effect.Effect
{
	public interface IPixelFrameBuffer
	{
		void SetPixel(int x, int y, Color c);
		

		void SetPixel(int x, int y, HSV hsv);

		Color GetColorAt(int x, int y);

	}
}

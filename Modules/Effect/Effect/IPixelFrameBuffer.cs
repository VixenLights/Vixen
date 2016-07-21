using System.Collections.Generic;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace VixenModules.Effect.Effect
{
	public interface IPixelFrameBuffer
	{
		void SetPixel(int x, int y, Color c);
		

		void SetPixel(int x, int y, HSV hsv);

		bool ContainsRow(int x);
		
	}
}

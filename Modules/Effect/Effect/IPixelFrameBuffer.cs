using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;

namespace VixenModules.Effect.Effect
{
	public interface IPixelFrameBuffer
	{
		void SetPixel(int x, int y, Color c);
		

		void SetPixel(int x, int y, HSV hsv);

		Color GetColorAt(int x, int y);

	}
}

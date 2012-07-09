using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Common.Controls.ColorManagement.ColorPicker
{
	/// <summary>
	/// gdi wrapper - get pixel
	/// </summary>
	internal class GDI32
	{
		// functions
		[DllImport("user32.dll")]
		private static extern IntPtr GetDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		private static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

		[DllImport("gdi32.dll")]
		private static extern int GetPixel(IntPtr hdc, int x, int y);

		/// <summary>
		/// returns the color of any location on the screen
		/// </summary>
		public static Color GetScreenPixel(int x, int y)
		{
			IntPtr descdc=GetDC(IntPtr.Zero);
			Color res=ColorTranslator.FromWin32(GetPixel(descdc,x,y));
			ReleaseDC(IntPtr.Zero,descdc);
			return res;
		}
	}
}

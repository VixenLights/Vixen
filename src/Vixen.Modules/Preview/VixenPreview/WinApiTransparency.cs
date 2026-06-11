using System.Runtime.InteropServices;

namespace VixenModules.Preview.VixenPreview
{
	/// <summary>
	/// Win32 API helpers for layered window transparency (LWA_COLORKEY).
	/// </summary>
	internal static class WinApiTransparency
	{
		internal const int GwlExStyle = -20;
		internal const int WsExLayered = 0x00080000;
		internal const int LwaColorKey = 0x00000001;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

		/// <summary>
		/// Enables color-key transparency on the given window handle using black (0x000000) as the key color.
		/// Black pixels anywhere in the window become visually transparent and click-through.
		/// </summary>
		internal static void EnableTransparency(IntPtr handle)
		{
			int style = GetWindowLong(handle, GwlExStyle);
			SetWindowLong(handle, GwlExStyle, style | WsExLayered);
			SetLayeredWindowAttributes(handle, 0x000000, 0, LwaColorKey);
		}

		/// <summary>
		/// Removes color-key transparency from the given window handle, restoring normal opaque rendering.
		/// </summary>
		internal static void DisableTransparency(IntPtr handle)
		{
			int style = GetWindowLong(handle, GwlExStyle);
			SetWindowLong(handle, GwlExStyle, style & ~WsExLayered);
		}
	}
}

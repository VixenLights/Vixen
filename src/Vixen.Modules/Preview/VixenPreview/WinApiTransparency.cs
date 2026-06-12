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

		private const uint SwpNomove = 0x0002;
		private const uint SwpNosize = 0x0001;
		private const uint SwpNozorder = 0x0004;
		private const uint SwpFramechanged = 0x0020;

		/// <summary>
		/// Transparency key color: RGB(1,1,1) — visually indistinguishable from black but distinct from the
		/// pure black (0,0,0) that Windows uses for system chrome (inactive title bar, resize borders).
		/// This ensures the window frame remains opaque and interactive while the preview background
		/// becomes transparent.
		/// </summary>
		internal static readonly Color KeyColor = Color.FromArgb(1, 1, 1);

		/// <summary>
		/// A nearly black non-key color used for fixed resize bands in transparent preview controls.
		/// </summary>
		internal static readonly Color ResizeBandColor = Color.FromArgb(2, 2, 2);

		/// <summary>COLORREF encoding of <see cref="KeyColor"/> (0x00BBGGRR).</summary>
		private const uint KeyColorRef = 0x00010101;

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		internal static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

		/// <summary>
		/// Enables color-key transparency on the given window handle. Pixels rendered with
		/// <see cref="KeyColor"/> become visually transparent and click-through; all other pixels
		/// (including system-drawn window chrome) remain fully opaque.
		/// </summary>
		internal static void EnableTransparency(IntPtr handle)
		{
			int style = GetWindowLong(handle, GwlExStyle);
			SetWindowLong(handle, GwlExStyle, style | WsExLayered);
			// Force the non-client area (borders, title bar) to redraw after the style change.
			SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpNozorder | SwpFramechanged);
			SetLayeredWindowAttributes(handle, KeyColorRef, 0, LwaColorKey);
		}

		/// <summary>
		/// Removes color-key transparency from the given window handle, restoring normal opaque rendering.
		/// </summary>
		internal static void DisableTransparency(IntPtr handle)
		{
			int style = GetWindowLong(handle, GwlExStyle);
			SetWindowLong(handle, GwlExStyle, style & ~WsExLayered);
			SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, SwpNomove | SwpNosize | SwpNozorder | SwpFramechanged);
		}
	}
}

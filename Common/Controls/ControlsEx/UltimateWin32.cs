using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Common.Controls.ControlsEx
{
	public class Win32
	{
		private Win32()
		{
		}

		#region functions

		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int CallNextHookEx(IntPtr hookHandle, int code, IntPtr wparam, ref CWPSTRUCT cwp);

		[DllImport("shell32.dll", EntryPoint = "ExtractIconA")]
		public static extern IntPtr ExtractIcon(IntPtr hInst, string lpszExeFileName, int nIconIndex);

		[DllImport("user32.dll")]
		public static extern IntPtr GetDCEx(IntPtr hwnd, IntPtr hrgnclip, int fdwOptions);

		[DllImport("gdi32.dll")]
		private static extern int GetDCOrgEx(IntPtr hdc, out POINTAPI lpPoint);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
		public static extern int ScrollWindowEx(HandleRef hWnd,
		                                        int nXAmount, int nYAmount,
		                                        COMRECT rectScrollRegion, ref RECT rectClip,
		                                        HandleRef hrgnUpdate, ref RECT prcUpdate, int flags);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool GetScrollInfo(HandleRef hWnd, int fnBar, SCROLLINFO si);

		[DllImport("user32.dll", EntryPoint = "GetClassNameA", CharSet = CharSet.Ansi, SetLastError = true,
			ExactSpelling = true)]
		public static extern int GetClassName(IntPtr hwnd, StringBuilder className, int maxCount);

		[DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileStringA")]
		public static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault,
		                                                 StringBuilder lpReturnedString, int nSize, string lpFileName);

		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr GetWindowDC(IntPtr hwnd);

		[DllImport("user32.dll")]
		public static extern int GetWindowRect(IntPtr hwnd, out RECT lpRect);

		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern int GetWindowThreadProcessId(IntPtr hwnd, int ID);

		[DllImport("user32.dll", EntryPoint = "PostMessageA")]
		public static extern int PostMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

		[DllImport("user32.dll")]
		public static extern int RedrawWindow(IntPtr hwnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, int fuRedraw);

		[DllImport("user32.dll")]
		public static extern int RedrawWindow(IntPtr hwnd, ref RECT lprcUpdate, IntPtr hrgnUpdate, int fuRedraw);

		[DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern IntPtr ReleaseDC(IntPtr hwnd, IntPtr hdc);

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern int SetScrollInfo(HandleRef hWnd, int fnBar, [In, Out] SCROLLINFO si, bool redraw);

		[DllImport("user32.dll", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi, SetLastError = true,
			ExactSpelling = true)]
		public static extern IntPtr SetWindowsHookEx(int type, CallWndProc hook, IntPtr instance, int threadID);

		[DllImport("user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
		public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

		[DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileStringA")]
		public static extern int WritePrivateProfileString(string lpApplicationName, string lpKeyName, string lpString,
		                                                   string lpFileName);


		//open theme
		[DllImport("uxtheme.dll")]
		private static extern IntPtr OpenThemeData(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string classList);

		//close theme
		[DllImport("uxtheme.dll")]
		private static extern void CloseThemeData(IntPtr hTheme);

		//fill rectangle with theme
		[DllImport("uxtheme.dll")]
		private static extern void DrawThemeBackground(IntPtr hTheme, IntPtr hDC, int partId, int stateId, ref RECT rect,
		                                               ref RECT clipRect);

		//get theme color
		[DllImport("uxtheme.dll")]
		public static extern int GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, ref int pColor);

		#endregion

		#region constants

		public const int DCX_WINDOW = 0x1;
		public const int DCX_USESTYLE = 0x10000;
		public const int DCX_INTERSECTRGN = 0x80;
		public const int DCX_CACHE = 0x2;
		public const int DCX_CLIPSIBLINGS = 0x10;

		public const int HTCLIENT = 1;

		public const int RDW_FRAME = 0x400;

		public const int SW_ERASE = 0x4;
		public const int SW_INVALIDATE = 0x2;

		public const int SWP_NOSIZE = 0x1;
		public const int SWP_NOMOVE = 0x2;

		public const int WM_ACTIVATE = 0x6;
		public const int WM_ACTIVATEAPP = 0x1C;
		public const int WH_CALLWNDPROC = 4;
		public const int WM_CREATE = 1;
		public const int WM_DESTROY = 2;
		public const int WM_LBUTTONDOWN = 0x201;
		public const int WM_LBUTTONUP = 0x202;
		public const int WM_MBUTTONDOWN = 0x207;
		public const int WM_MOUSELEAVE = 0x2A3;
		public const int WM_MOUSEMOVE = 0x200;
		public const int WM_NCACTIVATE = 0x86;
		public const int WM_NCCALCSIZE = 0x83;
		public const int WM_NCHITTEST = 0x84;
		public const int WM_NCLBUTTONDOWN = 0xA1;
		public const int WM_NCMBUTTONDOWN = 0xA7;
		public const int WM_NCRBUTTONDOWN = 0xA4;
		public const int WM_NCPAINT = 0x85;
		public const int WM_PRINT = 0x317;
		public const int WM_RBUTTONDOWN = 0x204;
		public const int WM_SIZE = 0x5;
		public const int WM_SETTINGCHANGE = WM_WININICHANGE;
		public const int WM_THEMECHANGED = 0x31A;
		public const int WM_WINDOWPOSCHANGED = 0x47;
		public const int WM_WINDOWPOSCHANGING = 0x46;
		public const int WM_WININICHANGE = 0x1A;

		public const int WVR_HREDRAW = 0x100;
		public const int WVR_REDRAW = (WVR_HREDRAW | WVR_VREDRAW);
		public const int WVR_VREDRAW = 0x200;

		#endregion

		#region types

		[StructLayout(LayoutKind.Sequential)]
		public class COMRECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public COMRECT(int x, int y, int width, int height)
			{
				this.Left = x;
				this.Top = y;
				this.Right = x + width;
				this.Bottom = y + height;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct CWPSTRUCT
		{
			public IntPtr lparam;
			public IntPtr wparam;
			public int message;
			public IntPtr hwnd;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct NCCALCSIZE_PARAMS
		{
			public RECT newbounds;
			public RECT oldbounds;
			public RECT oldclientbounds;
			public IntPtr lppos;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct POINTAPI
		{
			public POINTAPI(Point pt)
			{
				this.x = pt.X;
				this.y = pt.Y;
			}

			internal int x;
			internal int y;

			public static implicit operator Point(POINTAPI pt)
			{
				return new Point(pt.x, pt.y);
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;

			public RECT(int x, int y, int width, int height)
			{
				this.Left = x;
				this.Top = y;
				this.Right = x + width;
				this.Bottom = y + height;
			}

			public void Inflate(int width, int height)
			{
				this.Left -= width;
				this.Right += width;
				this.Top -= height;
				this.Bottom += height;
			}

			public void Offset(int x, int y)
			{
				this.Left += x;
				this.Right += x;
				this.Top += y;
				this.Bottom += y;
			}

			#region operators

			public static implicit operator Rectangle(RECT rct)
			{
				return Rectangle.FromLTRB(rct.Left, rct.Top, rct.Right, rct.Bottom);
			}

			public static implicit operator RECT(Rectangle rct)
			{
				return new RECT(rct.X, rct.Y, rct.Width, rct.Height);
			}

			public static bool operator ==(RECT r1, RECT r2)
			{
				return r1.Left == r2.Left && r1.Top == r2.Top &&
				       r1.Right == r2.Right && r1.Bottom == r2.Bottom;
			}

			public static bool operator !=(RECT r1, RECT r2)
			{
				return !(r1 == r2);
			}

			public override bool Equals(object obj)
			{
				return ((RECT) obj) == this;
			}

			public override int GetHashCode()
			{
				return Left << 24 | Right << 16 | Top << 8 | Bottom;
			}

			#endregion

			#region properties

			public Size Size
			{
				get { return new Size(this.Width, this.Height); }
				set
				{
					this.Width = value.Width;
					this.Height = value.Height;
				}
			}

			/// <summary>
			/// width of rectangle
			/// </summary>
			public int Width
			{
				get { return Right - Left; }
				set { Right = Left + value; }
			}

			/// <summary>
			/// height of rectangle
			/// </summary>
			public int Height
			{
				get { return Bottom - Top; }
				set { Bottom = Top + value; }
			}

			#endregion
		}

		[StructLayout(LayoutKind.Sequential)]
		public class SCROLLINFO
		{
			public int cbSize;
			public int fMask;
			public int nMin;
			public int nMax;
			public int nPage;
			public int nPos;
			public int nTrackPos;

			public SCROLLINFO()
			{
				this.cbSize = Marshal.SizeOf(typeof (SCROLLINFO));
			}

			public SCROLLINFO(int mask, int min, int max, int page, int pos)
			{
				this.cbSize = Marshal.SizeOf(typeof (SCROLLINFO));
				this.fMask = mask;
				this.nMin = min;
				this.nMax = max;
				this.nPage = page;
				this.nPos = pos;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct WINDOWPOS
		{
			internal int hwnd;
			internal int hWndInsertAfter;
			internal int x;
			internal int y;
			internal int cx;
			internal int cy;
			internal int flags;
		}

		#endregion

		#region user functions

		/// <summary>
		/// maps the given point on the given graphics to the screen
		/// </summary>
		public static Point ClientToScreen(Graphics gr, Point pt)
		{
			if (gr == null) return Point.Empty;
			IntPtr hdc = gr.GetHdc();
			POINTAPI org;
			if (GetDCOrgEx(hdc, out org) != 0) {
				pt.X += org.x;
				pt.Y += org.y;
			}
			gr.ReleaseHdc(hdc);
			return pt;
		}

		/// <summary>
		/// maps the given point to the specified graphics
		/// </summary>
		public static Point ScreenToClient(Graphics gr, Point pt)
		{
			if (gr == null) return Point.Empty;
			IntPtr hdc = gr.GetHdc();
			POINTAPI org;
			if (GetDCOrgEx(hdc, out org) != 0) {
				pt.X -= org.x;
				pt.Y -= org.y;
			}
			gr.ReleaseHdc(hdc);
			return pt;
		}

		/// <summary>
		/// opens a winxp theme for drawing into a dc
		/// </summary>
		public static IntPtr OpenThemeData2(IntPtr hWnd, [MarshalAs(UnmanagedType.LPTStr)] string classList)
		{
			try {
				return OpenThemeData(hWnd, classList);
			}
			catch {
				return IntPtr.Zero;
			}
		}

		/// <summary>
		/// closes the specified theme
		/// </summary>
		public static void CloseThemeData2(IntPtr hTheme)
		{
			try {
				CloseThemeData(hTheme);
			}
			catch {
				return;
			}
		}

		/// <summary>
		/// fills a rectangle with the specified theme
		/// </summary>
		public static void DrawThemeBackground2(IntPtr hTheme, IntPtr hDC, int partId, int stateId, ref RECT rect)
		{
			try {
				DrawThemeBackground(hTheme, hDC, partId, stateId, ref rect, ref rect);
			}
			catch {
				return;
			}
		}

		#endregion

		#region user types

		/// <summary>
		/// Double Buffered Graphics class
		/// </summary>
		public class NCGraphics : IDisposable
		{
			#region variables

			private Graphics _targetgraphics,
			                 _memgraphics;

			private Bitmap _membitmap;

			#endregion

			public NCGraphics(IntPtr hdc)
			{
				this._targetgraphics = Graphics.FromHdc(hdc);
				Size size = Size.Ceiling(_targetgraphics.VisibleClipBounds.Size);
				this._membitmap = new Bitmap(size.Width, size.Height);
				this._memgraphics = Graphics.FromImage(this._membitmap);
			}

			public void Dispose()
			{
				//render
				this._targetgraphics.DrawImageUnscaled(this._membitmap, Point.Empty);
				//dispose
				this._memgraphics.Dispose();
				this._membitmap.Dispose();
				this._targetgraphics.Dispose();
			}

			/// <summary>
			/// gets the graphics buffer to render on
			/// </summary>
			public Graphics Graphics
			{
				get { return this._memgraphics; }
			}
		}

		#endregion

		#region delegates

		public delegate int CallWndProc(int code, IntPtr wparam, ref CWPSTRUCT cwp);

		#endregion
	}
}
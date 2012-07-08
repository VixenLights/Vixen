using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// Double Buffered Graphics class
	/// </summary>
	internal class NCGraphics:IDisposable
	{
		#region variables
		private Graphics _targetgraphics,
			_memgraphics;
		private Bitmap _membitmap;
		#endregion
		public NCGraphics(IntPtr hdc, Size size)
		{
			this._targetgraphics = Graphics.FromHdc(hdc);
			this._membitmap = new Bitmap(size.Width, size.Height);
			this._memgraphics = Graphics.FromImage(this._membitmap);
		}
		public void Dispose()
		{
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
		/// <summary>
		/// renders the content of the buffer to the target
		/// </summary>
		public void Render()
		{
			this._targetgraphics.DrawImageUnscaled(this._membitmap, Point.Empty);
		}
	}
}

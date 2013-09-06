using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Vixen.Data.Value;
using Vixen.Execution.Context;
using Vixen.Sys;
using VixenModules.Preview.VixenPreview.Shapes;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using VixenModules.Property.Location;

namespace VixenModules.Preview.VixenPreview
{
	public partial class GDIControl : UserControl
	{
		private Image _background = null;
		private Bitmap _backgroundAlphaImage = null;
		private int _backgroundAlpha = 0;
		private Stopwatch renderTimer = new Stopwatch();
		private FastPixel.FastPixel fastPixel;
		BufferedGraphicsContext graphicsContext;
		BufferedGraphics backBuffer;

		public GDIControl()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);

			graphicsContext = BufferedGraphicsManager.Current;
			AllocateGraphicsBuffer();
		}

		public Image Background
		{
			get
			{
				return _background;
			}
			set
			{
				_background = value;
				CreateAlphaBackground();
			}
		}

		public int BackgroundAlpha 
		{
			get
			{
				return _backgroundAlpha;
			}
			set
			{
				_backgroundAlpha = value;
				if (_background != null) CreateAlphaBackground();
			}
		}

		public long RenderTime 
		{ 
			get
			{
				return renderTimer.ElapsedMilliseconds;
			}
		}

		public long FrameRate { get; set; }

		public FastPixel.FastPixel FastPixel
		{
			get
			{
				return fastPixel;
			}
			set
			{
				fastPixel = value;
			}
		}

		public bool IsUpdating
		{
			get { return FastPixel.locked; }
		}

		public void CreateAlphaBackground()
		{
			if (_background != null)
			{
				_backgroundAlphaImage = new Bitmap(_background.Width, _background.Height, PixelFormat.Format32bppPArgb);
				Graphics gfx = Graphics.FromImage(_backgroundAlphaImage);
				using (SolidBrush brush = new SolidBrush(Color.FromArgb(255 - BackgroundAlpha, 0, 0, 0)))
				{
					//gfx.DrawImageUnscaled(_background, 0, 0, _background.Width, _background.Height);
					gfx.DrawImage(_background, 0, 0, _background.Width, _background.Height);
					gfx.FillRectangle(brush, 0, 0, _backgroundAlphaImage.Width, _backgroundAlphaImage.Height);
				}
				gfx.Dispose();
			}
			else
			{
				_backgroundAlphaImage = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
				Graphics gfx = Graphics.FromImage(_backgroundAlphaImage);
				gfx.Clear(Color.DeepPink);
				gfx.Dispose();
			}
			fastPixel = new FastPixel.FastPixel(_backgroundAlphaImage.Width, _backgroundAlphaImage.Height);
		}

		private void GDIControl_Resize(object sender, EventArgs e)
		{
			AllocateGraphicsBuffer();
		}

		public void RenderImage()
		{
			if (_backgroundAlphaImage != null)
			{

				backBuffer.Graphics.DrawImageUnscaled(fastPixel.Bitmap, 0, 0);
			}
			else
			{
				backBuffer.Graphics.Clear(Color.Black);
			}

			if (!this.Disposing && graphicsContext != null)
				backBuffer.Render(Graphics.FromHwnd(this.Handle));

			renderTimer.Stop();
		}

		private void AllocateGraphicsBuffer()
		{
			if (backBuffer != null)
				backBuffer.Dispose();

			graphicsContext.MaximumBuffer =
				  new Size(this.Width + 1, this.Height + 1);
			if (this.Width > 0 && this.Height > 0)
			{
				backBuffer =
					graphicsContext.Allocate(this.CreateGraphics(),
													ClientRectangle);
			}
		}

		public void BeginUpdate()
		{
			renderTimer.Restart();
			fastPixel.CloneBitmap(_backgroundAlphaImage);
			fastPixel.Lock();
		}

		private DateTime frameRateTime;
		private long frameCount = 0;
		public void EndUpdate()
		{
			// Calculate our actual frame rate
			this.frameCount++;

			if (DateTime.UtcNow.Subtract(this.frameRateTime).TotalSeconds >= 1)
			{
				this.FrameRate = frameCount;
				this.frameCount = 0;
				this.frameRateTime = DateTime.UtcNow;
			}

			fastPixel.Unlock(true);
		}

		public void SetPixel(int x, int y, Color color)
		{
			fastPixel.SetPixel(new Point(x, y), color);
		}

	}
}

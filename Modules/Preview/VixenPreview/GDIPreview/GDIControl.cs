using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview
{
	public partial class GDIControl : UserControl
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		private Image _background;
		private Bitmap _backgroundAlphaImage;
		private int _backgroundAlpha;
		private readonly Stopwatch _renderTimer = new Stopwatch();
		private FastPixel.FastPixel _fastPixel;
		private DateTime _frameRateTime;
		private long _frameCount;

		public GDIControl()
		{
			InitializeComponent();
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint,true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer,true);
			SetStyle(ControlStyles.ResizeRedraw, true);
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
				return _renderTimer.ElapsedMilliseconds;
			}
		}

		public long FrameRate { get; set; }

		public FastPixel.FastPixel FastPixel
		{
			get
			{
				return _fastPixel;
			}
			set
			{
				_fastPixel = value;
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
					gfx.DrawImage(_background, 0, 0, _background.Width, _background.Height);
					gfx.FillRectangle(brush, 0, 0, _backgroundAlphaImage.Width, _backgroundAlphaImage.Height);
				}
				gfx.Dispose();
			}
			else
			{
				_backgroundAlphaImage = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
				Graphics gfx = Graphics.FromImage(_backgroundAlphaImage);
				gfx.Clear(Color.Black);
				gfx.Dispose();
			}
			_fastPixel = new FastPixel.FastPixel(_backgroundAlphaImage.Width, _backgroundAlphaImage.Height);
		}

		public void BeginUpdate()
		{
			_renderTimer.Restart();
			_fastPixel.CloneToBuffer(_backgroundAlphaImage);
		}

		
		public void EndUpdate()
		{
			// Calculate our actual frame rate
			_frameCount++;

			if (DateTime.UtcNow.Subtract(_frameRateTime).TotalSeconds >= 1)
			{
				FrameRate = _frameCount;
				_frameCount = 0;
				_frameRateTime = DateTime.UtcNow;
			}

			_fastPixel.UnlockFromBuffer();
		}

		public void SetPixel(int x, int y, Color color)
		{
			_fastPixel.SetPixel(new Point(x, y), color);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (g.VisibleClipBounds.Width > 0 && g.VisibleClipBounds.Height > 0)
			{
				g.DrawImageUnscaled(_fastPixel.Bitmap, 0, 0);
			}
			_renderTimer.Stop();
		}


	}
}

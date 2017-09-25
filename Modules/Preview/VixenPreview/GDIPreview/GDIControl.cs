using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace VixenModules.Preview.VixenPreview.GDIPreview
{
	public partial class GDIControl : UserControl
	{
		private Image _background;
		private Bitmap _backgroundAlphaImage;
		private int _backgroundAlpha;
		private readonly Stopwatch _renderTimer = new Stopwatch();
		private FastPixel.FastPixel _fastPixel;
		private DateTime _frameRateTime;
		private long _frameCount;
        private bool _defaultBackground = true;

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
                if (value == null)
                {
                    _defaultBackground = true;
                    _background = new Bitmap(Width>0?Width:1, Height>0?Height:1, PixelFormat.Format32bppPArgb);

                    Graphics gfx = Graphics.FromImage(_background);
                    gfx.Clear(Color.Black);
                    gfx.Dispose();
                }
                else
                {
                    _defaultBackground = false;
                    _background = value;
                }
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
				if (Background != null) CreateAlphaBackground();
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
			if (Width <= 0 || Height <= 0)
			{
				return;
			}
			if (Background != null)
			{
                Color c;
                c = Color.FromArgb(255 - BackgroundAlpha, 0, 0, 0);

                //_backgroundAlphaImage = new Bitmap(Background.Width, Background.Height, PixelFormat.Format32bppPArgb);
                _backgroundAlphaImage = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
                Graphics gfx = Graphics.FromImage(_backgroundAlphaImage);
                using (SolidBrush brush = new SolidBrush(c))
                {
                    gfx.FillRectangle(Brushes.Black, 0, 0, _backgroundAlphaImage.Width, _backgroundAlphaImage.Height);
                    gfx.DrawImage(Background, 0, 0, Background.Width, Background.Height);
                    gfx.FillRectangle(brush, 0, 0, Background.Width, Background.Height);
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

        private void GDIControl_Resize(object sender, EventArgs e)
        {
            if (Width > 0 && Height > 0)
            {
                if (_defaultBackground)
                {
                    Background = null;
                }
                else
                {
                    CreateAlphaBackground();
                }
                //_fastPixel.CloneToBuffer(_backgroundAlphaImage);
                BeginUpdate();
                EndUpdate();
            }
        }
	}
}

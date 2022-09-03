using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using VixenModules.Preview.VixenPreview.Shapes;

namespace VixenModules.Preview.VixenPreview.GDIPreview
{
	public partial class GDIControl : UserControl
	{		
		private Image _background;
		private Bitmap _backgroundAlphaImage;
		private int _backgroundAlpha;
		private readonly Stopwatch _renderTimer = new Stopwatch();
		private readonly Stopwatch _frameRateTimer = new Stopwatch();
						
		private FastPixel.FastPixel _fastPixel;
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
			_frameRateTimer.Start();
		}

		/// <summary>
		/// Static Constructor
		/// </summary>
		static GDIControl()
		{
			// Create a lock to prevent threading issues with the FastPixel frame buffer
			FastPixelLock = new object();
		}

		/// <summary>
		/// Display items that make up the preview.
		/// </summary>
		public List<DisplayItem> DisplayItems { get; set; }

		/// <summary>
		/// Lock to prevent threading issues with the FastPixel frame buffer.
		/// </summary>
		public static object FastPixelLock { get;  set; }
		
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

		public Size BackgroundSize => new Size((int)(_background.Width*ZoomLevel), (int)(_background.Height*ZoomLevel));

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

		public double ZoomLevel { get; set; } = 1;

		public void CreateAlphaBackground()
		{
			if (Width <= 0 || Height <= 0)
			{
				return;
			}

			int newWidth = Convert.ToInt32(_background.Width * ZoomLevel);
			int newHeight = Convert.ToInt32(_background.Height * ZoomLevel);

			if (newWidth <= 0 || newHeight <= 0)
			{
				return;
			}
			
			if (Background != null)
			{
				Color c = Color.FromArgb(255 - BackgroundAlpha, 0, 0, 0);

                //_backgroundAlphaImage = new Bitmap(Background.Width, Background.Height, PixelFormat.Format32bppPArgb);
                _backgroundAlphaImage = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);
				Graphics gfx = Graphics.FromImage(_backgroundAlphaImage);
				using (SolidBrush brush = new SolidBrush(c))
				{
					gfx.FillRectangle(Brushes.Black, 0, 0, Width, Height);
					gfx.DrawImage(Background, 0, 0, newWidth, newHeight);
					gfx.FillRectangle(brush, 0, 0, Width, Height);
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
			{
				lock (FastPixelLock)
				{
					_fastPixel = new FastPixel.FastPixel(_backgroundAlphaImage.Width, _backgroundAlphaImage.Height);
					BeginUpdate();
					EndUpdate();				
				}
			}

			Invalidate();
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

			if (_frameRateTimer.ElapsedMilliseconds > 999)
			{
				FrameRate = _frameCount;
				_frameCount = 0;
				_frameRateTimer.Restart();
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
				// Rendering of the WPF moving heads in the GDI was not performant.
				// This capability exists in debug for experimentation purposes and to keep the WPF moving head implementations complete.				
				#if DEBUG
					// Draw the moving head fixtures
					DrawMovingHeads();
				#endif

				g.DrawImageUnscaled(_fastPixel.Bitmap, 0, 0);
			}
			_renderTimer.Stop();
		}

		/// <summary>
		/// Draws the moving head fixtures.
		/// </summary>
		private void DrawMovingHeads()
		{
			// If the moving head GDI support is enabled then...
			if (false)
			{
				// Wait for the frame buffer to be available
				lock (FastPixelLock)
				{
					// If the frame buffer is not already locked then...
					bool locking = false;
					if (!_fastPixel.locked)
					{
						// Lock the frame buffer
						_fastPixel.Lock();

						// Remember that this method locked the frame buffer
						locking = true;
					}

					// Draw the shapes that support IDrawStaticPreviewShape 
					UpdateMovingHeads();

					// If this method locked the frame buffer then...
					if (locking)
					{
						// Unlock the frame buffer
						_fastPixel.Unlock(true);
					}			
				}
			}
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
			
				lock (FastPixelLock)
				{
					BeginUpdate();
					EndUpdate();
				}				
			}
        }

		/// <summary>
		/// Reeturns the display item shapes that implement <c>IDrawMovingHeadVolumes</c>.
		/// </summary>
		/// <returns></returns>
		private IEnumerable<IDrawStaticPreviewShape> GetIDrawStaticPreviewShapes()
		{
			// Get the display item shapes that implement IDrawMovingHeadVolumes
			return DisplayItems.Where(displayItem => displayItem.Shape is IDrawStaticPreviewShape)
				.Select(displayItem => displayItem.Shape)
					.Cast<IDrawStaticPreviewShape>().ToList();
		}

		/// <summary>
		/// Updates the moving heads.
		/// </summary>
		private void UpdateMovingHeads()
		{			
			// If the display items have been set on the control then...
			if (DisplayItems != null)
			{
				// Loop over all the shapes that implement IDrawStaticPreviewShape
				foreach (IDrawStaticPreviewShape shape in GetIDrawStaticPreviewShapes())
				{
					// Draw the shape using the GDI
					shape.DrawGDI(_fastPixel, false, false, ZoomLevel);
				}
			}
		}
	}
}

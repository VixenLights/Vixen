using System.ComponentModel;
using System.Drawing.Drawing2D;
using Common.Controls.ColorManagement.ColorModels;

namespace Common.Controls.ColorManagement.ColorPicker
{
	/// <summary>
	/// Zusammenfassung f�r ColorSelectionPlane.
	/// </summary>
	public class ColorSelectionPlane : Control
	{
		#region variables

		private Bitmap _bmp = new Bitmap(1, 1);
		private double _x = 0.0, _y = 0.0;

		#endregion

		/// <summary>
		/// ctor
		/// </summary>
		[ToolboxItem(false)]
		public ColorSelectionPlane()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.ResizeRedraw, true);
			base.Cursor = Cursors.Cross;
//			try
//			{
//				base.Cursor=new Cursor(this.GetType(),"ColorPicker.cur");
//			}
//			catch{}
		}

		#region helpers

		private Rectangle GetCursorBounds(double x, double y)
		{
			return new Rectangle(
				(int) (x*(Width - 3)) - 4,
				(int) (y*(Height - 3)) - 4,
				10, 10);
		}

		#endregion

		#region controller

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			//construct new bitmap
			if (_bmp != null)
				_bmp.Dispose();
			_bmp = new Bitmap(Math.Max(1, Width - 2), Math.Max(1, Height - 2),
			                  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			OnImageChanged();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_bmp == null) return;
			e.Graphics.DrawImageUnscaled(_bmp, 1, 1);
			e.Graphics.SmoothingMode =
				SmoothingMode.AntiAlias;
			//draw cursor ring
			Rectangle crs = GetCursorBounds(_x, _y);
			if ((crs.X + 4) >= 0 && (crs.X + 4) < _bmp.Width &&
			    (crs.Y + 4) >= 0 && (crs.Y + 4) < _bmp.Height) {
				Color col = _bmp.GetPixel(crs.X + 4, crs.Y + 4);
				if (col.R + col.G + col.B > 381)
					e.Graphics.DrawEllipse(Pens.Black, GetCursorBounds(_x, _y));
				else
					e.Graphics.DrawEllipse(Pens.White, GetCursorBounds(_x, _y));
			}
			if ((ModifierKeys & Keys.Shift) != 0) {
				float dx = (Width - 3)/8f,
				      dy = (Height - 3)/8f,
				      x = 1f,
				      y = 1f;
				using (Pen pn = new Pen(new HatchBrush(HatchStyle.SmallCheckerBoard,
				                                       Color.FromArgb(80, 255, 255, 255),
				                                       Color.FromArgb(0, 0, 0, 0)))) {
					for (int i = 0; i <= 8; i++,x += dx,y += dy) {
						e.Graphics.DrawLine(pn, 0, (int) y, Width, (int) y);
						e.Graphics.DrawLine(pn, (int) x, 0, (int) x, Height);
					}
				}
			}
			e.Graphics.DrawRectangle(Pens.Silver, 0, 0, Width - 1, Height - 1);
		}

		//mouse
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (SetPosition(
				e.X/Math.Max(1.0, Width - 2.0),
				e.Y/Math.Max(1.0, Height - 2.0)))
				RaiseScroll();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button != MouseButtons.None)
				if (SetPosition(
					e.X/Math.Max(1.0, Width - 2.0),
					e.Y/Math.Max(1.0, Height - 2.0)))
					RaiseScroll();
		}

		#endregion

		#region members

		public bool SetPosition(double posx, double posy)
		{
			posx = XYZ.ClipValue(posx, 0.0, 1.0);
			posy = XYZ.ClipValue(posy, 0.0, 1.0);
			if ((ModifierKeys & Keys.Shift) != 0) {
				posx = Math.Round(posx*8.0, 0)/8.0;
				posy = Math.Round(posy*8.0, 0)/8.0;
			}
			if (posx == _x && posy == _y) return false;
			Invalidate(Rectangle.Inflate(GetCursorBounds(_x, _y), 1, 1));
			_x = posx;
			_y = posy;
			Invalidate(Rectangle.Inflate(GetCursorBounds(_x, _y), 1, 1));
			Update();
			return true;
		}

		#endregion

		#region properties

		[Browsable(false),
		 DefaultValue(null),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Cursor Cursor
		{
			get { return base.Cursor; }
			set { return; }
		}

		internal Bitmap Image
		{
			get { return _bmp; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal double PositionX
		{
			get { return _x; }
			set { SetPosition(value, _y); }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal double PositionY
		{
			get { return _y; }
			set { SetPosition(_x, value); }
		}

		#endregion

		#region events

		public EventHandler ImageSizeChanged;

		private void OnImageChanged()
		{
			if (ImageSizeChanged != null)
			{
				ImageSizeChanged(this, EventArgs.Empty);
			}
		}
		private void RaiseScroll()
		{
			if (Scroll != null)
				Scroll(this, EventArgs.Empty);
		}

		internal event EventHandler Scroll;

		#endregion
	}
}
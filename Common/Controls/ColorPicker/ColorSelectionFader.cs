using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;

namespace Common.Controls.ColorManagement.ColorPicker
{
	/// <summary>
	/// Zusammenfassung für ColorSelectionFader.
	/// </summary>
	[System.ComponentModel.ToolboxItem(false)]
	public class ColorSelectionFader : Control
	{
		#region variables

		private Bitmap _bmp = new Bitmap(1, 1);
		private double _position = 1.0;

		#endregion

		public ColorSelectionFader()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.ResizeRedraw, true);
		}

		private Rectangle GetScrollerRectangle(double pos)
		{
			return new Rectangle(
				0, (int) (_position*(double) (this.Height - 11)),
				this.Width - 1, 10);
		}

		#region controller

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (_bmp != null)
				_bmp.Dispose();
			_bmp = new Bitmap(Math.Max(1, this.Height - 10), 1,
			                  System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			OnImageChanged();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (_bmp == null || this.Width < 11 || this.Height < 11) return;

			GraphicsState state = e.Graphics.Save();
			e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			e.Graphics.Transform = new Matrix(0f, 1f, 1f, 0f, 0f, 0f);
			e.Graphics.DrawImage(_bmp, 5, 5, this.Height - 11, this.Width - 10);

			e.Graphics.Restore(state);
			//draw gridlines
			if ((Control.ModifierKeys & Keys.Shift) != 0) {
				float dy = (float) (this.Height - 11)/8f, y = 5f;
				using (Pen pn = new Pen(new HatchBrush(HatchStyle.SmallCheckerBoard,
				                                       Color.FromArgb(80, 255, 255, 255),
				                                       Color.FromArgb(0, 0, 0, 0)))) {
					for (int i = 0; i <= 8; i++,y += dy) {
						e.Graphics.DrawLine(pn, 5, (int) y, this.Width - 6, (int) y);
					}
				}
			}
			e.Graphics.DrawRectangle(Pens.Silver, 5, 5, this.Width - 11, this.Height - 11);
			//draw fader
			Rectangle fader = GetScrollerRectangle(_position);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Point[] points =
				new Point[]
					{
						new Point(fader.X + 1, fader.Y), new Point(fader.X + 4, fader.Y),
						new Point(fader.X + 9, fader.Y + 5), new Point(fader.X + 4, fader.Bottom),
						new Point(fader.X + 1, fader.Bottom), new Point(fader.X, fader.Bottom - 1),
						new Point(fader.X, fader.Y + 1)
					};
			e.Graphics.FillPolygon(Brushes.White, points);
			e.Graphics.DrawPolygon(Pens.Silver, points);
			points =
				new Point[]
					{
						new Point(fader.Right - 1, fader.Y), new Point(fader.Right - 4, fader.Y),
						new Point(fader.Right - 9, fader.Y + 5), new Point(fader.Right - 4, fader.Bottom),
						new Point(fader.Right - 1, fader.Bottom), new Point(fader.Right, fader.Bottom - 1),
						new Point(fader.Right, fader.Y + 1)
					};

			e.Graphics.FillPolygon(Brushes.White, points);
			e.Graphics.DrawPolygon(Pens.Silver, points);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (SetPosition(
				(double) (e.Y - 5)/(double) Math.Max(1, this.Height - 11)))
				RaiseScroll();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Left)
				if (SetPosition(
					(double) (e.Y - 5)/(double) Math.Max(1, this.Height - 11)))
					RaiseScroll();
		}

		#endregion

		#region members

		public bool SetPosition(double value)
		{
			value = XYZ.ClipValue(value, 0.0, 1.0);
			if ((Control.ModifierKeys & Keys.Shift) != 0) {
				value = Math.Round(value*8.0, 0)/8.0;
			}
			if (value == _position) return false;
			this.Invalidate(Rectangle.Inflate(GetScrollerRectangle(_position), 1, 1));
			_position = value;
			this.Invalidate(Rectangle.Inflate(GetScrollerRectangle(_position), 1, 1));
			this.Update();
			return true;
		}

		#endregion

		internal Bitmap Image
		{
			get { return _bmp; }
		}

		internal double Position
		{
			get { return _position; }
			set { SetPosition(value); }
		}

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

		public event EventHandler Scroll;

		#endregion
	}
}
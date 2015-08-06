using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Button = System.Windows.Controls.Button;
using MouseEventHandler = System.Windows.Input.MouseEventHandler;

namespace Common.Controls.Theme
{
	class ThemeButtonRenderer : Button
	{
		#region Fields

		private Color _StartColor;
		private Color _EndColor;
		private Color _StartHoverColor;
		private Color _EndHoverColor;
		private bool bMouseHover;
		private Brush _paintBrush;
		private PointF _centerPoint;
		private StringFormat _sf = new StringFormat();

		#endregion

		#region Properties

		public Color StartColor
		{
			get { return _StartColor; }
			set
			{
				if (value == Color.Empty)
				{
					_StartColor = Color.FromArgb(251, 250, 249);
				}
				else
				{
					_StartColor = value;
				}
			}
		}

		public Color StartHoverColor
		{
			get { return _StartHoverColor; }
			set
			{
				if (value == Color.Empty)
				{
					_StartHoverColor = Color.White;
				}
				else
				{
					_StartHoverColor = value;
				}
			}
		}

		public Color EndHoverColor
		{
			get { return _EndHoverColor; }
			set
			{
				if (value == Color.Empty)
				{
					_EndHoverColor = Color.FromArgb(255, 255, 207);
				}
				else
				{
					_EndHoverColor = value;
				}
			}
		}

		public Color EndColor
		{
			get { return _EndColor; }
			set
			{
				if (value == Color.Empty)
				{
					_EndColor = Color.FromArgb(224, 220, 207);
				}
				else
				{
					_EndColor = value;
				}
			}
		}

		#endregion

		#region Constructor

		public ThemeButtonRenderer()
		{
			InitializeComponent();
			bMouseHover = false;
			_sf.Alignment = StringAlignment.Center;
			_sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
		}

		#endregion

		#region Methods

		private void OnMouseEnter(object sender, System.EventArgs e)
		{
			bMouseHover = true;
			
		//	Invalidate();
		}

		private void OnMouseLeave(object sender, System.EventArgs e)
		{
			bMouseHover = false;
		//	Invalidate();
		}

		private void InitializeComponent()
		{
			this.MouseEnter += new MouseEventHandler(this.OnMouseEnter);
			this.MouseLeave += new MouseEventHandler(this.OnMouseLeave);
		}

		protected void OnPaint(PaintEventArgs pevent)
		{
			base.OnRender(null);//OnPaint(pevent);
			Graphics g = pevent.Graphics;
			if (bMouseHover == true)
			{
				_paintBrush = new LinearGradientBrush(this.ClientRectangle, this.StartHoverColor, this.EndHoverColor,
					LinearGradientMode.Vertical);
			}
			else
			{
				_paintBrush = new LinearGradientBrush(this.ClientRectangle, this.StartColor, this.EndColor,
					LinearGradientMode.Vertical);
			}
			g.FillRectangle(_paintBrush, this.ClientRectangle);
			_paintBrush = new SolidBrush(this.ForeColor);
//this._centerPoint = new PointF((this.ClientRectangle.Left + this.ClientRectangle.Right) / 2,
// (this.ClientRectangle.Top + this.ClientRectangle.Bottom) / 2);
			this._centerPoint = new PointF((float) (this.Width/2), (float) (this.Height/2));
			g.DrawString(this.Text, this.Font, _paintBrush, _centerPoint.X, _centerPoint.Y - 5, _sf);
			paint_Border(pevent);
		}

		public Font Font { get; set; }

		public string Text { get; set; }

		public Color ForeColor { get; set; }

		private void paint_Border(PaintEventArgs e)
		{
			if (e == null)
				return;
			if (e.Graphics == null)
				return;
			Pen pen = new Pen(this.ForeColor, 1);
			Point[] pts = border_Get(0, 0, this.Width - 1, this.Height - 1);
			e.Graphics.DrawLines(pen, pts);
			pen.Dispose();
		}

		private Point[] border_Get(int nLeftEdge, int nTopEdge, double nWidth, double nHeight)
		{
			int X = (int) nWidth;
			int Y = (int) nHeight;
			Point[] points =
			{
				new Point(1, 0),
				new Point(X - 1, 0),
				new Point(X - 1, 1),
				new Point(X, 1),
				new Point(X, Y - 1),
				new Point(X - 1, Y - 1),
				new Point(X - 1, Y),
				new Point(1, Y),
				new Point(1, Y - 1),
				new Point(0, Y - 1),
				new Point(0, 1),
				new Point(1, 1)
			};
			for (int i = 0; i < points.Length; i++)
			{
				points[i].Offset(nLeftEdge, nTopEdge);
			}
			return points;
		}

		#endregion

		public Rectangle ClientRectangle { get; set; }
	}
}

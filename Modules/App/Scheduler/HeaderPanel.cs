using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.Scheduler {
	public partial class HeaderPanel : UserControl {
		//private Font _font = new Font("Arial", 12, FontStyle.Bold);
		
		private Color _headerGradientStart = Color.FromArgb(89, 135, 214);
		private Color _headerGradientEnd = Color.FromArgb(4, 57, 148);
		private Color _hoverGradientStart = Color.FromArgb(119, 165, 214);
		private Color _hoverGradientEnd = Color.FromArgb(24, 77, 148);

		private bool _inLeftButtonBounds = false;
		private bool _inRightButtonBounds = false;
		private Rectangle _buttonLeftBounds;
		private Rectangle _buttonRightBounds;

		public HeaderPanel() {
			InitializeComponent();
			Font = new Font("Arial", 12, FontStyle.Bold);
		}

		[DefaultValue(typeof(Color), "89, 135, 214")]
		public Color HeaderGradientStart {
			get { return _headerGradientStart; }
			set {
				_headerGradientStart = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "4, 57, 148")]
		public Color HeaderGradientEnd {
			get { return _headerGradientEnd; }
			set {
				_headerGradientEnd = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "119, 165, 214")]
		public Color HoverGradientStart {
			get { return _hoverGradientStart; }
			set {
				_hoverGradientStart = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "24, 77, 148")]
		public Color HoverGradientEnd {
			get { return _hoverGradientEnd; }
			set {
				_hoverGradientEnd = value;
				Invalidate();
			}
		}

		protected override void OnResize(EventArgs e) {
			Invalidate();
			base.OnResize(e);
		}

		protected override void OnPaint(PaintEventArgs e) {
			using(LinearGradientBrush headerGradientBrush = new LinearGradientBrush(ClientRectangle, HeaderGradientStart, HeaderGradientEnd, 90)) {
				e.Graphics.FillRectangle(headerGradientBrush, ClientRectangle);
			}
			_DrawHeaderButtons(e.Graphics, _inLeftButtonBounds, _inRightButtonBounds);

			// Current day
			e.Graphics.DrawString(DateTime.Today.ToLongDateString(), Font, Brushes.White, 10, 5);
		}

		protected virtual void _DrawHeaderButtons(Graphics g, bool hoverLeft, bool hoverRight) {
			int buttonWidth = 18;
			int buttonSpace = 8;

			// Anchored to the right
			int x = Width - (buttonWidth + buttonSpace + buttonWidth + buttonSpace);
			// Anchored to the top
			int y = (Height - buttonWidth) / 2;

			_buttonLeftBounds = new Rectangle(x, y, buttonWidth, buttonWidth);
			_buttonRightBounds = new Rectangle(x + buttonWidth + buttonSpace, y, buttonWidth, buttonWidth);

			Point[] leftButtonPoints = new Point[] {
		        new Point(x+12, y+5),
		        new Point(x+12, y+13),
		        new Point(x+6, y+9)
		    };

			Point[] rightButtonPoints = new Point[] {
		        new Point(x+6+buttonWidth+buttonSpace, y+5),
		        new Point(x+6+buttonWidth+buttonSpace, y+13),
		        new Point(x+12+buttonWidth+buttonSpace, y+9)
		    };

			using(LinearGradientBrush flatGradientBrush = new LinearGradientBrush(new Rectangle(x + 1, y + 1, buttonWidth - 1, Height), HeaderGradientStart, HeaderGradientEnd, 90),
									  hoverGradientBrush = new LinearGradientBrush(new Rectangle(x + 1, y + 1, buttonWidth - 1, buttonWidth - 1), HoverGradientStart, HoverGradientEnd, 90)) {
				g.FillRectangle(hoverLeft ? hoverGradientBrush : flatGradientBrush, x + 1, y + 1, buttonWidth - 1, buttonWidth - 1);
				g.FillRectangle(hoverRight ? hoverGradientBrush : flatGradientBrush, x + 1 + buttonWidth + buttonSpace, y + 1, buttonWidth - 1, buttonWidth - 1);
			}

			_DrawRoundRect(g, Pens.White, x, y, buttonWidth, buttonWidth, 3);
			_DrawRoundRect(g, Pens.White, x + buttonWidth + buttonSpace, y, buttonWidth, buttonWidth, 3);

			g.FillPolygon(Brushes.White, leftButtonPoints);
			g.FillPolygon(Brushes.White, rightButtonPoints);
		}

		private void _DrawRoundRect(Graphics g, Pen p, float X, float Y, float width, float height, float radius) {
			using(GraphicsPath gp = new GraphicsPath()) {
				gp.AddLine(X + radius, Y, X + width - (radius * 2), Y);
				gp.AddArc(X + width - (radius * 2), Y, radius * 2, radius * 2, 270, 90);
				gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius * 2));
				gp.AddArc(X + width - (radius * 2), Y + height - (radius * 2), radius * 2, radius * 2, 0, 90);
				gp.AddLine(X + width - (radius * 2), Y + height, X + radius, Y + height);
				gp.AddArc(X, Y + height - (radius * 2), radius * 2, radius * 2, 90, 90);
				gp.AddLine(X, Y + height - (radius * 2), X, Y + radius);
				gp.AddArc(X, Y, radius * 2, radius * 2, 180, 90);
				gp.CloseFigure();

				g.DrawPath(p, gp);
			}
		}


	}
}

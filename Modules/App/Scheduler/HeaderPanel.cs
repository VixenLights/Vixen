using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.App.Scheduler
{
	public partial class HeaderPanel : UserControl
	{
		private Color _headerGradientStart = Color.FromArgb(89, 135, 214);
		private Color _headerGradientEnd = Color.FromArgb(4, 57, 148);
		private Color _hoverGradientStart = Color.FromArgb(119, 165, 214);
		private Color _hoverGradientEnd = Color.FromArgb(24, 77, 148);
		private Color _borderColor = Color.Transparent;

		private bool _showButtons = true;
		private bool _inLeftButtonBounds = false;
		private bool _inRightButtonBounds = false;
		private Rectangle _buttonLeftBounds;
		private Rectangle _buttonRightBounds;
		private int _buttonWidth = 18;
		private int _buttonSpace = 8;

		public event EventHandler LeftButtonClick;
		public event EventHandler RightButtonClick;

		public HeaderPanel()
		{
			InitializeComponent();
			Font = new Font("Arial", 12, FontStyle.Bold);

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		private void HeaderPanel_Load(object sender, EventArgs e)
		{
			_CalcButtons();
			// Controls do not have a model resize loop.  Forms do.
			if (!this.DesignMode) {
				ParentForm.ResizeBegin += (s, ea) => BeginUpdate();
				ParentForm.ResizeEnd += (s, ea) => EndUpdate();
			}
		}

		[DefaultValue(true)]
		public bool ShowButtons
		{
			get { return _showButtons; }
			set
			{
				_showButtons = value;
				if (value) _CalcButtons();
				Invalidate();
			}
		}

		[DefaultValue(typeof (Color), "Transparent")]
		public Color BorderColor
		{
			get { return _borderColor; }
			set
			{
				_borderColor = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof (Color), "89, 135, 214")]
		public Color HeaderGradientStart
		{
			get { return _headerGradientStart; }
			set
			{
				_headerGradientStart = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof (Color), "4, 57, 148")]
		public Color HeaderGradientEnd
		{
			get { return _headerGradientEnd; }
			set
			{
				_headerGradientEnd = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof (Color), "119, 165, 214")]
		public Color HoverGradientStart
		{
			get { return _hoverGradientStart; }
			set
			{
				_hoverGradientStart = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof (Color), "24, 77, 148")]
		public Color HoverGradientEnd
		{
			get { return _hoverGradientEnd; }
			set
			{
				_hoverGradientEnd = value;
				Invalidate();
			}
		}

		public override string Text
		{
			get { return base.Text; }
			set { base.Text = value; }
		}

		private bool _updating;

		public void BeginUpdate()
		{
			_updating = true;
		}

		public void EndUpdate()
		{
			_updating = false;
			_CalcButtons();
			Refresh();
		}

		// Necessary to catch Maximize, Restore.
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			_CalcButtons();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			using (
				LinearGradientBrush headerGradientBrush = new LinearGradientBrush(ClientRectangle, HeaderGradientStart,
				                                                                  HeaderGradientEnd, 90)) {
				e.Graphics.FillRectangle(headerGradientBrush, ClientRectangle);
			}
			_DrawHeaderButtons(e.Graphics, _inLeftButtonBounds, _inRightButtonBounds);

			_DrawText(e.Graphics);

			_DrawBorder(e.Graphics);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			_CheckButtons(e.Location);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (_inLeftButtonBounds) {
				OnLeftButtonClick(EventArgs.Empty);
			}
			else if (_inRightButtonBounds) {
				OnRightButtonClick(EventArgs.Empty);
			}
		}

		protected virtual void _DrawHeaderButtons(Graphics g, bool hoverLeft, bool hoverRight)
		{
			if (!ShowButtons) return;

			if (!g.VisibleClipBounds.IntersectsWith(_buttonLeftBounds) && !g.VisibleClipBounds.IntersectsWith(_buttonRightBounds))
				return;

			int x = _buttonLeftBounds.X;
			int y = _buttonLeftBounds.Y;

			Point[] leftButtonPoints = new Point[]
			                           	{
			                           		new Point(x + 12, y + 5),
			                           		new Point(x + 12, y + 13),
			                           		new Point(x + 6, y + 9)
			                           	};

			Point[] rightButtonPoints = new Point[]
			                            	{
			                            		new Point(x + 6 + _buttonWidth + _buttonSpace, y + 5),
			                            		new Point(x + 6 + _buttonWidth + _buttonSpace, y + 13),
			                            		new Point(x + 12 + _buttonWidth + _buttonSpace, y + 9)
			                            	};

			using (
				LinearGradientBrush flatGradientBrush =
					new LinearGradientBrush(new Rectangle(x + 1, y + 1, _buttonWidth - 1, Height), HeaderGradientStart,
					                        HeaderGradientEnd, 90),
				                    hoverGradientBrush =
				                    	new LinearGradientBrush(new Rectangle(x + 1, y + 1, _buttonWidth - 1, _buttonWidth - 1),
				                    	                        HoverGradientStart, HoverGradientEnd, 90)) {
				g.FillRectangle(hoverLeft ? hoverGradientBrush : flatGradientBrush, x + 1, y + 1, _buttonWidth - 1, _buttonWidth - 1);
				g.FillRectangle(hoverRight ? hoverGradientBrush : flatGradientBrush, x + 1 + _buttonWidth + _buttonSpace, y + 1,
				                _buttonWidth - 1, _buttonWidth - 1);
			}

			_DrawRoundRect(g, Pens.White, x, y, _buttonWidth, _buttonWidth, 3);
			_DrawRoundRect(g, Pens.White, x + _buttonWidth + _buttonSpace, y, _buttonWidth, _buttonWidth, 3);

			g.FillPolygon(Brushes.White, leftButtonPoints);
			g.FillPolygon(Brushes.White, rightButtonPoints);
		}

		protected virtual void _DrawText(Graphics g)
		{
			SizeF textSize = g.MeasureString(Text, Font);
			if (g.VisibleClipBounds.IntersectsWith(new RectangleF(10, 5, textSize.Width, textSize.Height))) {
				g.DrawString(Text, Font, Brushes.White, 10, 5);
			}
		}

		protected virtual void _DrawBorder(Graphics g)
		{
			using (Pen pen = new Pen(_borderColor)) {
				g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
				;
			}
		}

		protected virtual void OnLeftButtonClick(EventArgs e)
		{
			if (LeftButtonClick != null) {
				LeftButtonClick(this, e);
			}
		}

		protected virtual void OnRightButtonClick(EventArgs e)
		{
			if (RightButtonClick != null) {
				RightButtonClick(this, e);
			}
		}

		private void _DrawRoundRect(Graphics g, Pen p, float X, float Y, float width, float height, float radius)
		{
			using (GraphicsPath gp = new GraphicsPath()) {
				gp.AddLine(X + radius, Y, X + width - (radius*2), Y);
				gp.AddArc(X + width - (radius*2), Y, radius*2, radius*2, 270, 90);
				gp.AddLine(X + width, Y + radius, X + width, Y + height - (radius*2));
				gp.AddArc(X + width - (radius*2), Y + height - (radius*2), radius*2, radius*2, 0, 90);
				gp.AddLine(X + width - (radius*2), Y + height, X + radius, Y + height);
				gp.AddArc(X, Y + height - (radius*2), radius*2, radius*2, 90, 90);
				gp.AddLine(X, Y + height - (radius*2), X, Y + radius);
				gp.AddArc(X, Y, radius*2, radius*2, 180, 90);
				gp.CloseFigure();

				g.DrawPath(p, gp);
			}
		}

		private void _CheckButtons(Point location)
		{
			bool wasInLeftButtonBounds = _inLeftButtonBounds;
			bool wasInRightButtonBounds = _inRightButtonBounds;
			_inLeftButtonBounds = _buttonLeftBounds.Contains(location);
			_inRightButtonBounds = _buttonRightBounds.Contains(location);
			if (wasInLeftButtonBounds != _inLeftButtonBounds) {
				Invalidate(_buttonLeftBounds);
			}
			if (wasInRightButtonBounds != _inRightButtonBounds) {
				Invalidate(_buttonRightBounds);
			}
		}

		private void _CalcButtons()
		{
			if (_updating) return;

			// Anchored to the right
			int x = Width - (_buttonWidth + _buttonSpace + _buttonWidth + _buttonSpace);
			// Anchored to the top
			int y = (Height - _buttonWidth)/2;

			_buttonLeftBounds = new Rectangle(x, y, _buttonWidth, _buttonWidth);
			_buttonRightBounds = new Rectangle(x + _buttonWidth + _buttonSpace, y, _buttonWidth, _buttonWidth);
		}

		private Rectangle _LeftButtonBounds
		{
			get { return _buttonLeftBounds; }
			set
			{
				if (value != _buttonLeftBounds) {
					Invalidate(_buttonLeftBounds);
					_buttonLeftBounds = value;
					Invalidate(_buttonLeftBounds);
				}
			}
		}

		private Rectangle _RightButtonBounds
		{
			get { return _buttonRightBounds; }
			set
			{
				if (value != _buttonRightBounds) {
					Invalidate(_buttonRightBounds);
					_buttonRightBounds = value;
					Invalidate(_buttonRightBounds);
				}
			}
		}
	}
}
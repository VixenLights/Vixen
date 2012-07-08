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
	partial class ScheduleAgendaView : UserControl, ISchedulerView {
		private int _headerHeight = 30;
		private int _agendaItemHeight = 50;

		private Font _dayViewHeaderFont = new Font("Arial", 12, FontStyle.Bold);
		private Font _agendaViewItemFont = new Font("Arial", 10, FontStyle.Bold);
		private Font _agendaViewTimeFont = new Font("Arial", 8);

		private Color _headerGradientStart = Color.FromArgb(89, 135, 214);
		private Color _headerGradientEnd = Color.FromArgb(4, 57, 148);
		private Color _hoverGradientStart = Color.FromArgb(119, 165, 214);
		private Color _hoverGradientEnd = Color.FromArgb(24, 77, 148);

		private bool _inLeftButtonBounds = false;
		private bool _inRightButtonBounds = false;
		private Rectangle _buttonLeftBounds;
		private Rectangle _buttonRightBounds;

		public ScheduleAgendaView() {
			InitializeComponent();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
		}

		[DefaultValue(30)]
		public int HeaderHeight {
			get { return _headerHeight; }
			set {
				_headerHeight = value;
				_UpdateScroll();
				Invalidate();
			}
		}

		[DefaultValue(typeof(Font), "Arial, 12pt, style=Bold")]
		public Font HeaderFont {
			get { return _dayViewHeaderFont; }
			set {
				_dayViewHeaderFont.Dispose();
				_dayViewHeaderFont = value;
				Invalidate();
			}
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

		[DefaultValue(50)]
		public int AgendaItemHeight {
			get { return _agendaItemHeight; }
			set {
				_agendaItemHeight = value;
				Invalidate();
			}
		}

		protected override void OnResize(EventArgs e) {
			Invalidate();
			_UpdateScroll();
			base.OnResize(e);
		}

		protected override void OnPaint(PaintEventArgs e) {
			Graphics g = e.Graphics; //***

			g.FillRectangle(Brushes.White, 0, _headerHeight, Width, Height - _headerHeight);

			// Header
			using(LinearGradientBrush headerGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, Width, HeaderHeight), HeaderGradientStart, HeaderGradientEnd, 90)) {
				g.FillRectangle(headerGradientBrush, 0, 0, Width, _headerHeight);
			}
			_DrawHeaderButtons(g, _inLeftButtonBounds, _inRightButtonBounds);

			// Current day
			g.DrawString(DateTime.Today.ToLongDateString(), _dayViewHeaderFont, Brushes.White, 10, 5);

			//// Horizontal dividers
			//for(int i = 0, y = HeaderHeight + AgendaItemHeight; i < m_applicableTimers.Count && y < Bottom; i++, y += AgendaItemHeight) {
			//    g.DrawLine(Pens.Gray, Left, y, Right, y);
			//}

			//// Draw the applicable timers
			//Brush fontColor;
			//foreach(Timer timer in m_applicableTimers) {
			//    if(timer.EndTime < DateTime.Now.TimeOfDay) {
			//        fontColor = Brushes.Gray;
			//    } else {
			//        if(timer.StartTime > DateTime.Now.TimeOfDay) {
			//            fontColor = Brushes.Black;
			//        } else {
			//            fontColor = Brushes.Red;
			//        }
			//    }
			//    foreach(ReferenceRectF rect in timer.DisplayBounds) {
			//        g.DrawString(timer.ProgramName, AgendaViewItemFont, fontColor, rect.Left + 16, rect.Top + 10);
			//        g.DrawString(string.Format("{0} - {1}", timer.StartDateTime.ToString("h:mm tt"), timer.EndDateTime.ToString("h:mm tt")), AgendaViewTimeFont, fontColor, rect.Left + 16, rect.Top + 30);
			//    }
			//}
		}


		//duplicate
		protected virtual void _DrawHeaderButtons(Graphics g, bool hoverLeft, bool hoverRight) {
			int buttonWidth = 18;
			int buttonSpace = 8;

			// Anchored to the right
			int x = Width - (buttonWidth + buttonSpace + buttonWidth + buttonSpace);
			// Anchored to the top
			int y = (HeaderHeight - buttonWidth) / 2;

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

			using(LinearGradientBrush flatGradientBrush = new LinearGradientBrush(new Rectangle(x + 1, y + 1, buttonWidth - 1, HeaderHeight), HeaderGradientStart, HeaderGradientEnd, 90),
									  hoverGradientBrush = new LinearGradientBrush(new Rectangle(x + 1, y + 1, buttonWidth - 1, buttonWidth - 1), HoverGradientStart, HoverGradientEnd, 90)) {
				g.FillRectangle(hoverLeft ? hoverGradientBrush : flatGradientBrush, x + 1, y + 1, buttonWidth - 1, buttonWidth - 1);
				g.FillRectangle(hoverRight ? hoverGradientBrush : flatGradientBrush, x + 1 + buttonWidth + buttonSpace, y + 1, buttonWidth - 1, buttonWidth - 1);
			}

			_DrawRoundRect(g, Pens.White, x, y, buttonWidth, buttonWidth, 3);
			_DrawRoundRect(g, Pens.White, x + buttonWidth + buttonSpace, y, buttonWidth, buttonWidth, 3);

			g.FillPolygon(Brushes.White, leftButtonPoints);
			g.FillPolygon(Brushes.White, rightButtonPoints);
		}

		//duplicate
		private void _DrawRoundRect(Graphics g, Pen p, float X, float Y, float width, float height, float radius) {
			GraphicsPath gp = new GraphicsPath();

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
			gp.Dispose();
		}

		private void _UpdateScroll() {
			vScrollBar.Top = HeaderHeight;
			vScrollBar.Left = Width - vScrollBar.Width;
			vScrollBar.Height = Height - HeaderHeight;
			//int largeChange = (Height - HeaderHeight) / AgendaItemHeight;
			//if(largeChange > m_applicableTimers.Count) {
			//    //vScrollBar.Enabled = false;
			//    vScrollBar.Visible = false;
			//    vScrollBar.Value = 0;
			//} else {
			//    vScrollBar.Maximum = m_applicableTimers.Count;
			//    vScrollBar.LargeChange = largeChange;
			//    //if(m_agendaViewScrollBarValue <= vScrollBar.Maximum) {
			//    //    vScrollBar.Value = m_agendaViewScrollBarValue;
			//    //} else {
			//    //    vScrollBar.Value = vScrollBar.Maximum;
			//    //}
			//    //vScrollBar.Enabled = true;
			//    vScrollBar.Visible = true;
			//}
			vScrollBar.Visible = false;
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e) {
			Invalidate(new Rectangle(0, HeaderHeight, Width, Height - HeaderHeight));
		}



		public IEnumerable<IScheduleItem> Items {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
	}
}

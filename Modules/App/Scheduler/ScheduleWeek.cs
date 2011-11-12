using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scheduler {
	public partial class ScheduleWeek : UserControl {
		private int _halfHourHeight = 20;
		private int _headerHeight = 30;
		private int _timeGutter = 50;
		private int _dayHeaderHeight = 17;

		private SolidBrush _backgroundBrush = new SolidBrush(Color.FromArgb(255, 255, 213));
		private Pen _hourPen = new Pen(Color.FromArgb(246, 219, 162));
		private Pen _halfHourPen = new Pen(Color.FromArgb(255, 239, 199));
		private Font _timeLargeFont = new Font("Tahoma", 16);
		private Font _timeSmallFont = new Font("Tahoma", 8);
		private Font _dayViewHeaderFont = new Font("Arial", 12, FontStyle.Bold);
		private Pen _timeLinePen = new Pen(Color.FromKnownColor(KnownColor.ControlDark));

		private Color _headerGradientStart = Color.FromArgb(89, 135, 214);
		private Color _headerGradientEnd = Color.FromArgb(4, 57, 148);
		private Color _hoverGradientStart = Color.FromArgb(119, 165, 214);
		private Color _hoverGradientEnd = Color.FromArgb(24, 77, 148);

		private bool _inLeftButtonBounds = false;
		private bool _inRightButtonBounds = false;
		private Rectangle _buttonLeftBounds;
		private Rectangle _buttonRightBounds;

		private const int DAY_HEADER_PAD = 2;
		private const int DAY_HEADER_SPACING = 5;

		public ScheduleWeek() {
			InitializeComponent();

			BackColor = Color.FromArgb(255, 255, 213);

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
		}

		[DefaultValue(typeof(Color), "255, 255, 213")]
		public override Color BackColor {
			get { return base.BackColor; }
			set {
				base.BackColor = value;
				_backgroundBrush.Dispose();
				_backgroundBrush = new SolidBrush(value);
				Invalidate();
			}
		}

		[DefaultValue(20)]
		public int HalfHourHeight {
			get { return _halfHourHeight; }
			set {
				_halfHourHeight = value;
				_UpdateScroll();
				Invalidate();
			}
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

		[DefaultValue(50)]
		public int TimeGutter {
			get { return _timeGutter; }
			set {
				_timeGutter = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "246, 219, 162")]
		public Color HourColor {
			get { return _hourPen.Color; }
			set {
				_hourPen.Dispose();
				_hourPen = new Pen(value);
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "255, 239, 199")]
		public Color HalfHourColor {
			get { return _halfHourPen.Color; }
			set {
				_halfHourPen.Dispose();
				_halfHourPen = new Pen(value);
				Invalidate();
			}
		}

		[DefaultValue(typeof(Font), "Tahoma, 16pt")]
		public Font TimeLargeFont {
			get { return _timeLargeFont; }
			set {
				_timeLargeFont.Dispose();
				_timeLargeFont = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Font), "Tahoma, 8pt")]
		public Font TimeSmallFont {
			get { return _timeSmallFont; }
			set {
				_timeSmallFont.Dispose();
				_timeSmallFont = value;
				Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "ControlDark")]
		public Color TimeLineColor {
			get { return _timeLinePen.Color; }
			set {
				_timeLinePen.Dispose();
				_timeLinePen = new Pen(value);
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

		[DefaultValue(17)]
		public int DayHeaderHeight {
			get { return _dayHeaderHeight; }
			set {
				_dayHeaderHeight = value;
				Invalidate();
			}
		}

		protected override void OnResize(EventArgs e) {
			Invalidate();
			_UpdateScroll();
			base.OnResize(e);
		}

		protected override void OnPaint(PaintEventArgs e) {
			Graphics g = e.Graphics;//***

			if(g.ClipBounds.Left < TimeGutter) {
				_DrawTimes(g);
			}

			if(g.ClipBounds.Right >= TimeGutter) {
				_DrawLines(g);
			}

			float dayWidth = (float)(Width - _timeGutter) / 7;
			
			if(g.ClipBounds.Top < HeaderHeight + DayHeaderHeight) {
				_DrawHeader(g, dayWidth);
			}

			// Vertical dividers
			for(int i = 1; i < 7; i++) {
				float x = dayWidth * i + TimeGutter;
				g.DrawLine(Pens.Black, x, HeaderHeight, x, Height);
			}

			// Current week
			DateTime startDate = DateTime.Today.AddDays(-((int)DateTime.Today.DayOfWeek));
			DateTime endDate = startDate.AddDays(6);
			g.DrawString(string.Format("{0}   -   {1}", startDate.ToString("m"), endDate.ToString("m")), HeaderFont, Brushes.White, TimeGutter + 10, 5);

			// Draw the blocks for the applicable timers
			//_DrawTimerBlocks(_applicableTimers, g);
		}

		private void _DrawHeader(Graphics g, float dayWidth) {
			using(LinearGradientBrush headerGradientBrush = new LinearGradientBrush(new Rectangle(0, 0, Width - TimeGutter, HeaderHeight), HeaderGradientStart, HeaderGradientEnd, 90)) {

				// Day headers
				SolidBrush dayBrush = new SolidBrush(_halfHourPen.Color);
				int currentDateIndex = (int)DateTime.Today.DayOfWeek;
				for(int i = 0; i < 7; i++) {
					float x = dayWidth * i + _timeGutter;
					string dayOfWeek = Enum.GetValues(typeof(DayOfWeek)).GetValue(i).ToString();
					if(currentDateIndex == i) {
						// Highlight current day's header
						g.FillRectangle(headerGradientBrush, x + DAY_HEADER_PAD, HeaderHeight + DAY_HEADER_PAD, dayWidth - DAY_HEADER_SPACING, DayHeaderHeight);
						g.DrawRectangle(Pens.Navy, x + DAY_HEADER_PAD, HeaderHeight + DAY_HEADER_PAD, dayWidth - DAY_HEADER_SPACING, DayHeaderHeight);
					} else {
						g.FillRectangle(dayBrush, x + DAY_HEADER_PAD, HeaderHeight + DAY_HEADER_PAD, dayWidth - DAY_HEADER_SPACING, DayHeaderHeight);
						g.DrawRectangle(_hourPen, x + DAY_HEADER_PAD, HeaderHeight + DAY_HEADER_PAD, dayWidth - DAY_HEADER_SPACING, DayHeaderHeight);
					}
					g.DrawString(dayOfWeek, TimeSmallFont, Brushes.Black, x + (dayWidth - g.MeasureString(dayOfWeek, TimeSmallFont).Width) / 2, HeaderHeight + 4);
				}

				// Header
				g.FillRectangle(headerGradientBrush, TimeGutter, 0, Width - TimeGutter, HeaderHeight);
				_DrawHeaderButtons(g, _inLeftButtonBounds, _inRightButtonBounds);

			}
		}

		//duplicate
		protected virtual void _DrawTimerBlocks(IEnumerable<ScheduleItem> items, Graphics g) {
			RectangleF textRect = new RectangleF();

			foreach(ScheduleItem item in items) {
				//foreach(ReferenceRectF timerDisplayRect in item.DisplayBounds) {
				//    g.FillRectangle(Brushes.White, timerDisplayRect.ToRectangleF());
				//    g.DrawRectangle(Pens.Black, timerDisplayRect.X, timerDisplayRect.Y, timerDisplayRect.Width, timerDisplayRect.Height);

				//    textRect.X = timerDisplayRect.X + 2;
				//    textRect.Y = timerDisplayRect.Y;
				//    textRect.Width = timerDisplayRect.Width;
				//    textRect.Height = Math.Min(13, timerDisplayRect.Height);
				//    foreach(string line in timer.ToString().Split('|')) {
				//        g.DrawString(line, m_timeSmallFont, Brushes.Black, textRect);
				//        textRect.Y += textRect.Height;
				//        if(textRect.Top >= timerDisplayRect.Bottom) {
				//            break;
				//        }
				//        if(textRect.Bottom > timerDisplayRect.Bottom) {
				//            textRect.Height = timerDisplayRect.Bottom - textRect.Top;
				//        }
				//    }
				//}
			}
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

		//duplicate
		protected virtual void _DrawTimes(Graphics g) {
			g.DrawLine(_timeLinePen, 0, HeaderHeight, TimeGutter - 5, HeaderHeight);

			int y;

			// Times
			int hour = (vScrollBar.Value % 2 == 1) ? vScrollBar.Value / 2 + 1 : vScrollBar.Value / 2;
			string hourString;
			int midPoint = _timeGutter >> 1;
			int lastMeridian = -1;
			string meridianString;

			for(y = 3 + HeaderHeight + ((vScrollBar.Value % 2 == 0) ? 0 : HalfHourHeight); y < Height && hour < 24; y += HalfHourHeight * 2) {
				if(hour == 0) {
					hourString = "12";
				} else if(hour > 12) {
					hourString = (hour - 12).ToString();
				} else {
					hourString = hour.ToString();
				}

				g.DrawString(hourString, TimeLargeFont, Brushes.Black, midPoint - (int)(g.MeasureString(hourString, TimeLargeFont).Width) + 6, y);

				if(lastMeridian != hour / 12) {
					lastMeridian = hour / 12;
					meridianString = lastMeridian == 0 ? "am" : "pm";
				} else {
					meridianString = "00";
				}

				g.DrawString(meridianString, TimeSmallFont, Brushes.Black, midPoint + 2, y);
				hour++;
			}
		}

		//duplicate
		protected virtual void _DrawLines(Graphics g) {
			g.FillRectangle(_backgroundBrush, TimeGutter, HeaderHeight, Width - TimeGutter, Height - HeaderHeight);

			int y;

			// Hour lines
			y = HeaderHeight + ((vScrollBar.Value % 2 == 0) ? HalfHourHeight * 2 : HalfHourHeight);
			while(y < Height) {
				g.DrawLine(_hourPen, TimeGutter, y, Width, y);
				g.DrawLine(_timeLinePen, 5, y, TimeGutter - 5, y);
				y += HalfHourHeight * 2;
			}

			// Half-hour lines
			y = HeaderHeight + ((vScrollBar.Value % 2 == 0) ? HalfHourHeight : HalfHourHeight * 2);
			while(y < Height) {
				g.DrawLine(_halfHourPen, TimeGutter, y, Width, y);
				y += _halfHourHeight * 2;
			}
		}

		//duplicate
		private void _UpdateScroll() {
			vScrollBar.Top = HeaderHeight + DayHeaderHeight + DAY_HEADER_PAD;
			vScrollBar.Left = Width - vScrollBar.Width;
			vScrollBar.Height = Height - HeaderHeight;
			vScrollBar.Visible = (Height - HeaderHeight) / HalfHourHeight <= vScrollBar.Maximum;
			if(vScrollBar.Visible) {
				vScrollBar.LargeChange = (Height - HeaderHeight) / HalfHourHeight;

				if(vScrollBar.Value + vScrollBar.LargeChange > vScrollBar.Maximum) {
					vScrollBar.Value = vScrollBar.Maximum - vScrollBar.LargeChange + 1;
				}
			}
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e) {
			Invalidate(new Rectangle(0, HeaderHeight, Width, Height - HeaderHeight));
		}

	}
}

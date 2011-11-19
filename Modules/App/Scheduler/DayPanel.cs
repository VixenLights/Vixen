using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace VixenModules.App.Scheduler {
	partial class DayPanel : UserControl {
		private int _halfHourHeight = 20;
		private int _timeGutter = 50;
		private int _topHalfHour;
		private bool _showTime = true;

		private SolidBrush _backgroundBrush = new SolidBrush(Color.FromArgb(255, 255, 213));
		private Pen _hourPen = new Pen(Color.FromArgb(246, 219, 162));
		private Pen _halfHourPen = new Pen(Color.FromArgb(255, 239, 199));
		private Font _timeLargeFont = new Font("Tahoma", 16);
		private Font _timeSmallFont = new Font("Tahoma", 8);
		private Pen _timeLinePen = new Pen(Color.FromKnownColor(KnownColor.ControlDark));

		private BlockLayoutEngine _layoutEngine;

		public event EventHandler<ScheduleEventArgs> TimeDoubleClick;

		public DayPanel() {
			InitializeComponent();

			_layoutEngine = new BlockLayoutEngine();

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

		[DefaultValue(true)]
		public bool ShowTime {
			get { return _showTime; }
			set {
				_showTime = value;
				TimeGutter = 0;
			}
		}

		private int _VisibleHalfHours {
			get { return Height / HalfHourHeight; }
		}

		public int TopHalfHour {
			get { return _topHalfHour; }
			set {
				if(value < 0 || value > 48 - _VisibleHalfHours) throw new IndexOutOfRangeException();
				_topHalfHour = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs e) {
			if(e.Graphics.VisibleClipBounds.Left < TimeGutter) {
				_DrawTimes(e.Graphics);
			}

			if(e.Graphics.VisibleClipBounds.Right >= TimeGutter) {
				_DrawLines(e.Graphics);
			}
		}

		private int _HalfHourFromY(int y) {
			return y / HalfHourHeight;
		}

		private int _GetHalfHourAt(int y) {
			return TopHalfHour + _HalfHourFromY(y);
		}

		private void _DrawTimes(Graphics g) {
			// Times
			int startHalfHour = _GetHalfHourAt((int)g.VisibleClipBounds.Y) / 2 * 2;
			int hour = startHalfHour / 2;
			int topHour = (TopHalfHour % 2 == 1) ? TopHalfHour / 2 + 1 : TopHalfHour / 2;
			string hourString;
			int midPoint = TimeGutter >> 1;
			string meridianString;

			for(int y = 3 + (startHalfHour - TopHalfHour) * HalfHourHeight; y < g.VisibleClipBounds.Bottom && hour < 24; y += HalfHourHeight * 2) {
				if(hour == 0) {
					hourString = "12";
				} else if(hour > 12) {
					hourString = (hour - 12).ToString();
				} else {
					hourString = hour.ToString();
				}

				g.DrawString(hourString, TimeLargeFont, Brushes.Black, midPoint - (int)(g.MeasureString(hourString, TimeLargeFont).Width) + 6, y);

				// AM/PM
				if(hour == topHour) {
					meridianString = topHour / 12 == 0 ? "am" : "pm";
				} else {
					meridianString = "00";
				}

				g.DrawString(meridianString, TimeSmallFont, Brushes.Black, midPoint + 2, y);
				hour++;
			}
		}

		protected virtual void _DrawLines(Graphics g) {
			g.FillRectangle(_backgroundBrush, TimeGutter, 0, Width - TimeGutter, Height);

			int startHalfHour = _GetHalfHourAt((int)g.VisibleClipBounds.Y);// / 2 * 2;
			int hour = startHalfHour / 2;

			// Hour lines
			int y = (startHalfHour - TopHalfHour) * HalfHourHeight;
			if(TopHalfHour % 2 == 1) y += HalfHourHeight;
	
			for(; y < g.VisibleClipBounds.Bottom && hour <= 24; y += HalfHourHeight * 2) {
				g.DrawLine(_hourPen, TimeGutter, y, Width, y);
				g.DrawLine(_timeLinePen, 5, y, TimeGutter - 5, y);
				hour++;
			}

			// Half-hour lines
			hour = startHalfHour / 2;
			y = (startHalfHour - TopHalfHour + 1) * HalfHourHeight;
			if(TopHalfHour % 2 == 1) y += HalfHourHeight;

			for(; y < g.VisibleClipBounds.Bottom && hour < 24; y += HalfHourHeight * 2) {
				g.DrawLine(_halfHourPen, TimeGutter, y, Width, y);
				hour++;
			}
		}

		private void DayPanel_MouseDoubleClick(object sender, MouseEventArgs e) {
			double halfHour = _GetHalfHourAt(e.Y);
			OnTimeDoubleClick(new ScheduleEventArgs(0, TimeSpan.FromHours(halfHour / 2)));
		}

		protected virtual void OnTimeDoubleClick(ScheduleEventArgs e) {
			if(TimeDoubleClick != null) {
				TimeDoubleClick(this, e);
			}
		}

		public override LayoutEngine LayoutEngine {
			get { return _layoutEngine; }
		}

		class BlockLayoutEngine : LayoutEngine {
			//public override void InitLayout(object child, BoundsSpecified specified) {
			//    base.InitLayout(child, specified);
			//}

			public override bool Layout(object container, LayoutEventArgs layoutEventArgs) {
				DayPanel parent = container as DayPanel;
				if(parent == null) throw new InvalidOperationException("Layout panel must be of type DayPanel");

				Rectangle rect = new Rectangle(parent.DisplayRectangle.X + parent.TimeGutter, parent.DisplayRectangle.Y, parent.DisplayRectangle.Width - parent.TimeGutter, parent.DisplayRectangle.Height);

				List<Control> placedBlocks = new List<Control>();
				List<Control> intersectingBlocks = new List<Control>();

				int minX;
				int x;
				int width;
				foreach(ScheduleItemVisual control in parent.Controls.OfType<ScheduleItemVisual>()) {
					if(!control.Visible) continue;

					// Initially set block to the full display width at the appropriate slot
					control.Left = rect.Left;
					control.Width = rect.Width;

					control.Top = (int)control.Item.RunStartTime.TotalMinutes / 30 * parent.HalfHourHeight;
					control.Height = (int)(control.Item.RunEndTime - control.Item.RunStartTime).TotalMinutes / 30 * parent.HalfHourHeight;
					//(testing)
					control.Height = Math.Max(control.Height, 10);

					if(placedBlocks.Count > 0) {
						minX = rect.Right;
						intersectingBlocks.Clear();

						// Iterate placed blocks, find all blocks that intersect
						foreach(Control placedBlock in placedBlocks) {
							if(placedBlock.ClientRectangle.IntersectsWith(rect)) {
								intersectingBlocks.Add(placedBlock);
								minX = Math.Min(placedBlock.Left, minX);
							}
						}
						// If the min of all intersecting blocks = rect.X, recalc all intersected
						// blocks and set the new block size, location appropriately.
						if(minX == rect.X) {
							x = rect.X;
							width = rect.Width / (intersectingBlocks.Count + 1);
							foreach(Control intersectingBlock in intersectingBlocks) {
								intersectingBlock.Left = x;
								intersectingBlock.Width = width;
								x += width;
							}
							control.Left = x;
							control.Width = width;
						} else {
							control.Width = minX - rect.X;
						}
					}

					placedBlocks.Add(control);
				}

				return false;
			}
		}
	}
}

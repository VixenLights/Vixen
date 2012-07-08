using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace VixenModules.App.Scheduler {
	partial class DayPanel : UserControl {
		private int _halfHourHeight = 20;
		private int _timeGutter = 50;
		private bool _showTime = true;
		private bool _updating = false;

		private SolidBrush _backgroundBrush = new SolidBrush(Color.FromArgb(255, 255, 213));
		private Pen _hourPen = new Pen(Color.FromArgb(246, 219, 162));
		private Pen _halfHourPen = new Pen(Color.FromArgb(255, 239, 199));
		private Font _timeLargeFont = new Font("Tahoma", 16);
		private Font _timeSmallFont = new Font("Tahoma", 8);
		private Pen _timeLinePen = new Pen(Color.FromKnownColor(KnownColor.ControlDark));

		private BlockLayoutEngine _layoutEngine;
		private List<IScheduleItem> _items;
		private List<ItemBlock> _blocks;
		private ItemBlock _selectedBlock;

		public event EventHandler<ScheduleEventArgs> TimeDoubleClick;
		public event EventHandler<ScheduleItemArgs> ItemDoubleClick;
		public event EventHandler<ScheduleItemArgs> ItemClick;

		public DayPanel() {
			InitializeComponent();

			_items = new List<IScheduleItem>();
			_blocks = new List<ItemBlock>();
			_layoutEngine = new BlockLayoutEngine();

			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.ResizeRedraw, true);

			BackColor = Color.FromArgb(255, 255, 213);
			AutoScrollMinSize = new Size(Width, HalfHourHeight * 48);
		}

		private void DayPanel_Load(object sender, EventArgs e) {
			// Controls do not have a model resize loop.  Forms do.
			if(!this.DesignMode) {
				ParentForm.ResizeBegin += (s, ea) => BeginUpdate();
				ParentForm.ResizeEnd += (s, ea) => EndUpdate();
			}
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
				AutoScrollMinSize = new Size(Width, value * 48);
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

		public IEnumerable<IScheduleItem> Items {
			get { return _items; }
			set {
				_items.Clear();
				_items.AddRange(value);
				_CalculateBlocks();
			}
		}

		public void BeginUpdate() {
			_updating = true;
		}

		public void EndUpdate() {
			_updating = false;
			_CalculateBlocks(); 
			Refresh();
		}

		public IScheduleItem SelectedItem {
			get { return (_SelectedBlock != null) ? _SelectedBlock.Item : null; }
		}

		private int _VisibleHalfHours {
			get { return Height / HalfHourHeight; }
		}

		protected override void OnPaint(PaintEventArgs e) {
			//if(_updating) return;
			e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
			_DrawTimes(e.Graphics);
			_DrawLines(e.Graphics);
			_DrawItems(e.Graphics);
		}

		// Necessary to catch Maximize, Restore.
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			_CalculateBlocks();
		}

		private int _TopHalfHour {
			get { return _HalfHourFromY(-AutoScrollPosition.Y); }
		}

		private int _HalfHourFromY(int y) {
			return y / HalfHourHeight;
		}

		private int _GetHalfHourAt(int y) {
			return _HalfHourFromY(y);
		}

		private int _TranslateX(int value) {
			return value - AutoScrollPosition.X;
		}

		private int _TranslateY(int value) {
			return value - AutoScrollPosition.Y;
		}

		private int _TimeToPixels(TimeSpan time) {
			return (int)(time.TotalMinutes / 30 * HalfHourHeight);
		}

		private void _DrawTimes(Graphics g) {
			RectangleF rect = new RectangleF(0, 0, TimeGutter, Height);
			rect.Intersect(g.VisibleClipBounds);
			g.FillRectangle(_backgroundBrush, rect);

			// Times
			int startHalfHour = _GetHalfHourAt((int)g.VisibleClipBounds.Y) / 2 * 2;
			int hour = startHalfHour / 2;
			int topHour = (_TopHalfHour % 2 == 1) ? _TopHalfHour / 2 + 1 : _TopHalfHour / 2;
			string hourString;
			int midPoint = TimeGutter >> 1;
			string meridianString;

			for(int y = startHalfHour * HalfHourHeight; y < g.VisibleClipBounds.Bottom && hour < 24; y += HalfHourHeight * 2) {
				if(hour == 0) {
					hourString = "12";
				} else if(hour > 12) {
					hourString = (hour - 12).ToString();
				} else {
					hourString = hour.ToString();
				}

				g.DrawLine(_timeLinePen, 5, y, TimeGutter - 5, y);
				g.DrawString(hourString, TimeLargeFont, Brushes.Black, midPoint - (int)(g.MeasureString(hourString, TimeLargeFont).Width) + 6, y + 3);

				// AM/PM
				if(hour == topHour) {
					meridianString = topHour / 12 == 0 ? "am" : "pm";
				} else {
					meridianString = "00";
				}

				g.DrawString(meridianString, TimeSmallFont, Brushes.Black, midPoint + 2, y + 3);
				hour++;
			}
		}

		private void _DrawLines(Graphics g) {
			RectangleF rect = new RectangleF(TimeGutter, 0, Width - TimeGutter, Height);
			rect.Intersect(g.VisibleClipBounds);
			g.FillRectangle(_backgroundBrush, rect);

			int startHalfHour = _GetHalfHourAt((int)g.VisibleClipBounds.Y);
			int hour = startHalfHour / 2;

			// Hour lines
			int y = hour * HalfHourHeight * 2;
	
			for(; y < g.VisibleClipBounds.Bottom && hour <= 24; y += HalfHourHeight * 2) {
				g.DrawLine(_hourPen, TimeGutter, y, Width, y);
				hour++;
			}

			// Half-hour lines
			hour = startHalfHour / 2;
			y = startHalfHour * HalfHourHeight;
			if(startHalfHour % 2 == 0) y += HalfHourHeight;

			for(; y < g.VisibleClipBounds.Bottom && hour < 24; y += HalfHourHeight * 2) {
				g.DrawLine(_halfHourPen, TimeGutter, y, Width, y);
				hour++;
			}
		}

		private void _DrawItems(Graphics g) {
			foreach(ItemBlock block in _blocks) {
				_DrawBlock(block, g);
			}
		}

		private void _DrawBlock(ItemBlock block, Graphics g) {
			int penWidth = block.Selected ? 3 : 1;
			using(Pen borderPen = new Pen(Color.Black, penWidth)) {
				_DrawRoundRect(g, borderPen, Brushes.White, block.Left, block.Top, block.Width - 1, block.Height - 1, 3);
			}
			g.DrawString(block.ProgramName, Font, Brushes.Black, new RectangleF(block.Left + 3, block.Top + 2, block.Width - 1, block.Height - 1));
		}

		private void _DrawRoundRect(Graphics g, Pen borderPen, Brush fillBrush, float X, float Y, float width, float height, float radius) {
			g.SmoothingMode = SmoothingMode.AntiAlias;
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

				g.FillPath(fillBrush, gp);
				g.DrawPath(borderPen, gp);
			}
		}

		private ItemBlock _GetItemAt(int x, int y) {
			return _blocks.FirstOrDefault(b => b.Contains(x, y));
		}

		private ItemBlock _SelectedBlock {
			get { return _selectedBlock; }
			set {
				if(value != _selectedBlock) {
					if(_selectedBlock != null) {
						_selectedBlock.Selected = false;
						//_RedrawBlock(_selectedBlock);
					}
					_selectedBlock = value;
					if(_selectedBlock != null) {
						_selectedBlock.Selected = true;
						//_RedrawBlock(_selectedBlock);
					}
					//Update();
					// Would rather Update(), but no painting happens even though areas have
					// been invalidated...?
					Refresh();
				}
			}
		}

		//private void _RedrawBlock(ItemBlock block) {
		//    int x = _TranslateX(block.Left);
		//    int y = _TranslateY(block.Top);
		//    Invalidate(new Rectangle(x, y, block.Width, block.Height));
		//}

		private void DayPanel_MouseDown(object sender, MouseEventArgs e) {
			ItemBlock itemBlock = _GetItemAt(_TranslateX(e.X), _TranslateY(e.Y));
			_SelectedBlock = itemBlock;
			OnItemClick(new ScheduleItemArgs((itemBlock != null) ? itemBlock.Item : null));
		}

		private void DayPanel_MouseDoubleClick(object sender, MouseEventArgs e) {
			ItemBlock itemBlock = _GetItemAt(_TranslateX(e.X), _TranslateY(e.Y));
			if(itemBlock != null) {
				OnItemDoubleClick(new ScheduleItemArgs(itemBlock.Item));
			} else {
				double halfHour = _GetHalfHourAt(_TranslateY(e.Y));
				OnTimeDoubleClick(new ScheduleEventArgs(0, TimeSpan.FromHours(halfHour / 2)));
			}
		}

		private void _CalculateBlocks() {
			if(_updating) return;

			_blocks = new List<ItemBlock>();
			foreach(ScheduleItem item in Items) {
				int top = _TimeToPixels(item.RunStartTime);
				int height = _TimeToPixels(item.RunEndTime - item.RunStartTime);
				ItemBlock block = new ItemBlock(item, top, Math.Max(height, 10));
				_blocks.Add(block);
			}
			_layoutEngine.Layout(_blocks, new Rectangle(TimeGutter, 0, DisplayRectangle.Width - TimeGutter, DisplayRectangle.Height));
		}

		protected virtual void OnTimeDoubleClick(ScheduleEventArgs e) {
			if(TimeDoubleClick != null) {
				TimeDoubleClick(this, e);
			}
		}

		protected virtual void OnItemDoubleClick(ScheduleItemArgs e) {
			if(ItemDoubleClick != null) {
				ItemDoubleClick(this, e);
			}
		}

		protected virtual void OnItemClick(ScheduleItemArgs e) {
			if(ItemClick != null) {
				ItemClick(this, e);
			}
		}
	}
}

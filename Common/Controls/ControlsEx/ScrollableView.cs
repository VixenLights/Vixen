using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ControlsEx
{
	/// <summary>
	/// scrollableview is a scrollablecontrol
	/// that doesnt scroll child controls, also
	/// and therefore is a replacement for
	/// applications such as listviews with
	/// headers or something
	/// </summary>
	public class ScrollableView : Control
	{
		#region types
		private abstract class ScrollProperties
		{
			#region variables
			private ScrollableView _parent;
			//data
			private bool _visible = false;
			private int _smallChange = 1,
				_maximum = 100,
				_minimum = 0,
				_value = 0;
			#endregion
			// Methods
			protected ScrollProperties(ScrollableView container)
			{
				if (container == null) throw new ArgumentNullException("container");
				this._parent = container;
			}

			/// <summary>
			/// updates the data to the visual interface
			/// </summary>
			public void UpdateScrollInfo()
			{
				if (_parent.IsHandleCreated && _visible)
				{
					Win32.SCROLLINFO si = new Win32.SCROLLINFO();
					si.cbSize = Marshal.SizeOf(typeof(Win32.SCROLLINFO));
					si.fMask = 0x17;
					si.nMin = _minimum;
					si.nMax = _maximum;
					si.nPage = this.PageSize;
					si.nPos = _value;
					si.nTrackPos = 0;
					Win32.SetScrollInfo(
						new HandleRef(_parent, _parent.Handle),
						this.Orientation, si, true);
				}
			}
			/// <summary>
			/// gets the position where the scroll thumb is released
			/// </summary>
			public int GetScrollThumbPosition()
			{
				Win32.SCROLLINFO si = new Win32.SCROLLINFO();
				si.fMask = 0x10;
				bool r = Win32.GetScrollInfo(
					new HandleRef(_parent, _parent.Handle),
					this.Orientation, si);
				return si.nTrackPos;
			}
			#region properties
			protected ScrollableView ParentControl
			{
				get { return _parent; }
			}
			public abstract int HorizontalDisplayPosition { get; }
			public abstract int VerticalDisplayPosition { get; }
			public abstract int Orientation { get; }
			public abstract int PageSize { get; }

			public bool Visible
			{
				get { return _visible; }
				set { _visible = value; }
			}
			public int Maximum
			{
				get { return _maximum; }
				set
				{
					if (_minimum > value) _minimum = value;
					if (_value > value) _value = value;
					_maximum = value;
				}
			}
			public int Minimum
			{
				get { return _minimum; }
				set
				{
					if (value < 0) value = 0;
					if (_maximum < value) _maximum = value;
					if (_value < value) _value = value;
					_minimum = value;
				}
			}
			public int SmallChange
			{
				get { return Math.Min(_smallChange, this.PageSize); }
				set
				{
					if (value < 0) value = 0;
					_smallChange = value;
				}
			}
			public int Value
			{
				get { return _value; }
				set
				{
					if (value > _maximum) value = _maximum;
					if (value < _minimum) value = _minimum;
					_value = value;
				}
			}
			#endregion
		}
		private class HScrollProperties : ScrollProperties
		{
			public HScrollProperties(ScrollableView container) : base(container) { }

			public override int HorizontalDisplayPosition
			{
				get { return -Value; }
			}
			public override int Orientation
			{
				get { return 0; }
			}
			public override int PageSize
			{
				get { return ParentControl.ClientRectangle.Width; }
			}
			public override int VerticalDisplayPosition
			{
				get { return ParentControl.DisplayRectangle.Y; }
			}
		}
		private class VScrollProperties : ScrollProperties
		{
			public VScrollProperties(ScrollableView container) : base(container) { }

			public override int HorizontalDisplayPosition
			{
				get { return ParentControl.DisplayRectangle.X; }
			}
			public override int Orientation
			{
				get { return 1; }
			}
			public override int PageSize
			{
				get { return ParentControl.ClientRectangle.Height; }
			}
			public override int VerticalDisplayPosition
			{
				get { return -Value; }
			}
		}
		public enum ScrollFlags
		{
			/// <summary>
			/// when this flag is set, scrolling is accelerated by win32,
			/// i.e. not the whole area has to be redrawn
			/// </summary>
			OptimizedScroll = 1,
			/// <summary>
			/// when this flag is set, mouse wheel is scrolling the view
			/// </summary>
			MouseWheelScroll = 2,
			/// <summary>
			/// all flags
			/// </summary>
			All = OptimizedScroll | MouseWheelScroll
		}
		#endregion
		#region variables
		//scrollbars
		private HScrollProperties _hscroll;
		private VScrollProperties _vscroll;
		//data
		private Rectangle _displayRect = Rectangle.Empty;
		private Size _autoScrollMinSize = Size.Empty;
		private bool _dragfullwindows = false;
		private ScrollFlags _flags = ScrollFlags.All;
		#endregion
		#region ctor
		/// <summary>
		/// ctor
		/// </summary>
		public ScrollableView()
		{
			_hscroll = new HScrollProperties(this);
			_vscroll = new VScrollProperties(this);
		}
		#endregion
		#region helpers
		/// <summary>
		/// synchronizes the appearance of the scrollbars
		/// </summary>
		protected virtual void SyncScrollbars()
		{
			if (!IsHandleCreated) return;
			if (_hscroll.Visible)
			{
				_hscroll.Maximum = _displayRect.Width - 1;
				_hscroll.SmallChange = 5;
				_hscroll.Value = -_displayRect.X;
				_hscroll.UpdateScrollInfo();
			}
			if (_vscroll.Visible)
			{
				_vscroll.Maximum = _displayRect.Height - 1;
				_vscroll.SmallChange = 5;
				_vscroll.Value = -_displayRect.Y;
				_vscroll.UpdateScrollInfo();
			}
		}
		/// <summary>
		/// shows or hides the scrollbars
		/// </summary>
		private void SetVisibleScrollbars(bool horiz, bool vert)
		{
			if (horiz == _hscroll.Visible && vert == _vscroll.Visible)
				return;
			int x = _displayRect.X,
				y = _displayRect.Y;
			//check which scrollbar is to be hidden
			_hscroll.Visible = horiz;
			if (!horiz)
			{
				x = 0;
				_hscroll.Value = 0;
			}
			_vscroll.Visible = vert;
			if (!vert)
			{
				y = 0;
				_vscroll.Value = 0;
			}
			//update visibility
			SetDisplayRectLocation(x, y);
			UpdateStyles();
		}
		/// <summary>
		/// applies changes made to the autoscrollminsize
		/// or the clientarea of the control
		/// </summary>
		private void ApplyScrollBarChanges()
		{
			Rectangle clientrect = ClientRectangle,
				displayrect = clientrect;
			//evaluate sizes
			if (_hscroll.Visible)
				clientrect.Height += SystemInformation.HorizontalScrollBarHeight;
			if (_vscroll.Visible)
				clientrect.Width += SystemInformation.VerticalScrollBarWidth;
			if (!_autoScrollMinSize.IsEmpty)
				displayrect.Size = _autoScrollMinSize;
			//hide / show scrollbars
			SetVisibleScrollbars(
				clientrect.Width < displayrect.Width,
				clientrect.Height < displayrect.Height);
			SetDisplayRectangleSize(displayrect.Size);
			SyncScrollbars();
		}
		/// <summary>
		/// sets the specified size and scrolls, if needed
		/// </summary>
		private void SetDisplayRectangleSize(Size value)
		{
			if (_displayRect.Size == value)
				return;
			_displayRect.Size = value;
			//evaluate delta
			int widthdelta = Math.Min(0, ClientRectangle.Width - value.Width),
				heightdelta = Math.Min(0, ClientRectangle.Height - value.Height);
			//evaluate position
			int x = 0, y = 0;
			if (_hscroll.Visible)
				x = Math.Max(_displayRect.X, widthdelta);
			if (_vscroll.Visible)
				y = Math.Max(_displayRect.Y, heightdelta);
			//update location
			SetDisplayRectLocation(x, y);
		}
		/// <summary>
		/// applies the scrollbar position
		/// </summary>
		protected virtual void SetDisplayRectLocation(int x, int y)
		{
			//evaluate delta
			int widthdelta = Math.Min(0, ClientRectangle.Width - _displayRect.Width),
				heightdelta = Math.Min(0, ClientRectangle.Height - _displayRect.Height);
			//evaluate new coordinates
			x = Math.Max(widthdelta, Math.Min(0, x));
			y = Math.Max(heightdelta, Math.Min(0, y));
			//scroll
			if (IsHandleCreated &&
				(_displayRect.X != x || _displayRect.Y != y))
			{
				if ((_flags & ScrollFlags.OptimizedScroll) != 0)
				{
					Win32.RECT rectClip = ClientRectangle,
						rectUpdate = ClientRectangle;
					Win32.ScrollWindowEx(new HandleRef(this, Handle),
						x - _displayRect.X, y - _displayRect.Y, null,
						ref rectClip, new HandleRef(null, IntPtr.Zero),
						ref rectUpdate, Win32.SW_ERASE | Win32.SW_INVALIDATE);
				}
				//write
				_displayRect.X = x; _displayRect.Y = y;
			}
		}
		#endregion
		#region controller
		protected override void OnLayout(LayoutEventArgs levent)
		{
			base.OnLayout(levent);
			ApplyScrollBarChanges();
		}
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if ((_flags & ScrollFlags.MouseWheelScroll) != 0)
			{
				if (_vscroll.Visible)
				{
					int y =
						Math.Min(-(ClientRectangle.Height - _displayRect.Height),
						Math.Max(-_displayRect.Y - e.Delta, 0));
					SetDisplayRectLocation(_displayRect.X, -y);
					SyncScrollbars();
				}
				else if (_hscroll.Visible)
				{
					int x =
						Math.Min(-(ClientRectangle.Width - _displayRect.Width),
						Math.Max(-_displayRect.X - e.Delta, 0));
					SetDisplayRectLocation(-x, _displayRect.Y);
					SyncScrollbars();
				}
			}
			base.OnMouseWheel(e);
		}
		protected virtual void OnSettingChange(EventArgs e)
		{
			if (SettingChange != null)
				SettingChange(this, e);
		}
		protected virtual void OnScroll(ScrollEventArgs se)
		{
			if (Scroll != null)
				Scroll(this, se);
		}
		#endregion
		#region config functions
		/// <summary>
		/// sets or erases the given scroll flags
		/// </summary>
		public void SetScrollFlags(ScrollFlags flags, bool value)
		{
			if (value)
				_flags |= flags;
			else _flags &= ~flags;
		}
		#endregion
		#region win32
		//check flags for scrollbar visibility
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams createParams = base.CreateParams;
				//show hscrollbar
				if (_hscroll.Visible)
					createParams.Style |= 0x100000;
				else
					createParams.Style &= -1048577;
				//show vscrollbar
				if (_vscroll.Visible)
					createParams.Style |= 0x200000;
				else
					createParams.Style &= -2097153;
				//
				return createParams;
			}
		}
		//call user methods
		private void WmOnScroll(ref Message m, int value)
		{
			ScrollEventType type = (ScrollEventType)(m.WParam.ToInt32() & 0xffff);
			if (type != ScrollEventType.EndScroll)
			{
				ScrollEventArgs se = new ScrollEventArgs(type, value);
				this.OnScroll(se);
			}
		}
		//scrolled horizontal bar
		private void WmHScroll(ref Message m)
		{
			if (m.LParam != IntPtr.Zero)
			{
				base.WndProc(ref m);
				return;
			}
			int newValue = -_displayRect.X,
				oldValue = newValue,
				maximum = -(this.ClientRectangle.Width - _displayRect.Width);

			switch (m.WParam.ToInt32() & 0xffff)
			{
				case 0://button downwards
					newValue = Math.Max(0, oldValue - _hscroll.SmallChange); break;
				case 1://button upwards
					newValue = Math.Min(maximum, oldValue + _hscroll.SmallChange); break;
				case 2://band downwards
					newValue = Math.Max(0, oldValue - _hscroll.PageSize); break;
				case 3://bandupwards
					newValue = Math.Min(maximum, oldValue + _hscroll.PageSize); break;
				case 4://scroll, only on dragfullwins
				case 5://release
					newValue = _hscroll.GetScrollThumbPosition(); break;
				case 6://pos1
					newValue = 0; break;
				case 7://end
					newValue = maximum; break;
			}
			if (_dragfullwindows || (m.WParam.ToInt32() & 0xffff) != 4)
			{
				SetDisplayRectLocation(-newValue, _displayRect.Y);
				SyncScrollbars();
			}
			this.WmOnScroll(ref m, newValue);
		}
		//scrolling vertical bar
		private void WmVScroll(ref Message m)
		{
			if (m.LParam != IntPtr.Zero)
			{
				base.WndProc(ref m);
				return;
			}
			int newValue = -_displayRect.Y,
				oldValue = newValue,
				maximum = -(this.ClientRectangle.Height - _displayRect.Height);

			switch (m.WParam.ToInt32() & 0xffff)
			{
				case 0://button downwards
					newValue = Math.Max(0, oldValue - _vscroll.SmallChange); break;
				case 1://button upwards
					newValue = Math.Min(maximum, oldValue + _vscroll.SmallChange); break;
				case 2://band downwards
					newValue = Math.Max(0, oldValue - _vscroll.PageSize); break;
				case 3://bandupwards
					newValue = Math.Min(maximum, oldValue + _vscroll.PageSize); break;
				case 4://scroll, only on dragfullwins
				case 5://release
					newValue = _vscroll.GetScrollThumbPosition(); break;
				case 6://pos1
					newValue = 0; break;
				case 7://end
					newValue = maximum; break;
			}
			if (_dragfullwindows || (m.WParam.ToInt32() & 0xffff) != 4)
			{
				SetDisplayRectLocation(_displayRect.X, -newValue);
				SyncScrollbars();
			}
			this.WmOnScroll(ref m, newValue);
		}
		//hook if user enables dragfullwindows
		private void WmSettingChange(ref Message m)
		{
			base.WndProc(ref m);
			_dragfullwindows = SystemInformation.DragFullWindows;
		}
		//filter scroll messages
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case 0x114:
					this.WmHScroll(ref m);
					return;

				case 0x115:
					this.WmVScroll(ref m);
					return;

				case 0x1a:
					this.WmSettingChange(ref m);
					return;
			}
			base.WndProc(ref m);
		}
		#endregion
		#region properties
		/// <summary>
		/// gets if the horizontal scrollbar is visible
		/// </summary>
		public bool HScroll
		{
			get { return _hscroll.Visible; }
		}
		/// <summary>
		/// gets if the vertical scrollbar is visible
		/// </summary>
		public bool VScroll
		{
			get { return _vscroll.Visible; }
		}
		/// <summary>
		/// gets or sets the autoscrollposition
		/// </summary>
		[DefaultValue(typeof(Point), "0,0")]
		public Point AutoScrollPosition
		{
			get
			{
				if (_displayRect.IsEmpty)
					return Point.Empty;
				return _displayRect.Location;
			}
			set
			{
				if (!Created)
					return;
				SetDisplayRectLocation(-value.X, -value.Y);
				SyncScrollbars();
			}
		}
		[DefaultValue(typeof(Size), "0,0")]
		public Size AutoScrollMinSize
		{
			get { return _displayRect.Size; }
			set
			{
				if (value.Width < 0) value.Width = 0;
				if (value.Height < 0) value.Height = 0;
				if (_displayRect.Size == value) return;
				_autoScrollMinSize = value;
				PerformLayout();
			}
		}
		/// <summary>
		/// gets the display rectangle
		/// </summary>
		public override Rectangle DisplayRectangle
		{
			get
			{
				Rectangle disp = ClientRectangle;
				if (!_displayRect.IsEmpty)
				{
					disp.Location = _displayRect.Location;
					if (_hscroll.Visible)
						disp.Width = _displayRect.Width;
					if (_vscroll.Visible)
						disp.Height = _displayRect.Height;
				}
				return disp;
			}
		}
		#endregion
		#region events
		/// <summary>
		/// thrown when content is scrolled
		/// </summary>
		public event ScrollEventHandler Scroll;
		/// <summary>
		/// thrown when windows settings changed
		/// </summary>
		public event EventHandler SettingChange;
		#endregion
	}
}

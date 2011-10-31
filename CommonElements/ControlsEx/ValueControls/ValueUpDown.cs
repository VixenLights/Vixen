using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CommonElements.ControlsEx.ValueControls
{
	/// <summary>
	/// ValueUpDown is a value spin and edit control
	/// with dropdown tracker
	/// </summary>
	[DefaultEvent("ValueChanged")]
	[DefaultProperty("Value")]
	public class ValueUpDown : ValueControl
	{
		#region types
		private class TrackerPopupForm:PopupForm
		{
			#region variables
			private MiniTracker _tracker;
			private Orientation _trackerorientation=
				Orientation.Vertical;
			#endregion
			public TrackerPopupForm()
			{
				AddTracker(_trackerorientation);
			}
			#region helpers
			private void AddTracker(Orientation value)
			{
				if(value==Orientation.Horizontal)
				{
					this._tracker = new HMiniTracker();
					this._tracker.Size = new Size(120, 25);
					//
					this.Controls.Add(this._tracker);
					this.ClientSize = new Size(120, 25);
				}
				else
				{
					this._tracker = new VMiniTracker();
					this._tracker.Size = new Size(25, 120);
					//
					this.Controls.Add(this._tracker);
					this.ClientSize = new Size(25, 120);
				}
				this._tracker.Dock = DockStyle.Fill;
				this._tracker.MouseUp+=new MouseEventHandler(_tracker_MouseUp);
				this._tracker.ValueChanged+=new ValueChangedEH(_tracker_ValueChanged);
			}
			#endregion
			#region controller
			private void _tracker_MouseUp(object sender, MouseEventArgs e)
			{
				this.Close();
			}
			private void _tracker_ValueChanged(ValueControl sender, ValueChangedEventArgs e)
			{
				if(ValueChanged!=null)
					ValueChanged(sender,e);
			}
			#endregion
			#region members
			public void Assign(int value, int maximum, int minimum)
			{
				_tracker.Assign(value,maximum,minimum);
			}
			public void ShowUp(ValueUpDown control)
			{
				if(control==null)
					throw new ArgumentNullException("control");
				_tracker.Assign(control.Value, control.Maximum, control.Minimum);
				if (_trackerorientation == Orientation.Horizontal) {
					this.Width = Math.Max(32, control.Width + 24);
					//set mouse
					ShowByControl(control, control.PointToScreen(new Point(0, 22)));
					Point pt = _tracker.GetTrackerPos();
					Cursor.Position = _tracker.PointToScreen(pt);
					Win32.PostMessage(_tracker.Handle, Win32.WM_LBUTTONDOWN, 0, (pt.Y << 16) | (pt.X - 1));
				} else {
					this.Height = Math.Max(32, control.Width + 24);
					//set mouse
					Point mouse = Control.MousePosition;
					Point pt = _tracker.GetTrackerPos();
					mouse = new Point(mouse.X - pt.X, mouse.Y - pt.Y);
					ShowByControl(control, mouse);
					Win32.PostMessage(_tracker.Handle, Win32.WM_LBUTTONDOWN, 0, (pt.Y << 16) | (pt.X - 1));
				}
			}
			#endregion
			#region properties
			public Orientation TrackerOrientation
			{
				get{return _trackerorientation;}
				set
				{
					if(value==_trackerorientation) return;
					_trackerorientation=value;
					//remove old tracker
					if(_tracker!=null && this.Controls.Contains(_tracker))
					{
						this._tracker.MouseUp-=new MouseEventHandler(_tracker_MouseUp);
						this._tracker.ValueChanged-=new ValueChangedEH(_tracker_ValueChanged);
						this.Controls.Remove(_tracker);
						_tracker.Dispose();
					}
					//add new tracker
					AddTracker(value);
				}
			}
			public int Value
			{
				get{return _tracker.Value;}
			}
			public event ValueChangedEH ValueChanged;
			#endregion
		}
		#endregion
		#region variables
		private TrackerPopupForm _trackerpopup;
		private TextBox _textbox;
		private Timer _timer;
		private bool _mouseentered = false, _dropped = false;
		private ElementInfo[] _elements = new ElementInfo[]
			{
				new ElementInfo(),new ElementInfo(),new ElementInfo()
			};
		private int _currelement = -1;
		#endregion
		/// <summary>
		/// ctor
		/// </summary>
		public ValueUpDown()
		{
			#region container+tracker
			this._trackerpopup = new TrackerPopupForm();
			this._trackerpopup.ValueChanged+=new ValueChangedEH(_tracker_ValueChanged);
			this._trackerpopup.VisibleChanged += new EventHandler(_trackerpopup_VisibleChanged);
			this._trackerpopup.TrackerOrientation = TrackerOrientation;
			#endregion
			#region textbox
			this._textbox = new TextBox();
			this._textbox.Size = new Size(this.Width - 32, 20);
			this._textbox.Location = new Point(2, 4);
			this._textbox.BorderStyle = BorderStyle.None;
			this._textbox.Text = this.Minimum.ToString();
			this._textbox.BackColor = Color.White;
			this._textbox.MaxLength = 5;
			this._textbox.KeyDown += new KeyEventHandler(_textbox_KeyDown);
			this._textbox.KeyPress += new KeyPressEventHandler(_textbox_KeyPress);
			this.Controls.Add(this._textbox);
			#endregion
			#region timer
			this._timer = new Timer();
			this._timer.Tick += new EventHandler(_timer_Tick);
			#endregion
			this.Size = new Size(72, 25);
			this.SetStyle(ControlStyles.ResizeRedraw |
				ControlStyles.FixedHeight |
				ControlStyles.DoubleBuffer, true);
		}
		protected override void Dispose(bool disposing)
		{
			this._timer.Stop();
			_trackerpopup.Dispose();
			base.Dispose(disposing);
		}
		#region helpers
		/// <summary>
		/// evaluates which element lies under the cursor
		/// </summary>
		/// <param name="mouseloc">position of the cursor, in screen coordinates</param>
		private int HitElement(int x, int y)
		{
			for (int i = 0; i < _elements.Length; i++)
			{
				if (_elements[i].Bounds.Contains(x,y) &&
					_elements[i].State != ElementState.disabled)
					return i;
			}
			return -1;
		}
		/// <summary>
		/// updates the textbox, that it fits the value
		/// </summary>
		private void UpdateTextBox()
		{
			this._textbox.MaxLength = Math.Max(
				Maximum.ToString().Length,
				Minimum.ToString().Length) + 2;
			this._textbox.Text = Value.ToString();
		}
		/// <summary>
		/// tries to parse an integer and writes the result
		/// to the specified variable, if successful
		/// </summary>
		private bool TryParseInteger(string txt, ref int result)
		{
			double rs;
			if (double.TryParse(txt, System.Globalization.NumberStyles.Integer,
				null, out rs))
			{
				result = (int)rs;
				return true;
			}
			return false;
		}
		#endregion
		#region controller
		#region general
		// makes sure, the height is constant
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore(x, y, Math.Max(48, width), 25, specified);
		}
		// adjusts the size of all child elements
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			_elements[0].Bounds = new Rectangle(this.Width - 16, 1, 15, 20);
			_elements[1].Bounds = new Rectangle(this.Width - 27, 1, 11, 10);
			_elements[2].Bounds = new Rectangle(this.Width - 27, 11, 11, 10);
			this._textbox.Width = this.Width - 32;
		}
		// checks if all child elements follow the enabled state
		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			if (!this.Enabled) this._timer.Stop();
			ElementState state = this.Enabled ? ElementState.normal : ElementState.disabled;
			for (int i = 0; i < _elements.Length; i++)
				_elements[i].State = state;
			_currelement = -1;
			this.Refresh();
		}
		// draws all elements
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(SystemColors.Control);
			if (this.Height < 24 || this.Width < 24) return;
			IntPtr data = Win32.OpenThemeData2(this.Handle, "Combobox");
			if (data != IntPtr.Zero)//draw with winxp themes
			{
				IntPtr hdc = e.Graphics.GetHdc();
				#region background, dropdown button
				Win32.RECT rct = new Win32.RECT(0, 0, this.Width, 22);
				Win32.DrawThemeBackground2(data, hdc, 0, _mouseentered ? 2 : 1, ref rct);
				rct.Left = rct.Right - 15; rct.Top += 2; rct.Bottom -= 2; rct.Right -= 2;
				Win32.DrawThemeBackground2(data, hdc, 1, (int)_elements[0].State, ref rct);
				Win32.CloseThemeData2(data);
				#endregion
				#region spin buttons
				data = Win32.OpenThemeData2(this.Handle, "Spin");
				if (data == IntPtr.Zero)
					goto final;
				rct.Left -= 13; rct.Width = 13; rct.Top--; rct.Height = 10;
				Win32.DrawThemeBackground2(data, hdc, 1, (int)_elements[1].State, ref rct);
				rct.Top += 10; rct.Bottom += 10;
				Win32.DrawThemeBackground2(data, hdc, 2, (int)_elements[2].State, ref rct);
				Win32.CloseThemeData2(data);
				#endregion
				#region progress bar
				data = Win32.OpenThemeData2(this.Handle, "Progress");
				if (data == IntPtr.Zero)
					goto final;
				rct.Bottom = this.Height; rct.Top = rct.Bottom - 3;
				rct.Left = 0; rct.Right = GetPercentage(this.Width);
				Win32.DrawThemeBackground2(data, hdc, 3, 1, ref rct);
				Win32.CloseThemeData2(data);
				#endregion
			final:
				e.Graphics.ReleaseHdc(hdc);
			}
			else//draw without winxp themes
			{
				#region background, dropdown button
				Rectangle rct = new Rectangle(0, 0, this.Width, 22);
				e.Graphics.FillRectangle(Brushes.White, rct);
				e.Graphics.DrawRectangle(_mouseentered ? SystemPens.Highlight : SystemPens.ControlDark,
					rct.X, rct.Y, rct.Width - 1, rct.Height - 1);
				rct.X = this.Width - 15; rct.Y = 1; rct.Height -= 2; rct.Width = 15;
				ControlPaint.DrawComboButton(e.Graphics, rct, _elements[0].ToButtonState());
				#endregion
				#region spin button
				rct.X -= 13; rct.Width = 13; rct.Height = 10;
				ControlPaint.DrawScrollButton(e.Graphics, rct, ScrollButton.Up, _elements[1].ToButtonState());
				rct.Y += 10;
				ControlPaint.DrawScrollButton(e.Graphics, rct, ScrollButton.Down, _elements[2].ToButtonState());
				#endregion
				#region progress bar
				rct.Y = this.Height - 3; rct.Height = 3;
				rct.X = 0; rct.Width = GetPercentage(this.Width);
				e.Graphics.FillRectangle(SystemBrushes.Highlight, rct);
				#endregion
			}
		}
		#endregion
		#region frame highlighting
		// frame is higlighted on mouseenter
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			if (!this.Enabled || _mouseentered) return;
			_mouseentered = true;
			this.Refresh();
		}
		// frame falls back on mouse leave
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (!_mouseentered || _dropped ||
				this.RectangleToScreen(this.ClientRectangle).Contains(Control.MousePosition)) return;
			_mouseentered = false;
			if (_currelement != -1)
			{
				_elements[_currelement].State = ElementState.normal;
				_currelement = -1;
			}
			this.Refresh();
		}
		#endregion
		#region textbox
		// accept only digits and control keys
		void _textbox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
				e.Handled = true;
		}
		// interrupt the return key
		void _textbox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				e.Handled = true;
				this.OnTextBoxReturn();
			}
		}
		#endregion
		#region mouse actions
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!this.Enabled || _dropped) return;
			_currelement = HitElement(e.X,e.Y);
			if (_currelement != -1)
			{
				_elements[_currelement].State = ElementState.pushed;
				Refresh(_elements[_currelement].Bounds);
				switch (_currelement)
				{
					case 0: this.OnDropDownPressed(); break;
					case 1: this.OnSpinUp(); break;
					case 2: this.OnSpinDown(); break;
				}
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!this.Enabled || _dropped || e.Button != MouseButtons.None) return;
			int curr = HitElement(e.X,e.Y);
			if (curr == _currelement) return;
			if (curr != -1)
			{
				_elements[curr].State = ElementState.hot;
				this.Invalidate(Rectangle.Inflate(_elements[curr].Bounds,1,1));
			}
			if (_currelement != -1)
			{
				_elements[_currelement].State = ElementState.normal;
				this.Invalidate(Rectangle.Inflate(_elements[_currelement].Bounds,1,1));
			}
			_currelement = curr;
			this.Update();
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (!this.Enabled || _dropped || _currelement == -1) return;
			this.OnButtonUp();
			int curr = HitElement(e.X,e.Y);
			if (curr == _currelement)
			{
				_elements[_currelement].State = ElementState.hot;
			}
			else if (curr != -1)
			{
				_elements[_currelement].State = ElementState.normal;
				_elements[curr].State = ElementState.hot;
				this.Invalidate(_elements[curr].Bounds);
			}
			else
			{
				_elements[_currelement].State = ElementState.normal;
			}
			this.Invalidate(_elements[_currelement].Bounds);
			this.Update();
		}
		#endregion
		#region buttons
		// spin up
		private void OnSpinUp()
		{
			this.SetValueRaise(Value + 1);
			this._timer.Interval = 400;
			this._timer.Start();
		}
		// spin down
		private void OnSpinDown()
		{
			this.SetValueRaise(Value - 1);
			this._timer.Interval = 400;
			this._timer.Start();
		}
		// deactivates the timer
		private void OnButtonUp()
		{
			this._timer.Stop();
		}
		// timer proceeds to fire spins, and speeds up continously
		void _timer_Tick(object sender, EventArgs e)
		{
			this._timer.Interval = Math.Max(10, this._timer.Interval / 2);
			switch (_currelement)
			{
				case 1:
					if (!this.SetValueRaise(Value + 1))
						this._timer.Stop();
					break;
				case 2:
					if (!this.SetValueRaise(Value - 1))
						this._timer.Stop();
					break;
				default:
					this._timer.Stop(); break;
			}
		}
		// sets the value that is in the textbox
		private void OnTextBoxReturn()
		{
			int val = 0;
			if (TryParseInteger(this._textbox.Text, ref val) && base.SetValueCore(val))
				this.RaiseValueChanged();
			else
				this._textbox.Text = Value.ToString();
		}
		#endregion
		#region popup tracker
		// sets the value in the dropdown tracker
		private void _tracker_ValueChanged(ValueControl sender, ValueChangedEventArgs e)
		{
			this.SetValueRaise(_trackerpopup.Value);
		}
		// shows the tracker and attaches the mouse to it
		private void OnDropDownPressed()
		{
			_dropped = true;
			_trackerpopup.ShowUp(this);
		}
		void _trackerpopup_VisibleChanged(object sender, EventArgs e)
		{
			if (!_trackerpopup.Visible)
			{
				_dropped = false;
				if (!this.Enabled) return;
				if (this.RectangleToScreen(this.ClientRectangle).
					Contains(Control.MousePosition))
				{
					_elements[0].State = ElementState.normal;
					this.Refresh(_elements[0].Bounds);
				}
				else
					this.OnMouseLeave(EventArgs.Empty);
			}
		}
		#endregion
		#endregion
		#region set range
		protected override void OnSetMaximum()
		{
			this.UpdateTextBox();
			this.Refresh();
		}
		protected override void OnSetMinimum()
		{
			this.UpdateTextBox();
			this.Refresh();
		}
		protected override void OnAfterSetValue()
		{
			this._textbox.Text=Value.ToString();
			this.Refresh();
		}
		/// <summary>
		/// sets the value, updates the textbox
		/// and raises the valuechanged event
		/// </summary>
		public bool SetValueRaise(int value)
		{
			if (base.SetValueCore(value))
			{
				this._textbox.Text = value.ToString();
				base.RaiseValueChanged();
				this.Refresh();
				return true;
			}
			return false;
		}
		#endregion
		#region properties
		[DefaultValue(Orientation.Horizontal)]
		public Orientation TrackerOrientation
		{
			get{return _trackerpopup.TrackerOrientation;}
			set{_trackerpopup.TrackerOrientation=value;}
		}
		#endregion
	}
}

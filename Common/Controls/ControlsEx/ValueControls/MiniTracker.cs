using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Common.Controls.ControlsEx.ValueControls
{
	/// <summary>
	/// encapsulates a trackbar
	/// </summary>
	public abstract class MiniTracker : ValueControl
	{
		#region variables

		protected int _offsetx, _offsety;
		protected ElementInfo _tracker;
		private readonly ToolTip toolTip;

		#endregion

		/// <summary>
		/// ctor
		/// </summary>
		public MiniTracker()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
			              ControlStyles.DoubleBuffer |
			              ControlStyles.UserPaint |
			              ControlStyles.ResizeRedraw, true);
			_tracker = new ElementInfo();
			_tracker.State = ElementState.hot;
			_tracker.Bounds = new Rectangle(0, this.Height/2 - 7, 11, 20);
			toolTip = new ToolTip();
			MouseHover += MiniTracker_MouseHover;
			MouseLeave += MiniTracker_MouseLeave;
			ValueChanged += MiniTracker_ValueChanged;
			UpdateTrackerPosition();
		}

		#region helper

		/// <summary>
		/// sets the value of the tracker according to the position
		/// </summary>
		protected abstract bool SetValue(int x, int y);

		protected abstract void UpdateTrackerPosition();

		protected override void OnAfterSetValue()
		{
			UpdateTrackerPosition();
		}

		/// <summary>
		/// gets the position of the center of the tracker
		/// </summary>
		public Point GetTrackerPos()
		{
			Point pt = _tracker.Bounds.Location;
			pt.Offset(_tracker.Bounds.Width/2,
			          _tracker.Bounds.Height/2);
			return pt;
		}

		#endregion

		#region controller

		// make sure the tracker is aligned correct
		protected override void OnSizeChanged(EventArgs e)
		{
			this.UpdateTrackerPosition();
			base.OnSizeChanged(e);
		}

		// makes sure the tracker is enabled correct
		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			_tracker.State = this.Enabled
			                 	? ElementState.hot
			                 	: ElementState.disabled;
			this.Refresh();
		}

		#region mouse actions

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (!this.Enabled) return;
			if (_tracker.Bounds.Contains(e.X, e.Y)) {
				_offsetx = _tracker.Bounds.X - e.X;
				_offsety = _tracker.Bounds.Y - e.Y;
			}
			else {
				_offsetx = -_tracker.Bounds.Width/2;
				_offsety = -_tracker.Bounds.Height/2;
				this.SetValue(e.X + _offsetx, e.Y + _offsetx);
			}
			_tracker.State = ElementState.pushed;
			this.Refresh();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (!this.Enabled || e.Button != MouseButtons.Left || _tracker.State != ElementState.pushed) return;
			if (this.SetValue(e.X + _offsetx, e.Y + _offsety))
				this.Refresh();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if (!this.Enabled) return;
			_tracker.State = ElementState.hot;
			_offsetx = _offsety = 0;
			this.Refresh();
		}

		#endregion

		#endregion

		/// <summary>
		/// sets the specified values of the tracker
		/// </summary>
		public void Assign(int value, int maximum, int minimum)
		{
			maximum = Math.Max(maximum, minimum + 1);
			value = Math.Max(minimum, Math.Min(maximum, value));
			base.SetMinimumCore(minimum);
			base.SetMaximumCore(maximum);
			base.SetValueCore(value);
		}

		private void MiniTracker_MouseHover(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(toolTip.GetToolTip(this)))
			{
				Point point = new Point(PointToClient(MousePosition).X, PointToClient(MousePosition).Y - 20);
				toolTip.Show(Value.ToString(), this, point);	
			}
			
		}

		private void MiniTracker_MouseLeave(object sender, EventArgs e)
		{
			toolTip.Hide(this);
		}

		void MiniTracker_ValueChanged(ValueControl sender, ValueChangedEventArgs e)
		{
			if (!string.IsNullOrEmpty(toolTip.GetToolTip(this)))
			{
				toolTip.Hide(this);
				Point point = new Point(PointToClient(MousePosition).X, PointToClient(MousePosition).Y - 20);
				toolTip.Show(Value.ToString(), this, point);
			}
		}

	}

	/// <summary>
	/// horizontal implementation of the minitracker control
	/// </summary>
	public class HMiniTracker : MiniTracker
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			IntPtr data = Win32.OpenThemeData2(this.Handle, "Trackbar");
			if (data != IntPtr.Zero) {
				IntPtr hdc = e.Graphics.GetHdc();
				Win32.RECT rct = new Win32.RECT(
					_tracker.Bounds.Width/2, this.Height/2 - 2,
					this.Width - _tracker.Bounds.Width, 4);
				Win32.DrawThemeBackground2(data, hdc, 1, 1, ref rct);
				rct = _tracker.Bounds;
				Win32.DrawThemeBackground2(data, hdc, 4, 1 + (int) _tracker.State, ref rct);
				Win32.CloseThemeData2(hdc);
				e.Graphics.ReleaseHdc(hdc);
			}
			else {
				ControlPaint.DrawBorder3D(e.Graphics,
				                          _tracker.Bounds.Width/2, this.Height/2 - 2,
				                          this.Width - _tracker.Bounds.Width, 4,
				                          Border3DStyle.SunkenOuter, Border3DSide.All);
				ControlPaint.DrawButton(e.Graphics, _tracker.Bounds, ButtonState.Normal);
			}
		}

		protected override void UpdateTrackerPosition()
		{
			_tracker.Bounds.X =
				_tracker.Bounds.Width/2 + GetPercentage(
					this.Width - _tracker.Bounds.Width*2);
		}

		protected override bool SetValue(int x, int y)
		{
			if (this.Width <= this._tracker.Bounds.Width*2) return false;
			int oldvalue = Value;
			if (SetPercentage(x - this._tracker.Bounds.Width/2,
			                  this.Width - this._tracker.Bounds.Width*2)) {
				base.RaiseValueChanged();
			}
			return true;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			_tracker.Bounds.Y = (this.Height - this._tracker.Bounds.Height)/2;
			base.OnSizeChanged(e);
		}
	}

	/// <summary>
	/// horizontal implementation of the minitracker control
	/// </summary>
	public class VMiniTracker : MiniTracker
	{
		public VMiniTracker()
		{
			_tracker.Bounds.Size = new Size(20, 11);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			IntPtr data = Win32.OpenThemeData2(this.Handle, "Trackbar");
			if (data != IntPtr.Zero) {
				IntPtr hdc = e.Graphics.GetHdc();
				Win32.RECT rct = new Win32.RECT(
					this.Width/2 - 2, _tracker.Bounds.Height/2,
					4, this.Height - _tracker.Bounds.Height);
				Win32.DrawThemeBackground2(data, hdc, 1, 1, ref rct);
				rct = _tracker.Bounds;
				Win32.DrawThemeBackground2(data, hdc, 8, 1 + (int) _tracker.State, ref rct);
				Win32.CloseThemeData2(hdc);
				e.Graphics.ReleaseHdc(hdc);
			}
			else {
				ControlPaint.DrawBorder3D(e.Graphics,
				                          _tracker.Bounds.Width/2, this.Height/2 - 2,
				                          this.Width - _tracker.Bounds.Width, 4,
				                          Border3DStyle.SunkenOuter, Border3DSide.All);
				ControlPaint.DrawButton(e.Graphics, _tracker.Bounds, ButtonState.Normal);
			}
		}

		protected override void UpdateTrackerPosition()
		{
			_tracker.Bounds.Y =
				_tracker.Bounds.Height/2 + 1 + GetPercentage(
					this.Height - _tracker.Bounds.Height*2);
		}

		protected override bool SetValue(int x, int y)
		{
			if (this.Height <= this._tracker.Bounds.Height*2) return false;
			int oldvalue = Value;
			if (SetPercentage(y - _tracker.Bounds.Height/2 + 1,
			                  this.Height - _tracker.Bounds.Height*2)) {
				base.RaiseValueChanged();
			}
			return true;
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			_tracker.Bounds.X = (this.Width - this._tracker.Bounds.Width)/2;
			base.OnSizeChanged(e);
		}
	}
}
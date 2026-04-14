using Timer = System.Windows.Forms.Timer;

namespace Common.Controls.ControlsEx.ValueControls
{
	public abstract class ValueScrollBar : ValueControl
	{
		#region variablen

		protected ElementInfo[] _elems = new ElementInfo[]
		                                 	{
		                                 		new ElementInfo(ElementState.normal), new ElementInfo(ElementState.normal),
		                                 		new ElementInfo(ElementState.normal)
		                                 	};

		private int _offsetx, _offsety, _selection = -1;
		private Timer _tim;

		#endregion

		/// <summary>
		/// ctor
		/// </summary>
		public ValueScrollBar()
		{
			SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer, true);

			UpdateElementsLayout();
			_tim = new Timer();
			_tim.Tick += new EventHandler(_tim_Tick);
		}

		#region helper

		/// <summary>
		/// determines, which element is hit by the mouse location
		/// </summary>
		protected int HitElement(int x, int y)
		{
			for (int i = 0; i < 3; i++)
				if (_elems[i].Bounds.Contains(x, y))
					return i;
			return -1;
		}

		/// <summary>
		/// Updates the tracker position according to the value
		/// </summary>

		#endregion

		#region override

		protected abstract void UpdateElementsLayout();
		protected abstract void UpdateTrackerPosition();
		protected abstract void SetPosition(int x, int y);

		#endregion

		#region controller

		#region layout

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			UpdateElementsLayout();
			Refresh();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			UpdateElementsLayout();
			Refresh();
		}

		protected override void OnSetMaximum()
		{
			UpdateElementsLayout();
			Refresh();
		}

		protected override void OnSetMinimum()
		{
			UpdateElementsLayout();
			Refresh();
		}

		protected override void OnBeforeSetValue(int newvalue)
		{
			Invalidate(_elems[2].Bounds);
		}

		protected override void OnAfterSetValue()
		{
			UpdateTrackerPosition();
			Invalidate(_elems[2].Bounds);
			Update();
		}

		#endregion

		#region mouse actions

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!Enabled || e.Button != MouseButtons.Left) return;
			_selection = HitElement(e.X, e.Y);
			switch (_selection) {
				case 0:
					OnSpinDown();
					break;
				case 1:
					OnSpinUp();
					break;
				case 2:
					_offsetx = e.X - _elems[2].Bounds.X;
					_offsety = e.Y - _elems[2].Bounds.Y;
					break;
				default:
					_selection = 2;
					_offsetx = _elems[2].Bounds.Width/2;
					_offsety = _elems[2].Bounds.Height/2;
					_elems[2].State = ElementState.pushed;
					SetPosition(e.X - _offsetx, e.Y - _offsety);
					return;
			}
			_elems[_selection].State = ElementState.pushed;
			Refresh(_elems[_selection].Bounds);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!Enabled) return;
			if (e.Button == MouseButtons.None) {
				int sel = HitElement(e.X, e.Y);
				if (sel == _selection) return;
				if (sel != -1) {
					_elems[sel].State = ElementState.hot;
					Invalidate(_elems[sel].Bounds);
				}
				if (_selection != -1) {
					_elems[_selection].State = ElementState.normal;
					Invalidate(_elems[_selection].Bounds);
				}
				_selection = sel;
				Update();
			}
			else if (_selection == 2 && e.Button == MouseButtons.Left) {
				SetPosition(e.X - _offsetx, e.Y - _offsety);
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (!Enabled || e.Button != MouseButtons.Left || _selection == -1) return;
			OnButtonUp();
			int sel = HitElement(e.X, e.Y);
			if (sel == _selection) {
				_elems[_selection].State = ElementState.hot;
			}
			else if (sel != -1) {
				_elems[_selection].State = ElementState.normal;
				_elems[sel].State = ElementState.hot;
				Invalidate(_elems[sel].Bounds);
			}
			else {
				_elems[_selection].State = ElementState.normal;
			}
			Invalidate(_elems[_selection].Bounds);
			Update();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (!Enabled) return;
			if (_selection != -1) {
				_elems[_selection].State = ElementState.normal;
				Refresh(_elems[_selection].Bounds);
				_selection = -1;
			}
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			if (!Enabled) _tim.Stop();
			ElementState state = Enabled ? ElementState.normal : ElementState.disabled;
			for (int i = 0; i < _elems.Length; i++)
				_elems[i].State = state;
			_selection = -1;
			Refresh();
		}

		private void OnSpinUp()
		{
			if (SetValueCore(Value + 1))
				RaiseValueChanged();
			_tim.Interval = 400;
			_tim.Start();
		}

		private void OnSpinDown()
		{
			if (SetValueCore(Value - 1))
				RaiseValueChanged();
			_tim.Interval = 400;
			_tim.Start();
		}

		private void OnButtonUp()
		{
			_tim.Stop();
		}

		private void _tim_Tick(object sender, EventArgs e)
		{
			_tim.Interval = Math.Max(10, _tim.Interval/2);
			switch (_selection) {
				case 1:
					if (!SetValueCore(Value + 1))
						_tim.Stop();
					else
						RaiseValueChanged();
					break;
				case 0:
					if (!SetValueCore(Value - 1))
						_tim.Stop();
					else
						RaiseValueChanged();
					break;
				default:
					_tim.Stop();
					break;
			}
		}

		#endregion

		#endregion
	}

	public class HValueScrollBar : ValueScrollBar
	{
		protected override void UpdateElementsLayout()
		{
			_elems[0].Bounds = new Rectangle(0, 0, SystemInformation.HorizontalScrollBarHeight, Height);
			_elems[1].Bounds = Rectangle.FromLTRB(
				Width - _elems[0].Bounds.Width, 0, Width, Height);
			_elems[2].Bounds.Height = Height;
			using (Graphics gr = CreateGraphics()) {
				float width = Math.Max(
					gr.MeasureString(Maximum.ToString(), base.Font).Width,
					gr.MeasureString(Minimum.ToString(), base.Font).Width) + 4f;
				_elems[2].Bounds.Width = (int) width;
			}
			UpdateTrackerPosition();
		}

		protected override void UpdateTrackerPosition()
		{
			_elems[2].Bounds.Location = new Point(
				GetPercentage(_elems[1].Bounds.Left - _elems[0].Bounds.Right - _elems[2].Bounds.Width) + _elems[0].Bounds.Right,
				0);
		}

		protected override void SetPosition(int x, int y)
		{
			if (SetPercentage(x - _elems[0].Bounds.Right,
			                  _elems[1].Bounds.Left - _elems[0].Bounds.Right - _elems[2].Bounds.Width))
				RaiseValueChanged();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			IntPtr data = Win32.OpenThemeData2(Handle, "Scrollbar");
			if (data != IntPtr.Zero) //mit xp themes
			{
				IntPtr hdc = e.Graphics.GetHdc();
				Win32.RECT rct = new Win32.RECT(0, 0, Width, Height);
				Win32.DrawThemeBackground2(data, hdc, 4, 4, ref rct);
				rct = _elems[0].Bounds;
				rct.Top--;
				Win32.DrawThemeBackground2(data, hdc, 1, (int) _elems[0].State + 8, ref rct);
				rct = _elems[1].Bounds;
				rct.Top--;
				Win32.DrawThemeBackground2(data, hdc, 1, (int) _elems[1].State + 12, ref rct);
				rct = _elems[2].Bounds;
				rct.Top--;
				Win32.DrawThemeBackground2(data, hdc, 2, (int) _elems[2].State, ref rct);
				e.Graphics.ReleaseHdc(hdc);
				Win32.CloseThemeData2(data);
			}
			else //ohne xpthemes, einfache rechtecke
			{
				e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, ClientRectangle);
				ControlPaint.DrawScrollButton(e.Graphics, _elems[0].Bounds, ScrollButton.Left, _elems[0].ToButtonState());
				ControlPaint.DrawScrollButton(e.Graphics, _elems[1].Bounds, ScrollButton.Right, _elems[1].ToButtonState());
				ControlPaint.DrawButton(e.Graphics, _elems[2].Bounds, ButtonState.Normal);
			}
			using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap)) {
				fmt.LineAlignment = fmt.Alignment = StringAlignment.Center;
				e.Graphics.DrawString(Value.ToString(), base.Font,
				                      Enabled ? Brushes.Black : Brushes.Gray, _elems[2].Bounds, fmt);
			}
		}
	}

	public class VValueScrollBar : ValueScrollBar
	{
		protected override void UpdateElementsLayout()
		{
			_elems[0].Bounds = new Rectangle(0, 0, Width, SystemInformation.VerticalScrollBarWidth);
			_elems[1].Bounds = Rectangle.FromLTRB(0,
			                                      Height - _elems[0].Bounds.Height, Width, Height);
			_elems[2].Bounds.Width = Width;
			using (Graphics gr = CreateGraphics()) {
				float height = Math.Max(
					gr.MeasureString(Maximum.ToString(), base.Font).Width,
					gr.MeasureString(Minimum.ToString(), base.Font).Width) + 4f;
				_elems[2].Bounds.Height = (int) height;
			}
			UpdateTrackerPosition();
		}

		protected override void UpdateTrackerPosition()
		{
			_elems[2].Bounds.Location = new Point(
				0,
				GetPercentage(_elems[1].Bounds.Top - _elems[0].Bounds.Bottom - _elems[2].Bounds.Height) + _elems[0].Bounds.Bottom);
		}

		protected override void SetPosition(int x, int y)
		{
			if (SetPercentage(y - _elems[0].Bounds.Bottom,
			                  _elems[1].Bounds.Top - _elems[0].Bounds.Bottom - _elems[2].Bounds.Height))
				RaiseValueChanged();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			IntPtr data = Win32.OpenThemeData2(Handle, "Scrollbar");
			if (data != IntPtr.Zero) //mit xp themes
			{
				IntPtr hdc = e.Graphics.GetHdc();
				Win32.RECT rct = new Win32.RECT(0, 0, Width, Height);
				Win32.DrawThemeBackground2(data, hdc, 6, 2, ref rct);
				rct = _elems[0].Bounds;
				rct.Left--;
				Win32.DrawThemeBackground2(data, hdc, 1, (int) _elems[0].State, ref rct);
				rct = _elems[1].Bounds;
				rct.Left--;
				Win32.DrawThemeBackground2(data, hdc, 1, (int) _elems[1].State + 4, ref rct);
				rct = _elems[2].Bounds;
				rct.Left--;
				Win32.DrawThemeBackground2(data, hdc, 3, (int) _elems[2].State, ref rct);
				e.Graphics.ReleaseHdc(hdc);
				Win32.CloseThemeData2(data);
			}
			else //ohne xpthemes, einfache rechtecke
			{
				e.Graphics.FillRectangle(SystemBrushes.ControlLightLight, ClientRectangle);
				ControlPaint.DrawScrollButton(e.Graphics, _elems[0].Bounds, ScrollButton.Up, _elems[0].ToButtonState());
				ControlPaint.DrawScrollButton(e.Graphics, _elems[1].Bounds, ScrollButton.Down, _elems[1].ToButtonState());
				ControlPaint.DrawButton(e.Graphics, _elems[2].Bounds, ButtonState.Normal);
			}
			_elems[2].Bounds.X -= 3;
			using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap |
			                                           StringFormatFlags.DirectionVertical)) {
				fmt.LineAlignment = fmt.Alignment = StringAlignment.Center;

				e.Graphics.DrawString(Value.ToString(), base.Font,
				                      Enabled ? Brushes.Black : Brushes.Gray, _elems[2].Bounds, fmt);
			}
			_elems[2].Bounds.X += 3;
		}
	}
}
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CommonElements.ControlsEx.ValueControls
{
	public abstract class ValueScrollBar:ValueControl
	{
		#region variablen
		protected ElementInfo[] _elems=new ElementInfo[]
			{
				new ElementInfo(ElementState.normal),new ElementInfo(ElementState.normal),new ElementInfo(ElementState.normal)
			};
		private int _offsetx, _offsety, _selection=-1;
		private Timer _tim;
		#endregion
		/// <summary>
		/// ctor
		/// </summary>
		public ValueScrollBar()
		{
			this.SetStyle(
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer, true);

			this.UpdateElementsLayout();
			this._tim=new Timer();
			this._tim.Tick+=new EventHandler(_tim_Tick);
		}
		#region helper
		/// <summary>
		/// determines, which element is hit by the mouse location
		/// </summary>
		protected int HitElement(int x, int y)
		{
			for(int i=0; i<3; i++)
				if (_elems[i].Bounds.Contains(x,y))
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
			base.OnSizeChanged (e);
			this.UpdateElementsLayout();
			this.Refresh();
		}
		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.UpdateElementsLayout();
			this.Refresh();
		}
		protected override void OnSetMaximum()
		{
			this.UpdateElementsLayout();
			this.Refresh();
		}
		protected override void OnSetMinimum()
		{
			this.UpdateElementsLayout();
			this.Refresh();
		}
		protected override void OnBeforeSetValue(int newvalue)
		{
			this.Invalidate(_elems[2].Bounds);
		}
		protected override void OnAfterSetValue()
		{
			UpdateTrackerPosition();
			this.Invalidate(_elems[2].Bounds);
			this.Update();
		}
		#endregion
		#region mouse actions
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (!this.Enabled || e.Button!=MouseButtons.Left) return;
			_selection=this.HitElement(e.X,e.Y);
			switch(_selection)
			{
				case 0:this.OnSpinDown();break;
				case 1:this.OnSpinUp(); break;
				case 2:
					_offsetx=e.X-_elems[2].Bounds.X;
					_offsety=e.Y-_elems[2].Bounds.Y;
					break;
				default:
					_selection=2;
					_offsetx=_elems[2].Bounds.Width/2;
					_offsety=_elems[2].Bounds.Height/2;
					_elems[2].State=ElementState.pushed;
					this.SetPosition(e.X-_offsetx,e.Y-_offsety);
					return;
			}
			_elems[_selection].State=ElementState.pushed;
			this.Refresh(_elems[_selection].Bounds);
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!this.Enabled) return;
			if (e.Button==MouseButtons.None)
			{
				int sel=this.HitElement(e.X,e.Y);
				if (sel==_selection) return;
				if (sel!=-1)
				{
					_elems[sel].State=ElementState.hot;
					this.Invalidate(_elems[sel].Bounds);
				}
				if (_selection!=-1)
				{
					_elems[_selection].State=ElementState.normal;
					this.Invalidate(_elems[_selection].Bounds);
				}
				_selection=sel;
				this.Update();
			}
			else if (_selection==2 && e.Button==MouseButtons.Left)
			{
				this.SetPosition(e.X-_offsetx,e.Y-_offsety);
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (!this.Enabled || e.Button!=MouseButtons.Left || _selection==-1) return;
			this.OnButtonUp();
			int sel=this.HitElement(e.X,e.Y);
			if (sel==_selection)
			{
				_elems[_selection].State=ElementState.hot;
			}
			else if (sel!=-1)
			{
				_elems[_selection].State=ElementState.normal;
				_elems[sel].State=ElementState.hot;
				this.Invalidate(_elems[sel].Bounds);
			}
			else
			{
				_elems[_selection].State=ElementState.normal;
			}
			this.Invalidate(_elems[_selection].Bounds);
			this.Update();
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			if (!this.Enabled) return;
			if (_selection!=-1)
			{
				_elems[_selection].State=ElementState.normal;
				this.Refresh(_elems[_selection].Bounds);
				_selection=-1;
			}
		}
		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			if (!this.Enabled) this._tim.Stop();
			ElementState state = this.Enabled ? ElementState.normal : ElementState.disabled;
			for (int i = 0; i < _elems.Length; i++)
				_elems[i].State = state;
			_selection = -1;
			this.Refresh();
		}
		private void OnSpinUp()
		{
			if(SetValueCore(Value+1))
				RaiseValueChanged();
			_tim.Interval=400;
			this._tim.Start();
		}
		private void OnSpinDown()
		{
			if(SetValueCore(Value-1))
				RaiseValueChanged();
			_tim.Interval=400;
			this._tim.Start();
		}
		private void OnButtonUp()
		{
			this._tim.Stop();
		}
		private void _tim_Tick(object sender, EventArgs e)
		{
			this._tim.Interval = Math.Max(10, this._tim.Interval / 2);
			switch (_selection)
			{
				case 1:
					if (!SetValueCore(Value + 1))
						this._tim.Stop();
					else
						RaiseValueChanged();
					break;
				case 0:
					if (!SetValueCore(Value - 1))
						this._tim.Stop();
					else
						RaiseValueChanged();
					break;
				default:
					this._tim.Stop(); break;
			}
		}
		#endregion
		#endregion

	}
	public class HValueScrollBar:ValueScrollBar
	{
		protected override void UpdateElementsLayout()
		{
			_elems[0].Bounds=new Rectangle(0,0,SystemInformation.HorizontalScrollBarHeight,this.Height);
			_elems[1].Bounds=Rectangle.FromLTRB(
				this.Width-_elems[0].Bounds.Width,0,this.Width,this.Height);
			_elems[2].Bounds.Height=this.Height;
			using(Graphics gr=this.CreateGraphics())
			{
				float width=Math.Max(
					gr.MeasureString(Maximum.ToString(),base.Font).Width,
					gr.MeasureString(Minimum.ToString(),base.Font).Width)+4f;
				_elems[2].Bounds.Width=(int)width;
			}
			UpdateTrackerPosition();
		}
		protected override void UpdateTrackerPosition()
		{
			_elems[2].Bounds.Location=new Point(
				GetPercentage(_elems[1].Bounds.Left-_elems[0].Bounds.Right-_elems[2].Bounds.Width)+_elems[0].Bounds.Right,
				0);
		}
		protected override void SetPosition(int x, int y)
		{
			if (SetPercentage(x-_elems[0].Bounds.Right,
				_elems[1].Bounds.Left-_elems[0].Bounds.Right-_elems[2].Bounds.Width))
				RaiseValueChanged();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			IntPtr data=Win32.OpenThemeData2(this.Handle,"Scrollbar");
			if (data!=IntPtr.Zero)//mit xp themes
			{
				IntPtr hdc=e.Graphics.GetHdc();
				Win32.RECT rct=new Win32.RECT(0,0,this.Width,this.Height);
				Win32.DrawThemeBackground2(data,hdc,4,4,ref rct);
				rct=base._elems[0].Bounds; rct.Top--;
				Win32.DrawThemeBackground2(data,hdc,1,(int)base._elems[0].State+8,ref rct);
				rct=base._elems[1].Bounds; rct.Top--;
				Win32.DrawThemeBackground2(data,hdc,1,(int)base._elems[1].State+12,ref rct);
				rct=base._elems[2].Bounds; rct.Top--;
				Win32.DrawThemeBackground2(data,hdc,2,(int)base._elems[2].State,ref rct);
				e.Graphics.ReleaseHdc(hdc);
				Win32.CloseThemeData2(data);
			}
			else//ohne xpthemes, einfache rechtecke
			{
				e.Graphics.FillRectangle(SystemBrushes.ControlLightLight,this.ClientRectangle);
				ControlPaint.DrawScrollButton(e.Graphics,base._elems[0].Bounds,ScrollButton.Left,base._elems[0].ToButtonState());
				ControlPaint.DrawScrollButton(e.Graphics,base._elems[1].Bounds,ScrollButton.Right,base._elems[1].ToButtonState());
				ControlPaint.DrawButton(e.Graphics,base._elems[2].Bounds,ButtonState.Normal);
			}
			using(StringFormat fmt=new StringFormat(StringFormatFlags.NoWrap))
			{
				fmt.LineAlignment=fmt.Alignment=StringAlignment.Center;
				e.Graphics.DrawString(base.Value.ToString(),base.Font,
					this.Enabled?Brushes.Black:Brushes.Gray,base._elems[2].Bounds,fmt);
			}
		}
	}
	public class VValueScrollBar:ValueScrollBar
	{
		protected override void UpdateElementsLayout()
		{
			_elems[0].Bounds=new Rectangle(0,0,this.Width,SystemInformation.VerticalScrollBarWidth);
			_elems[1].Bounds=Rectangle.FromLTRB(0,
				this.Height-_elems[0].Bounds.Height,this.Width,this.Height);
			_elems[2].Bounds.Width=this.Width;
			using(Graphics gr=this.CreateGraphics())
			{
				float height=Math.Max(
					gr.MeasureString(Maximum.ToString(),base.Font).Width,
					gr.MeasureString(Minimum.ToString(),base.Font).Width)+4f;
				_elems[2].Bounds.Height=(int)height;
			}
			UpdateTrackerPosition();
		}
		protected override void UpdateTrackerPosition()
		{
			_elems[2].Bounds.Location=new Point(
				0,
				GetPercentage(_elems[1].Bounds.Top-_elems[0].Bounds.Bottom-_elems[2].Bounds.Height)+_elems[0].Bounds.Bottom);
		}
		protected override void SetPosition(int x, int y)
		{
			if(SetPercentage(y-_elems[0].Bounds.Bottom,
				_elems[1].Bounds.Top-_elems[0].Bounds.Bottom-_elems[2].Bounds.Height))
				RaiseValueChanged();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			IntPtr data=Win32.OpenThemeData2(this.Handle,"Scrollbar");
			if (data!=IntPtr.Zero)//mit xp themes
			{
				IntPtr hdc=e.Graphics.GetHdc();
				Win32.RECT rct=new Win32.RECT(0,0,this.Width,this.Height);
				Win32.DrawThemeBackground2(data,hdc,6,2,ref rct);
				rct=base._elems[0].Bounds; rct.Left--;
				Win32.DrawThemeBackground2(data,hdc,1,(int)base._elems[0].State,ref rct);
				rct=base._elems[1].Bounds; rct.Left--;
				Win32.DrawThemeBackground2(data,hdc,1,(int)base._elems[1].State+4,ref rct);
				rct=base._elems[2].Bounds; rct.Left--;
				Win32.DrawThemeBackground2(data,hdc,3,(int)base._elems[2].State,ref rct);
				e.Graphics.ReleaseHdc(hdc);
				Win32.CloseThemeData2(data);
			}
			else//ohne xpthemes, einfache rechtecke
			{
				e.Graphics.FillRectangle(SystemBrushes.ControlLightLight,this.ClientRectangle);
				ControlPaint.DrawScrollButton(e.Graphics,base._elems[0].Bounds,ScrollButton.Up,base._elems[0].ToButtonState());
				ControlPaint.DrawScrollButton(e.Graphics,base._elems[1].Bounds,ScrollButton.Down,base._elems[1].ToButtonState());
				ControlPaint.DrawButton(e.Graphics,base._elems[2].Bounds,ButtonState.Normal);
			}
			base._elems[2].Bounds.X-=3;
			using(StringFormat fmt=new StringFormat(StringFormatFlags.NoWrap |
					  StringFormatFlags.DirectionVertical))
			{
				fmt.LineAlignment=fmt.Alignment=StringAlignment.Center;

				e.Graphics.DrawString(base.Value.ToString(),base.Font,
					this.Enabled?Brushes.Black:Brushes.Gray,base._elems[2].Bounds,fmt);
			}
			base._elems[2].Bounds.X+=3;
		}
	}
}

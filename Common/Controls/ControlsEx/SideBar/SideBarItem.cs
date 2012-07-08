using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// Zusammenfassung für SideBarItem.
	/// </summary>
	[ToolboxItem(false),
	Designer(typeof(SideBarItemDesigner)),
	DefaultEvent("CollapsedChanged"),
	DefaultProperty("Caption")]
	public class SideBarItem:Panel
	{
		#region variables
		private SideBar _owner;

		private bool _collapsed=false,
			_specialgroup=false,
			_titlehighlighted=false;
		private int _position,//vertical position is prestored here
			_expandedheight,//expanedheight is prestored here
			_titlebarsize=23;
		private string _caption=null;
		private Bitmap _titleimage=null;
		#endregion
		public SideBarItem()
		{
			this.SetStyle(ControlStyles.ResizeRedraw |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserPaint |
				ControlStyles.DoubleBuffer
				, true);
			_expandedheight=this.Height-HeaderHeight;
			this._animationtimer=new Timer();
			this._animationtimer.Interval=10;
			this._animationtimer.Tick+=new EventHandler(AnimationFrame);
		}
		#region helper
		#region animation
		private Timer _animationtimer;
		private int _preheight, _endheight, _step;//used for animation
		/// <summary>
		/// starts the animation, if possible
		/// </summary>
		private void AnimationStart()
		{
			this._animationtimer.Stop();
			if(this._owner!=null &&
				this._owner.Animate &&
				this._owner.ActivateAnimation)
			{
				_preheight=this.Height;
				if (_collapsed)
					_endheight=this.HeaderHeight;
				else
					_endheight=this.HeaderHeight+_expandedheight;
				_step=0;
				this._animationtimer.Start();
			}
			else
			{
				if (_collapsed)
					base.Height=this.HeaderHeight;
				else
					base.Height=this.HeaderHeight+_expandedheight;
			}
		}
		
		private void AnimationFrame(object sender, EventArgs e)
		{
			base.Height=//compute new height
				_preheight+
				(int)(Math.Sin(Math.PI*(double)(_step)/40.0)*
				(double)(_endheight-_preheight));
			_step++;
			if (_step>20)
				this.AnimationEnd();

		}
		private void AnimationEnd()
		{
			this._animationtimer.Stop();
		}
		#endregion
		private bool TitleHighlighted
		{
			get{return _titlehighlighted;}
			set
			{
				if (_titlehighlighted==value) return;
				_titlehighlighted=value;
				if(_titlehighlighted)
					this.Cursor=Cursors.Hand;
				else
					this.Cursor=Cursors.Default;
				this.InvalidateTitleBar();
				this.Update();
			}
		}
		#endregion
		#region controller
		#region mouse
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter (e);
			this.TitleHighlighted=this.PointToClient(Control.MousePosition).Y<0;//higlight titlebar
		}
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave (e);
			this.TitleHighlighted=false;//higlight titlebar
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);
			if (e.Button==MouseButtons.None)
				this.TitleHighlighted=e.Y<0;//higlight titlebar
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			if (e.Y<0)
				this.Collapsed=!this.Collapsed;
		}
		#endregion
		#region size
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged (e);
			if(this._owner!=null)
				this._owner.DoLayout();//recalculate positions
		}
		private void LimitBounds(ref int x, ref int y, ref int width, ref int height)
		{
			if (this._owner!=null)
			{
				x=8;
				width=this._owner.GetDisplayWidth();
				y=_position+_owner.AutoScrollPosition.Y;
			}
			if(height<HeaderHeight) height=HeaderHeight;
		}
//		public new void  SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
//		{
//			this.LimitBounds(ref x, ref y, ref width, ref height);
//			base.SetBounds(x,y,width,height,specified);
//		}
		//make sure, the control size is acceptable
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			this.LimitBounds(ref x, ref y, ref width, ref height);
			base.SetBoundsCore (x, y, width, height, specified);
		}
		#endregion
		#region wndproc overrides
		#region ncpaint
		/// <summary>
		/// invalidates the titlebar
		/// </summary>
		public void InvalidateTitleBar()
		{
			Win32.RECT rct = new Win32.RECT(0, -HeaderHeight, this.Width, HeaderHeight);
			Win32.RedrawWindow(this.Handle,ref rct,IntPtr.Zero,Win32.RDW_FRAME|0x85);
		}
		/// <summary>
		/// invalidates the whole window
		/// </summary>
		public void InvalidateWhole()
		{
			Win32.RedrawWindow(this.Handle,IntPtr.Zero,IntPtr.Zero,Win32.RDW_FRAME|0x85);
			this.Update();
		}
		private void WmNCPaint(ref Message m)
		{
			base.DefWndProc(ref m);
			//get dc
			IntPtr hdc=Win32.GetDCEx(m.HWnd,m.WParam,
				Win32.DCX_WINDOW|Win32.DCX_CACHE|
				Win32.DCX_CLIPSIBLINGS|Win32.DCX_USESTYLE);
			if (hdc==IntPtr.Zero) return;
			//get bounds
			Win32.RECT bounds;
			Win32.GetWindowRect(m.HWnd,out bounds);
			if (bounds.Width<1 || bounds.Height<1) return;
			bounds.Offset(-bounds.Left,-bounds.Top);
			//process message using a double buffered graphics
			using (NCGraphics gr = new NCGraphics(hdc, bounds.Size))
			{
				this.OnNCPaint(new PaintEventArgs(gr.Graphics, bounds));
				gr.Render();
			}
			//clean up
			Win32.ReleaseDC(m.HWnd,hdc);
			m.Result=IntPtr.Zero;
		}
		protected virtual void OnNCPaint(PaintEventArgs e)
		{
			if (this._owner==null)
			{
				//draw header without parent
				SideBar.DefaultRenderer.DrawItemHeader(
					new DrawHeaderArgs(e.Graphics,e.ClipRectangle,HeaderHeight,this._caption,
					_specialgroup,_collapsed,this.Location,this.TitleHighlighted,
					this._titleimage,_titlebarsize));
			}
			else
			{
				//draw header with parent
				this._owner.Renderer.DrawItemHeader(
					new DrawHeaderArgs(e.Graphics,e.ClipRectangle,HeaderHeight,this._owner.GetContentRectangle(),
					this._caption,_specialgroup,_collapsed,this.Location,this.TitleHighlighted,
					this._titleimage,_titlebarsize));
			}
			this._owner.Renderer.DrawItemBorder(
				new DrawItemArgs(e.Graphics,e.ClipRectangle,HeaderHeight,
				_specialgroup,_collapsed));
		}
		#endregion
		#region nccalcsize
		private void WmCalcSize(ref Message m)
		{
			base.DefWndProc(ref m);
			if (m.WParam==IntPtr.Zero)
			{
				Win32.RECT rct=(Win32.RECT)m.GetLParam(typeof(Win32.RECT));
				//get new clientsize
				this.OnCalcSize(ref rct);
				Marshal.StructureToPtr(rct,m.LParam,true);
			}
			else
			{
				Win32.NCCALCSIZE_PARAMS calc=
					(Win32.NCCALCSIZE_PARAMS)m.GetLParam(typeof(Win32.NCCALCSIZE_PARAMS));
				//get new clientsize
				this.OnCalcSize(ref calc.newbounds);
				Marshal.StructureToPtr(calc,m.LParam,true);
			}
			m.Result=new IntPtr(Win32.WVR_REDRAW);
		}
		/// <summary>
		/// calculates the new clientsize
		/// </summary>
		protected virtual void OnCalcSize(ref Win32.RECT clientbounds)
		{
			clientbounds.Inflate(-1,-1);
			clientbounds.Top+=HeaderHeight-1;
		}
		#endregion
		#region nchittest
		//map mouse messages
		private void WmNCHitTest(ref Message m)
		{
			Point pt=this.PointToClient(
				new Point(m.LParam.ToInt32()));
			m.Result=new IntPtr(Win32.HTCLIENT);//map all mouse messages to client
		}
		#endregion
		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				case Win32.WM_NCPAINT:
					this.WmNCPaint(ref m); return;
				case Win32.WM_NCCALCSIZE:
					this.WmCalcSize(ref m); break;
				case Win32.WM_NCHITTEST:
					this.WmNCHitTest(ref m); return;
			}
			base.WndProc (ref m);
		}
		#endregion
		#endregion
		#region public members
		/// <summary>
		/// repositions the element
		/// </summary>
		public void RePosition()
		{
			this.SetBoundsCore(0,0,
				this.Width,this.Height,
				BoundsSpecified.All);
//			if (this.Parent!=null)
//				this.Parent.PerformLayout(this,"Bounds");

		}
		/// <summary>
		/// sets the inner height of the control and animates it
		/// </summary>
		/// <param name="value">the new height</param>
		public void SetExpandedHeightAnimate(int value)
		{
			value=Math.Max(1,value);
			if (value==_expandedheight) return;
			_expandedheight=value;
			if (_collapsed)
				this.RaiseCollapseChanged();
			this._collapsed=false;
			this.AnimationStart();
		}
		public void AdaptBackColor()
		{
			if (this._owner!=null)
				base.BackColor=this._owner.Renderer.ItemBackColor;
		}
		#endregion
		#region properties
		/// <summary>
		/// specifies the height of the title bar (16-60)
		/// </summary>
		[Description("specifies the height of the title bar (16-60)"),
		DefaultValue(23)]
		public int TitleBarSize
		{
			get{return _titlebarsize;}
			set
			{
				value=Math.Max(16,Math.Min(60,value));
				if (value==_titlebarsize) return;
				_titlebarsize=value;
				this.Height=_expandedheight+HeaderHeight;
			}
		}
		/// <summary>
		/// gets the space needed for the title bar
		/// </summary>
		[Browsable(false)]
		public int HeaderHeight
		{
			get
			{
				if (this._titleimage==null)
					return _titlebarsize;
				else
					return Math.Max(_titlebarsize,this._titleimage.Height);
			}
		}
		/// <summary>
		/// specifies the image displayed in the title bar. only sizes up to 48x48 are accepted
		/// </summary>
		[Description("specifies the image displayed in the title bar. only sizes up to 48x48 are accepted"),
		DefaultValue(null),
		Localizable(true)]
		public Bitmap CaptionImage
		{
			get{return this._titleimage;}
			set
			{
				if (this._titleimage==value) return;
				if (value!=null && (value.Height>48 || value.Width>48)) return;
				this._titleimage=value;
				this.Height=_expandedheight+HeaderHeight;
			}
		}
		/// <summary>
		/// specifies the text written on the title bar
		/// </summary>
		[Description("specifies the text written on the title bar"),
		DefaultValue(null),
		Localizable(true)]
		public string Caption
		{
			get{return this._caption;}
			set
			{
				if (this._caption==value) return;
				this._caption=value;
				this.InvalidateTitleBar();
				this.Update();
			}
		}
		/// <summary>
		/// specifies if the group is collapsed or not
		/// </summary>
		[Description("specifies if the group is collapsed or not"),
		DefaultValue(false)]
		public bool Collapsed
		{
			get{return _collapsed;}
			set
			{
				if (value==_collapsed) return;
				_collapsed=value;
				this.RaiseCollapseChanged();
				this.AnimationStart();
			}
		}
		/// <summary>
		/// specifies if the group is painted special
		/// </summary>
		[Description("specifies if the group is painted special"),
		DefaultValue(false)]
		public bool SpecialGroup
		{
			get{return _specialgroup;}
			set
			{
				if (_specialgroup==value) return;
				_specialgroup=value;
				this.InvalidateWhole();
				this.Update();
			}
		}
		/// <summary>
		/// specifies the height of the inner panel if expanded
		/// </summary>
		[Description("specifies the height of the inner panel if expanded")]
		public int ExpandedHeight
		{
			get{return _expandedheight;}
			set
			{
				if (value<1) value=0;
				if (_expandedheight==value) return;
				_expandedheight=value;
				if (_collapsed)
					this.Height=HeaderHeight;
				else
					this.Height=HeaderHeight+_expandedheight;
				if (this.Parent!=null)
					this.Parent.PerformLayout(this,"Bounds");
			}
		}
		/// <summary>
		/// do not use this member
		/// </summary>
		internal int Position
		{
			get{return _position;}
			set{_position=value;}
		}
		/// <summary>
		/// do not use this member
		/// </summary>
		internal SideBar Owner
		{
			get{return this._owner;}
			set
			{
				if (value==this._owner) return;
				this._owner=value;
				this.AdaptBackColor();
			}
		}
		#region designer serialisation overrides
		public new int Height
		{
			get{return base.Height;}
			set
			{
				if (_collapsed)value=HeaderHeight;
				base.Height=value;
			}
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Color BackColor
		{
			get{return base.BackColor;}
			set{return;}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DockStyle Dock
		{
			get{return DockStyle.None;}
			set{base.Dock=DockStyle.None;}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point Location
		{
			get{return base.Location;}
			set{base.Location=value;}
		}
		public new Size Size
		{
			get{return base.Size;}
			set
			{
				if (this.DesignMode && !_collapsed)
				_expandedheight=Math.Max(0,value.Height-HeaderHeight);
				if (_collapsed)value.Height=HeaderHeight;
				base.Size=value;
			}
		}
		#endregion
		#endregion
		#region events
		public void RaiseCollapseChanged()
		{
			if (this.CollapsedChanged!=null)
				this.CollapsedChanged(this,EventArgs.Empty);
		}
		[Description("occurs, when the control is collapsed or expanded")]
		public event EventHandler CollapsedChanged;
		#endregion
	}

}

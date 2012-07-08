using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ControlsEx.SideBar
{
	/// <summary>
	/// SideBar is a ExplorerBar implementation for .net 1.1
	/// </summary>
	[ToolboxItem(true),
	Designer(typeof(SideBarDesigner))]
	public class SideBar:ScrollableControl,ISupportInitialize
	{
		/// <summary>
		/// new controll collection which only accepts SideBaritems
		/// </summary>
		public new class ControlCollection:Control.ControlCollection
		{
			#region variables
			private SideBar _owner;
			#endregion
			public ControlCollection(SideBar owner):base(owner)
			{
				if (owner==null)
					throw new ArgumentNullException("owner");
				this._owner=owner;
			}
			#region add
			/// <summary>
			/// adds the specified control to the collection. only SideBaritems can be added
			/// </summary>
			/// <param name="value">the SideBaritem to be added</param>
			public override void Add(Control value)
			{
				AddInternal(value as SideBarItem);
				this._owner.DoLayout();
			}
			/// <summary>
			/// adds the specified array of controls to the collection. only SideBaritems can be added
			/// </summary>
			/// <param name="controls">the array of SideBaritems to be added</param>
			public override void AddRange(Control[] controls)
			{
				if (controls==null) return;
				foreach(Control ctl in controls)
				{
					AddInternal(ctl as SideBarItem);
				}
				this._owner.DoLayout();
			}
            // adds the specified SideBaritem to the collection
			private void AddInternal(SideBarItem item)
			{
				if (item==null) 
					throw new ArgumentException("Only SideBarItems can be added");
				if (item.Owner!=null)
					throw new ArgumentException("SideBarItems can only be assigned to one SideBar");
				item.Owner=this._owner;
				base.Add (item);
			}
			#endregion
			#region remove
			/// <summary>
			/// removes the specified control from the collection. only SideBaritems are contained
			/// </summary>
			/// <param name="value">the SideBaritem to be removed</param>
			public override void Remove(Control value)
			{
				SideBarItem item=value as SideBarItem;
				if (item==null || !this.Contains(item)) return;
				base.Remove (value);
				item.Owner=null;
				this._owner.DoLayout();
			}
			/// <summary>
			/// removes all controls from the collection
			/// </summary>
			public override void Clear()
			{
				foreach(SideBarItem item in this)
				{
					item.Owner=null;
				}
				base.Clear ();
			}
			#endregion
		}
		#region variables
		private bool VScrollVisible=false,
			_animate=true, _initing=false;
		private static SideBarRendererBase _defaultrenderer;
		private SideBarRendererBase _renderer;
		#endregion
		public SideBar()
		{
			this._renderer=DefaultRenderer;//provoke the creation of a DefaultRenderer
			this.SetStyle(
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw, true);
		}
		#region controller
		//draw background through renderer
		protected override void OnPaint(PaintEventArgs e)
		{
			this._renderer.DrawSideBarBackground(
				new DrawElementArgs(e.Graphics,this.GetContentRectangle()));
		}
		protected override void OnSizeChanged(EventArgs e)
		{
			this.DoLayout();
			base.OnSizeChanged (e);
		}
		//use own controlcollection which only accepts SideBaritems to be added
		protected override System.Windows.Forms.Control.ControlCollection CreateControlsInstance()
		{
			return new ControlCollection(this);
		}
		#endregion
		#region public members
		/// <summary>
		/// gets the rectangle including the SideBarItems,
		/// including the offset of autoscroll
		/// </summary>
		public Rectangle GetContentRectangle()
		{
			if (this.VScroll)
				return new Rectangle(0,this.AutoScrollPosition.Y,
					this.Width,this.AutoScrollMinSize.Height);
			else
				return this.ClientRectangle;
		}
		/// <summary>
		/// recalculates the position of all SideBaritems
		/// and displays scrollbars, if needed
		/// </summary>
		public void DoLayout()
		{
			//if (_initing) return;
			int y=8;
			foreach(SideBarItem item in this.Controls)
			{
				item.Position=y;
				y+=item.Height+8;
			}
			VScrollVisible=y>this.Height;//set own flag
			this.AutoScrollMinSize=new Size(0,y);
			foreach(SideBarItem item in this.Controls)
			{
//				item.Bounds=new Rectangle(
//					8,item.Position,this.GetDisplayWidth(),
//					item.Height);
				item.RePosition();
				item.InvalidateTitleBar();
			}
		}
		/// <summary>
		/// returns the width a contained SideBaritem can have
		/// </summary>
		public int GetDisplayWidth()
		{
			int ret=this.Width-16;
			if (VScrollVisible)
				ret-=SystemInformation.VerticalScrollBarWidth;
			return Math.Max(0,ret);
		}
		#endregion
		#region properties
		/// <summary>
		/// specifies if the SideBaritems are animated on expand and collapse
		/// </summary>
		[Description("specifies if the SideBaritems are animated on expand and collapse"),
		DefaultValue(true)]
		public bool Animate
		{
			get{return _animate;}
			set{_animate=value;}
		}
		/// <summary>
		/// returns, if animations should be enabled
		/// </summary>
		[Browsable(false)]
		public bool ActivateAnimation
		{
			get{return (!this.DesignMode) && (!_initing);}
		}
		/// <summary>
		/// gets or sets the currently selected renderer
		/// </summary>
		public SideBarRendererBase Renderer
		{
			get{return this._renderer;}
			set
			{
				if (value==this._renderer) return;
				if (value==null)
					this._renderer=DefaultRenderer;
				else
					this._renderer=value;

				foreach(SideBarItem item in this.Controls)
					item.AdaptBackColor();
				this.Refresh();
			}
		}
		/// <summary>
		/// returns the default renderer
		/// </summary>
		public static SideBarRendererBase DefaultRenderer
		{
			get
			{
				if (_defaultrenderer==null)
					_defaultrenderer=new SideBarRendererRoyale();
				return _defaultrenderer;
			}
		}
		#endregion
		#region ISupportInitialize Member
		public void BeginInit()
		{
			_initing=true;
		}
		public void EndInit()
		{
			_initing=false;
			this.DoLayout();
		}
		#endregion
	}
}

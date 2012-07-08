using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace ControlsEx.DockingFrames
{
	/// <summary>
	/// TabControl - the ultimative tabcontrol
	/// </summary>
	[ToolboxItem(true),
	Designer(typeof(TabControlDesigner)),
	DefaultProperty("Pages"),
	DefaultEvent("SelectedPageChanged")]
	public class TabControl : BorderedControl, ITabControl
	{
		#region collection
		public new class ControlCollection : Control.ControlCollection
		{
			#region variables
			private TabControl _owner;
			#endregion
			public ControlCollection(TabControl owner)
				: base(owner)
			{
				this._owner = owner;
			}
			#region add pages
			public override void Add(Control value)
			{
				TabPage page = value as TabPage;
				if (page == null) return;
				if (page.Owner != null)
					throw new Exception("Tab Page can only be assigned to one TabControl");
				base.Add(value);
				//update TabControl
				this.OnAddedPage(page);
				_owner.HeaderModule.Reload(HeaderModule.Section.Headers, page);
				if (_owner.SelectedPage == null && this.Count > 0)
					_owner.SetSelectedPageInternal((TabPage)this[0]);
				else
					_owner.HeaderModule.Refresh(HeaderModule.Section.Headers);
				_owner.HeaderModule.Reload(HeaderModule.Section.Browser, null);
			}
			public override void AddRange(Control[] controls)
			{
				if (controls == null) return;
				TabPage page;
				for (int i = 0; i < controls.Length; i++)
				{
					page = controls[i] as TabPage;
					if (page == null || page.Owner != null) continue;
					base.Add(page);
					this.OnAddedPage(page);
				}
				//update TabControl
				_owner.HeaderModule.Reload(HeaderModule.Section.Headers, null);

				if (_owner.SelectedPage == null && this.Count > 0)
					_owner.SetSelectedPageInternal((TabPage)this[0]);
				else
					_owner.HeaderModule.Refresh(HeaderModule.Section.Headers);
				_owner.HeaderModule.Reload(HeaderModule.Section.Browser, null);
			}
			protected virtual void OnAddedPage(TabPage value)
			{
				value.Visible = false;
				value.Owner = _owner;
				value.Dock = DockStyle.Fill;
				//value.Bounds=_owner.TabBounds;
				value.Closing += new EventHandler<CancelEventArgs>(_owner.Page_Closing);
				_owner.OnAddedPage(value);
				value.OnAdded();
			}
			#endregion
			#region remove page
			public override void Remove(Control value)
			{
				int index = IndexOf(value);
				base.Remove(value);
				TabPage page = value as TabPage;
				if (page == null) return;
				//update TabControl
				this.OnRemovedPage(page, index);
				_owner.HeaderModule.Reload(HeaderModule.Section.Browser, null);
			}
			protected virtual void OnRemovedPage(TabPage value, int index)
			{
				value.Closing -= new EventHandler<CancelEventArgs>(_owner.Page_Closing);
				_owner.OnRemovedPage(value);
				value.OnRemoved();
				value.Owner = null;

				_owner.HeaderModule.Reload(HeaderModule.Section.Headers, null);
				if (_owner.SelectedPage == value)
				{
					if (this.Count > 0)
						_owner.SetSelectedPageInternal((TabPage)this[Math.Max(0, index - 1)]);
					else
						_owner.SetSelectedPageInternal(null);
				}
				else
					_owner.HeaderModule.Refresh(HeaderModule.Section.Headers);
			}
			#endregion
			#region clear all
			public override void Clear()
			{
				TabPage page;
				while (Count > 0)
				{
					page = (TabPage)this[0];
					base.Remove(page);
					page.Closing -= new EventHandler<CancelEventArgs>(_owner.Page_Closing);
					_owner.OnRemovedPage(page);
					page.OnRemoved();
					page.Owner = null;
				}
				//update TabControl
				this.OnRemovedAll();
				_owner.HeaderModule.Reload(HeaderModule.Section.Browser, null);
			}
			protected virtual void OnRemovedAll()
			{
				_owner.HeaderModule.Reload(HeaderModule.Section.Headers, null);
				if (_owner.SelectedPage != null)
					_owner.SetSelectedPageInternal(null);
				else
					_owner.HeaderModule.Refresh(HeaderModule.Section.Headers);
			}
			#endregion
		}
		#endregion
		#region variables
		private TabPageCollection _coll;
		private TabPage _selected = null;
		private HeaderModule _header;
		#endregion
		public TabControl()
		{
			this.SetStyle(ControlStyles.AllPaintingInWmPaint |
				ControlStyles.DoubleBuffer |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw, true);
			this._coll = new TabPageCollection(this);
			if ((this._header = CreateHeaderModule()) == null)
				throw new ArgumentNullException("CreateHeaderModule");
		}
		#region controller
		protected virtual HeaderModule CreateHeaderModule()
		{
			return new VS2005HeaderModule(this);
		}
		//return own control collection which accepts only TabPages to be added
		protected override System.Windows.Forms.Control.ControlCollection CreateControlsInstance()
		{
			return new ControlCollection(this);
		}
		//make sure the header is correctly docked
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			HeaderModule.SizeChanged();
		}
		//make sure the selected page is correctly docked
		protected override void OnLayout(LayoutEventArgs levent)
		{
			//base.OnLayout(levent);
			if (_selected != null)
				_selected.Bounds = this.TabBounds;
		}
		//post to header
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			HeaderModule.MouseDown(e);
		}
		//post to header
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			HeaderModule.MouseMove(e);
		}
		//post to header
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			HeaderModule.MouseUp(e);
		}
		//post to header
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			Point pt = this.PointToClient(Control.MousePosition);
			HeaderModule.MouseLeave(
				new MouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, 0));
		}
		//post to header
		protected override void OnPaint(PaintEventArgs e)
		{
			if (_coll.Count < 1)
				e.Graphics.Clear(SystemColors.AppWorkspace);
			else
			{
				base.OnPaint(e);
				HeaderModule.Paint(e);
			}
		}
		/// <summary>
		/// occurs, whenever a tabpage is removed
		/// </summary>
		protected void OnAddedPage(TabPage value)
		{
			if (PageAdded != null)
				PageAdded(this, new TabPageEventArgs(value));
		}
		/// <summary>
		/// occurs, whenever a tabpage is added
		/// </summary>
		protected void OnRemovedPage(TabPage value)
		{
			if (PageRemoved != null)
				PageRemoved(this, new TabPageEventArgs(value));
		}
		#endregion
		/// <summary>
		/// requests to close the selected page
		/// </summary>
		public void CloseSelectedPage()
		{
			if (_selected != null)
				_selected.Close();
		}
		#region properties
		/// <summary>
		/// gets if the control is in design mode
		/// </summary>
		[Browsable(false)]
		public bool Designing
		{
			get { return this.DesignMode; }
		}
		/// <summary>
		/// gets the header module
		/// </summary>
		[Description("gets the header module"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public HeaderModule HeaderModule
		{
			get { return _header; }
		}
		/// <summary>
		/// collection of all pages in this tabcontrol
		/// </summary>
		[Description("collection of all pages in this tabcontrol"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TabPageCollection Pages
		{
			get { return this._coll; }
		}
		IList<ITabPage> ITabControl.Pages
		{
			get { return this._coll; }
		}
		/// <summary>
		/// returns the bounds of the tab inside the control
		/// </summary>
		[Description("returns the bounds of the tab inside the control"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rectangle TabBounds
		{
			get
			{
				Rectangle rct = this.ClientRectangle;
				rct.Height -= HeaderModule.HeaderHeight;
				rct.Y += HeaderModule.HeaderHeight;
				return rct;
			}
		}
		/// <summary>
		/// gets or sets the selected page on this tab control. you cannot set any page that isn't part of this tabcontrol
		/// </summary>
		[Description("gets or sets the selected page on this tab control. you cannot set any page that isn't part of this tabcontrol")]
		public TabPage SelectedPage
		{
			get { return this._selected; }
			set
			{
				if (value == null || !this.Controls.Contains(value)) return;
				this.SetSelectedPageInternal(value);
			}
		}
		ITabPage ITabControl.SelectedPage
		{
			get { return this._selected; }
			set
			{
				TabPage page = value as TabPage;
				if (page == null || !this.Controls.Contains(page)) return;
				this.SetSelectedPageInternal(page);
			}
		}
		/// <summary>
		/// sets the selected page. do not use this method
		/// </summary>
		private void SetSelectedPageInternal(TabPage value)
		{
			if (value == this._selected)
				return;
			if (BeforeSelectedPageChanged != null)
				BeforeSelectedPageChanged(this, EventArgs.Empty);
			//store first to prevent stack overflow
			TabPage oldsel = _selected;
			_selected = value;
			//
			if (oldsel != null)
			{
				oldsel.Hide();
				oldsel.OnDeselected();
			}
			if (value != null)
			{
				value.Show();
				value.OnSelected();
			}
			this.HeaderModule.Refresh(HeaderModule.Section.Headers);
			this.HeaderModule.Reload(HeaderModule.Section.CloseButton |
				HeaderModule.Section.Browser, null);
			this.RaiseSelectedPageChanged();
		}
		#endregion
		#region events
		private void RaiseSelectedPageChanged()
		{
			if (SelectedPageChanged != null)
				SelectedPageChanged(this, new TabPageEventArgs(this._selected));
		}
		void Page_Closing(object sender, CancelEventArgs e)
		{
			TabPage page = sender as TabPage;
			if (PageClosing != null && page != null)
			{
				PageCloseEventArgs evt = new PageCloseEventArgs(page, e.Cancel);
				PageClosing(this, evt);
				e.Cancel = evt.Cancel;
			}
		}
		[Description("this event is fired whenever the selected page is changed")]
		public event TabPageEventHandler SelectedPageChanged;
		public event EventHandler BeforeSelectedPageChanged;
		[Description("this event is fired when a new page is added, but not those created at design time")]
		public event TabPageEventHandler PageAdded;
		[Description("this event is fired whenever a page is removed")]
		public event TabPageEventHandler PageRemoved;
		[Description("this is fired on a page close request")]
		public event EventHandler<PageCloseEventArgs> PageClosing;
		#endregion
	}
	/// <summary>
	/// interface for using headermodules
	/// </summary>
	public interface ITabControl
	{
		IList<ITabPage> Pages { get; }
		ITabPage SelectedPage { get; set; }
		void CloseSelectedPage();
		void Update();
		void Invalidate(Region rgn, bool updateChildren);
		Graphics CreateGraphics();
		Rectangle ClientRectangle { get; }
		bool Designing { get; }
	}
	/// <summary>
	/// delegate for the pageadded / pageremoved / selectedpagechanged event
	/// </summary>
	public delegate void TabPageEventHandler(TabControl sender, TabPageEventArgs e);
	public class TabPageEventArgs : EventArgs
	{
		#region variables
		private TabPage _page;
		#endregion
		public TabPageEventArgs(TabPage page)
		{
			_page = page;
		}
		public TabPage Page
		{
			get { return _page; }
		}
	}
	/// <summary>
	/// used for the pageclosing event
	/// </summary>
	public class PageCloseEventArgs : TabPageEventArgs
	{
		private bool _cancel;
		public PageCloseEventArgs(TabPage page, bool cancel)
			: base(page)
		{
			_cancel = cancel;
		}
		public bool Cancel
		{
			get { return _cancel; }
			set { _cancel = value; }
		}
	}
}

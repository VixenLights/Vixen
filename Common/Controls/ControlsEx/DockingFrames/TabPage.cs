using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ControlsEx.DockingFrames
{
	/// <summary>
	/// TabPage is a Page used for XPControl
	/// </summary>
	[ToolboxItem(false),
	Designer(typeof(TabPageDesigner)),
	DefaultProperty("Caption")]
	public class TabPage:UserControl,ITabPage
	{
		#region variables
		private TabControl _owner;
		private string _caption;
		private bool _canclose = true;
		#endregion
		#region ctor
		public TabPage(string caption)
		{
			this._caption=caption;
		}
		public TabPage():this("TabPage")
		{
		}
		#endregion
		#region controller
		//make sure that the page is correctly docked
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if (this._owner!=null)
			{
				Rectangle bounds=_owner.TabBounds;
				x=bounds.X; y=bounds.Y;
				width=bounds.Width;
				height=bounds.Height;
			}
			base.SetBoundsCore (x, y, width, height, specified);
		}
		//make sure only the selected page is online
		protected override void SetVisibleCore(bool value)
		{
			if (this._owner != null && //make sure correct visibility
				value != (_owner.SelectedPage == this))
					_owner.SelectedPage = value ? this : null;
			else
				base.SetVisibleCore(value);
		}
		//select this tabpage
		protected override void Select(bool directed, bool forward)
		{
			if (_owner != null)
				_owner.SelectedPage = this;
			else
				base.Select(directed, forward);
		}
		#endregion
		/// <summary>
		/// closes the current page, if closing is supported
		/// </summary>
		public void Close()
		{
			if (_owner == null || !_canclose)
				return;
			CancelEventArgs e = new CancelEventArgs(false);
			OnClosing(e);
			//owner may have changed in handler
			if (!e.Cancel && _owner!=null)
			{
				_owner.Pages.Remove(this);
				//very important! otherwise there
				//will be a invalidoperationexception
				//when finalize is called
				Dispose();
			}
		}
		#region interrupts
		/// <summary>
		/// occurs, when the tabpage is removed from a tabcontrol,
		/// which can be done via the close button, or the remove
		/// methods of the owning tabcontrol.
		/// at this moment, the Owner property is still set.
		/// this method is called AFTER the OnPageRemoved method in TabControl.
		/// to interrupt this message, override it.
		/// </summary>
		protected internal virtual void OnRemoved()
		{
			return;
		}
		/// <summary>
		/// occurs, when the tabpage is added to a tabcontrol.
		/// at this moment, the Owner property is already set.
		/// this method is called AFTER the OnPageAdded method in TabControl.
		/// to interrupt this message, override it.
		/// </summary>
		protected internal virtual void OnAdded()
		{
			return;
		}
		/// <summary>
		/// occurs, when the page is selected
		/// </summary>
		protected internal virtual void OnSelected()
		{
			return;
		}
		/// <summary>
		/// occurs, when the page is deselected
		/// </summary>
		protected internal virtual void OnDeselected()
		{
			return;
		}
		/// <summary>
		/// called after the parent pageclosing event
		/// </summary>
		protected internal virtual void OnClosing(CancelEventArgs e)
		{
			if (Closing != null)
				Closing(this, e);
		}
		#endregion
		#region properties
		#region browsable
		/// <summary>
		/// gets or sets the owner. do not use this method
		/// </summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal TabControl Owner
		{
			get{return this._owner;}
			set{this._owner=value;}
		}
		/// <summary>
		/// gets or sets the caption which is displayed in the header and the menuitem
		/// </summary>
		[Description("gets or sets the caption which is displayed in the header and the menuitem"),
		DefaultValue("TabPage"),
		Localizable(true)]
		public string Caption
		{
			get{return _caption;}
			set
			{
				if (value==_caption) return;
				this._caption=value;
				if (_owner!=null)
				{
					_owner.HeaderModule.Reload(
						HeaderModule.Section.Browser |
						HeaderModule.Section.Headers, this);
					_owner.HeaderModule.Refresh(
						HeaderModule.Section.Browser |
						HeaderModule.Section.Headers);
				}
			}
		}
		/// <summary>
		/// gets or sets if the tabpage can be removed via the close button
		/// </summary>
		[Description("gets or sets if the tabpage can be removed via the close button"),
		DefaultValue(true)]
		public bool CanClose
		{
			get{return _canclose;}
			set
			{
				_canclose=value;
				if (_owner!=null)
					_owner.HeaderModule.Reload(
						HeaderModule.Section.CloseButton,this);
			}
		}
		#endregion
		#region designerserilization overrides
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point Location
		{
			get{return base.Location;}
			set{base.Location=value;}
		}
		[Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DockStyle Dock
		{
			get{return base.Dock;}
			set{base.Dock=value;}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Visible
		{
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		public new Size Size
		{
			get{
				if(_owner!=null)
					return _owner.TabBounds.Size;
				return base.Size;}
			set{base.Size=value;}
		}
		#endregion
		#endregion
		[Description("this is fired on a page close request")]
		public event EventHandler<CancelEventArgs> Closing;
	}
	/// <summary>
	/// generic interface for headermodule
	/// </summary>
	public interface ITabPage
	{
		String Caption { get; }
		bool CanClose { get; }
		void Close();
	}
}

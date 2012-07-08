using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ControlsEx.DockingFrames
{
	/// <summary>
	/// header module is responsible for drawing
	/// and coordinating tab buttons
	/// </summary>
	public abstract class HeaderModule : IDisposable
	{
		/// <summary>
		/// specifies flags of different header
		/// sections for refreshing
		/// </summary>
		public enum Section
		{
			/// <summary>
			/// the tab headers
			/// </summary>
			Headers = 1,
			/// <summary>
			/// the controls used to browse,
			/// can be a spin or context menu button
			/// </summary>
			Browser = 2,
			/// <summary>
			/// close button for selected tab
			/// </summary>
			CloseButton = 4,
			/// <summary>
			/// whole header
			/// </summary>
			All = 7
		}
		#region variables
		private ITabControl _owner;
		private int _maxheaderwidth = 200;
		#endregion
		/// <summary>
		/// ctor
		/// </summary>
		public HeaderModule(ITabControl owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			_owner = owner;
		}
		public virtual void Dispose() { }
		#region public members
		/// <summary>
		/// paint header section
		/// </summary>
		public void Paint(PaintEventArgs e)
		{
			if (e == null || e.Graphics == null) return;
			OnPaint(e);
		}
		/// <summary>
		/// makes the owner refresh the areas
		/// needed to repaint the given sections
		/// </summary>
		public void Refresh(Section section)
		{
			OnRefresh(section);
		}
		/// <summary>
		/// measures the header sections
		/// </summary>
		/// <param name="start">the tabpage where to start. set NULL to remeasure all</param>
		public void Reload(Section section, ITabPage start)
		{
			if (!this.Owner.Pages.Contains(start))
				start = null;
			OnReload(section, start);
		}
		public void MouseDown(MouseEventArgs e)
		{
			OnMouseDown(e);
		}
		public void MouseMove(MouseEventArgs e)
		{
			OnMouseMove(e);
		}
		public void MouseUp(MouseEventArgs e)
		{
			OnMouseUp(e);
		}
		public void MouseLeave(MouseEventArgs e)
		{
			OnMouseLeave(e);
		}
		public void SizeChanged()
		{
			OnSizeChanged();
		}
		#endregion
		#region virtual
		protected virtual void OnPaint(PaintEventArgs e) { }
		protected virtual void OnRefresh(Section section) { }
		protected virtual void OnReload(Section section, ITabPage start) { }
		protected virtual void OnMouseDown(MouseEventArgs e) { }
		protected virtual void OnMouseMove(MouseEventArgs e) { }
		protected virtual void OnMouseUp(MouseEventArgs e) { }
		protected virtual void OnMouseLeave(MouseEventArgs e) { }
		protected virtual void OnSizeChanged() { }
		public abstract int HeaderHeight { get; }
		#endregion
		#region properties
		/// <summary>
		/// gets the tabcontrol this header is
		/// associated to
		/// </summary>
		protected ITabControl Owner
		{
			get { return _owner; }
		}
		/// <summary>
		/// gets or sets the maximal width a tab header can be
		/// </summary>
		[DefaultValue(200),
		Description("gets or sets the maximal width a tab header can be")]
		public int MaxHeaderWidth
		{
			get { return _maxheaderwidth; }
			set
			{
				value = Math.Max(24, Math.Min(1000, value));
				if (value == _maxheaderwidth) return;
				_maxheaderwidth = value;
				if (this.Owner.Pages.Count < 1) return;
				Reload(Section.Headers, null);
				Refresh(Section.Headers);
			}
		}
		#endregion
	}
	/// <summary>
	/// draws headers in style of vs2005
	/// </summary>
	public class VS2005HeaderModule : HeaderModule
	{
		#region variables
		private ArrayList _paths = new ArrayList();
		private ContextMenu _mnu;
		private Font _fontinactive, _fontactive;
		//butons
		private Rectangle _tabsbtn = new Rectangle(0, 3, 15, 14),
			_closebtn = new Rectangle(17, 3, 15, 14);
		private ButtonState _tabsstate = ButtonState.Inactive,
			_closestate = ButtonState.Inactive;
		#endregion
		public VS2005HeaderModule(ITabControl owner)
			: base(owner)
		{
			_mnu = new ContextMenu();
			_fontinactive = new Font("Microsoft Sans Serif", 8.25f);
			_fontactive = new Font("Microsoft Sans Serif", 9.25f, FontStyle.Bold);
			//
		}
		public override void Dispose()
		{
			_fontactive.Dispose();
			_fontinactive.Dispose();
			_mnu.Dispose();
		}

		#region overrides
		protected override void OnSizeChanged()
		{
			_closebtn.X = Owner.ClientRectangle.Width -
				_closebtn.Width;
			_tabsbtn.X = _closebtn.X - 1 - _tabsbtn.Width;

			Refresh(Section.All);
		}
		#region paint
		protected override void OnPaint(PaintEventArgs e)
		{
			//bottom line
			using (Pen pn = new Pen(Color.FromArgb(157, 157, 161)))
			{
				e.Graphics.DrawLine(pn, 1, 19, this.Owner.ClientRectangle.Width - 2, 19);
			}
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			//clip test
			PaintHeaders(e);
			PaintButtons(e);
		}
		private void PaintHeaders(PaintEventArgs e)
		{
			if (_paths.Count < 1 || _paths.Count != this.Owner.Pages.Count)
				return;
			GraphicsPath path; Rectangle bounds;
			//draw pages end-to-start
			for (int i = this.Owner.Pages.Count - 1; i > -1; i--)
			{
				#region unselected pages
				ITabPage page = this.Owner.Pages[i];
				path = (GraphicsPath)_paths[i];
				bounds = Rectangle.Round(path.GetBounds());
				if (page == this.Owner.SelectedPage ||
					path.GetBounds().Right > _tabsbtn.X)
					continue;
				using (LinearGradientBrush brs = new LinearGradientBrush(
						  new Point(0, 3), new Point(0, 19),
						  Color.White, Color.FromArgb(236, 236, 239)))
				{
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.FillPath(brs, path);//fill tab

					using (Pen pn = new Pen(Color.FromArgb(157, 157, 161)))
					{
						e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
						e.Graphics.DrawPath(pn, path);//draw tab outline

						e.Graphics.DrawCurve(Pens.White,//draw tab front
							new Point[]{new Point(bounds.X+2,18),
										   new Point(bounds.X+13,7),
										   new Point(bounds.X+20,4)}, 0.7f);

						pn.Color = Color.FromArgb(233, 233, 237);
						e.Graphics.DrawLines(pn, new Point[]{//draw tab end
																	  new Point(bounds.Right-2,4),
																	  new Point(bounds.Right-1,5),
																	  new Point(bounds.Right-1,18)});
					}
				}
				if (page.Caption != null)
				{
					using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap))
					{
						fmt.LineAlignment = fmt.Alignment =
							StringAlignment.Center;
						fmt.Trimming = StringTrimming.EllipsisCharacter;
						e.Graphics.DrawString(page.Caption, _fontinactive, Brushes.Black,
							new RectangleF(bounds.X + 13, bounds.Y + 2, bounds.Width - 14, bounds.Height - 2), fmt);
					}
				}
				#endregion
			}
			//draw selected page
			if (this.Owner.SelectedPage != null)
			{
				#region selected page
				path = (GraphicsPath)_paths[
					this.Owner.Pages.IndexOf(this.Owner.SelectedPage)];
				bounds = Rectangle.Round(path.GetBounds());
				if (bounds.Right > _tabsbtn.X) return;
				//draw page
				using (Pen pn = new Pen(Color.FromArgb(157, 157, 161)))
				{
					e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
					e.Graphics.FillPath(Brushes.White, path);//fill tab

					e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
					e.Graphics.DrawPath(pn, path);

					e.Graphics.DrawLine(Pens.White,
						bounds.X + 1, bounds.Bottom, bounds.Right - 1, bounds.Bottom);//draw tab bottom line
				}
				if (this.Owner.SelectedPage.Caption != null)
				{
					using (StringFormat fmt = new StringFormat(StringFormatFlags.NoWrap))
					{
						fmt.LineAlignment = fmt.Alignment =
							StringAlignment.Center;
						fmt.Trimming = StringTrimming.EllipsisCharacter;
						e.Graphics.DrawString(this.Owner.SelectedPage.Caption,
							_fontactive, Brushes.Black, new RectangleF(
							bounds.X + 13, bounds.Y + 2, bounds.Width - 14, bounds.Height - 2), fmt);
					}
				}
				#endregion
			}
		}
		private void PaintButtons(PaintEventArgs e)
		{
			Pen pn;
			#region tabs buttton
			DrawButtonBg(e.Graphics, _tabsbtn, _tabsstate);
			if (_paths.Count < 1 || ((GraphicsPath)_paths[//shows all tabs?
				_paths.Count - 1]).GetBounds().Right < _tabsbtn.X)
			{
				e.Graphics.FillPolygon(_tabsstate == ButtonState.Inactive ?
					Brushes.DimGray : Brushes.Black,
					new Point[]{new Point(_tabsbtn.X+3,_tabsbtn.Y+5),
								   new Point(_tabsbtn.X+11,_tabsbtn.Y+5),
								   new Point(_tabsbtn.X+7,_tabsbtn.Y+9)});
			}
			else
			{
				pn = _tabsstate == ButtonState.Inactive ?
					Pens.DimGray : Pens.Black;
				e.Graphics.DrawRectangle(pn, _tabsbtn.X + 3, _tabsbtn.Y + 2, _tabsbtn.Width - 7, _tabsbtn.Height - 5);
				e.Graphics.DrawLine(pn, _tabsbtn.X + 3, _tabsbtn.Bottom - 4, _tabsbtn.Right - 4, _tabsbtn.Bottom - 4);
			}
			#endregion
			#region close button
			DrawButtonBg(e.Graphics, _closebtn, _closestate);
			pn = _closestate == ButtonState.Inactive ?
				Pens.DimGray : Pens.Black;
			e.Graphics.DrawLine(pn, _closebtn.X + 3, _closebtn.Y + 4, _closebtn.X + 9, _closebtn.Y + 10);
			e.Graphics.DrawLine(pn, _closebtn.X + 4, _closebtn.Y + 4, _closebtn.X + 10, _closebtn.Y + 10);
			e.Graphics.DrawLine(pn, _closebtn.X + 9, _closebtn.Y + 4, _closebtn.X + 3, _closebtn.Y + 10);
			e.Graphics.DrawLine(pn, _closebtn.X + 10, _closebtn.Y + 4, _closebtn.X + 4, _closebtn.Y + 10);
			#endregion
		}
		private void DrawButtonBg(Graphics gr, Rectangle rct, ButtonState state)
		{
			if (state != ButtonState.Checked &&
				state != ButtonState.Pushed) return;
			using (SolidBrush brs = new SolidBrush(Color.White))
			{
				if (state == ButtonState.Checked)
					brs.Color = Color.FromArgb(100, SystemColors.Highlight);
				else
					brs.Color = Color.FromArgb(160, SystemColors.Highlight);
				gr.FillRectangle(brs,
					rct.X, rct.Y, rct.Width - 1, rct.Height - 1);
				gr.DrawRectangle(SystemPens.Highlight,
					rct.X, rct.Y, rct.Width - 1, rct.Height - 1);
			}
		}
		#endregion
		protected override void OnReload(HeaderModule.Section section, ITabPage start)
		{
			if ((section & HeaderModule.Section.Headers) != 0)
			{
				#region remeasure headers
				bool remeasure = start == null;
				//manage paths array
				while (_paths.Count > this.Owner.Pages.Count)
				{
					GraphicsPath path = (GraphicsPath)_paths[0];
					_paths.RemoveAt(0);
					path.Dispose();
				}
				while (_paths.Count < this.Owner.Pages.Count)
				{
					_paths.Add(new GraphicsPath());
				}
				//remeasure tabs
				int x = 1, width;
				using (Graphics gr = this.Owner.CreateGraphics())
				{
					for (int i = 0; i < this.Owner.Pages.Count; i++)
					{
						ITabPage page = this.Owner.Pages[i];
						GraphicsPath path = (GraphicsPath)_paths[i];
						if (start == page) remeasure = true;
						if (!remeasure)
						{
							x = (int)(path.GetBounds().Right) - 10;
							continue;
						}
						#region measure header
						if (page.Caption == null || page.Caption == "")
							width = 24;
						else
							width = Math.Min(this.MaxHeaderWidth,
								24 + (int)gr.MeasureString(page.Caption, _fontactive).Width);
						path.Reset();
						path.AddCurve(//create tab shape
							new Point[]{new Point(x,19),
										   new Point(x+12,7),
										   new Point(x+19,3)}, 0.7f);
						path.AddLine(x + 19, 3, x + width - 2, 3);
						path.AddLine(x + width, 5, x + width, 19);
						#endregion
						x += width - 10;
					}
				}
				#endregion
			}
			if ((section & HeaderModule.Section.Browser) != 0)
			{
				#region tabs button
				//manage contextmenu
				while (_mnu.MenuItems.Count > this.Owner.Pages.Count)
				{
					MenuItem item = _mnu.MenuItems[0];
					_mnu.MenuItems.RemoveAt(0);
					item.Dispose();
				}
				while (_mnu.MenuItems.Count < this.Owner.Pages.Count)
				{
					_mnu.MenuItems.Add(new MenuItem("item",
						new EventHandler(MenuItem_Click)));
				}
				//reset defaultitems
				foreach (MenuItem item in _mnu.MenuItems)
					item.DefaultItem = false;
				//update captions
				for (int i = 0; i < this.Owner.Pages.Count; i++)
				{
					ITabPage page = this.Owner.Pages[i];
					_mnu.MenuItems[i].Text = page.Caption;
					if (page == this.Owner.SelectedPage)
						_mnu.MenuItems[i].DefaultItem = true;
				}
				_tabsstate = _mnu.MenuItems.Count > 0 ?
					ButtonState.Normal : ButtonState.Inactive;
				#endregion
			}
			if ((section & HeaderModule.Section.CloseButton) != 0)
			{
				#region close button
				ButtonState state = (
					this.Owner.SelectedPage != null &&
					this.Owner.SelectedPage.CanClose) ?
					ButtonState.Normal :
					ButtonState.Inactive;
				if (_closestate != state)
				{
					_closestate = state;
					Refresh(Section.CloseButton);
				}
				#endregion
			}
		}
		protected override void OnRefresh(HeaderModule.Section section)
		{
			using (Region rgn = new Region())
			{
				if ((section & HeaderModule.Section.Headers) != 0)
					rgn.Union(new Rectangle(0, 0, _tabsbtn.X, HeaderHeight));
				if ((section & HeaderModule.Section.Browser) != 0)
					rgn.Union(_tabsbtn);
				if ((section & HeaderModule.Section.CloseButton) != 0)
					rgn.Union(_closebtn);
				//update control
				this.Owner.Invalidate(rgn, false);
				this.Owner.Update();
			}
		}
		#region buttons
		private bool SetTabsButton(ButtonState state)
		{
			if (_tabsstate == ButtonState.Inactive ||
				_tabsstate == state) return false;
			_tabsstate = state;
			Refresh(Section.Browser);
			return true;
		}
		private bool SetCloseButton(ButtonState state)
		{
			if (_closestate == ButtonState.Inactive ||
				_closestate == state) return false;
			_closestate = state;
			Refresh(Section.CloseButton);
			return true;
		}
		private void HoverButtons(int x, int y)
		{
			if (_tabsbtn.Contains(x, y))
				SetTabsButton(ButtonState.Checked);
			else
				SetTabsButton(ButtonState.Normal);
			if (_closebtn.Contains(x, y))
				SetCloseButton(ButtonState.Checked);
			else
				SetCloseButton(ButtonState.Normal);
		}
		#endregion
		//set new selected page
		private void MenuItem_Click(object sender, EventArgs e)
		{
			int i = ((MenuItem)sender).Index;
			if (i < this.Owner.Pages.Count)
				this.Owner.SelectedPage =
					this.Owner.Pages[i];
		}
		#region mouse
		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (_tabsbtn.Contains(e.X, e.Y) &&
				SetTabsButton(ButtonState.Pushed))//show pages menu
			{
				Control owner = this.Owner as Control;
				if (owner != null && _mnu.MenuItems.Count < 1) return;
				_mnu.Show(owner, new Point(_tabsbtn.X, _tabsbtn.Bottom + 1));
			}
			else if (_closebtn.Contains(e.X, e.Y) &&
				SetCloseButton(ButtonState.Pushed))//close (remove) current page
			{
				if (!this.Owner.Designing)
					this.Owner.CloseSelectedPage();
			}
			else
			{
				if (this.Owner.SelectedPage != null && //hit selected page
					((GraphicsPath)_paths[this.Owner.Pages.IndexOf(
					this.Owner.SelectedPage)]).IsVisible(e.X, e.Y))
					return;
				for (int i = 0; i < this.Owner.Pages.Count; i++)
				{
					GraphicsPath path = (GraphicsPath)_paths[i];
					ITabPage page = this.Owner.Pages[i];
					if (page == this.Owner.SelectedPage ||
						path.GetBounds().Right > _tabsbtn.X) continue;
					if (path.IsVisible(e.X, e.Y))
					{
						this.Owner.SelectedPage = page;
						return;
					}
				}
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e.Button != MouseButtons.None) return;
			HoverButtons(e.X, e.Y);
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			HoverButtons(e.X, e.Y);
		}
		protected override void OnMouseLeave(MouseEventArgs e)
		{
			HoverButtons(e.X, e.Y);
		}
		#endregion
		public override int HeaderHeight
		{
			get
			{
				return 20;
			}
		}

		#endregion
	}
}

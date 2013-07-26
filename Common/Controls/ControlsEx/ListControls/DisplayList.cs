using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Common.Controls.ControlsEx.ListControls
{
	/// <summary>
	/// abstract base class for all displaylists
	/// </summary>
	[Designer(typeof (DisplayListDesigner)), //prevent drawing the grid on the control
	 DefaultEvent("SelectionChanged")]
	public abstract class DisplayList : BorderedScrollableControl
	{
		#region types

		/// <summary>
		/// collection for containing displayitems
		/// </summary>
		public class DisplayItemCollection : CollectionBase<DisplayList, DisplayItem>
		{
			public DisplayItemCollection(DisplayList owner) :
				base(owner)
			{
			}

			#region public members

			/// <summary>
			/// adds the specified array of elements to the collection
			/// </summary>
			public void AddRange(DisplayItem[] elems)
			{
				if (elems == null || elems.Length == 0) return;
				foreach (DisplayItem elem in elems) {
					OnValidate(elem);
					InnerList.Add(elem);
					//
					elem.Refresh += new EventHandler(OnElementRefresh);
				}
				Owner.Reload();
			}

			#endregion

			#region overrides

			//refresh area
			private void OnElementRefresh(object sender, EventArgs e)
			{
				int i = IndexOf(sender as DisplayItem);
				if (i != -1) {
					Owner.Invalidate(i);
					Owner.Update();
				}
			}

			//validate items
			protected override void OnValidate(DisplayItem value)
			{
				if (value == null)
					throw new ArgumentException("value");
			}

			//insert / add
			protected override void OnInsert(int index, DisplayItem value)
			{
				if (index <= Owner._selection)
					Owner.RaiseBeforeSelectionChanged();
			}

			protected override void OnInsertComplete(int index, DisplayItem value)
			{
				value.Refresh += new EventHandler(OnElementRefresh);
				//selection
				if (index <= Owner._selection) {
					Owner._selection =
						Math.Min(this.Count - 1, Owner._selection + 1);
					Owner.RaiseSelectionChanged();
				}
				Owner.Reload();
			}

			//remove
			protected override void OnRemove(int index, DisplayItem value)
			{
				if (index <= Owner._selection)
					Owner.RaiseBeforeSelectionChanged();
			}

			protected override void OnRemoveComplete(int index, DisplayItem value)
			{
				value.Refresh -= new EventHandler(OnElementRefresh);
				if (index <= Owner._selection) {
					Owner._selection = Math.Min(this.Count - 1,
					                            Math.Max(0, Owner._selection - 1));
					Owner.RaiseSelectionChanged();
				}
				Owner.Reload();
			}

			//set
			protected override void OnSet(int index, DisplayItem oldValue, DisplayItem newValue)
			{
				if (index == Owner._selection)
					Owner.RaiseBeforeSelectionChanged();
			}

			protected override void OnSetComplete(int index, DisplayItem oldValue, DisplayItem newValue)
			{
				oldValue.Refresh -= new EventHandler(OnElementRefresh);
				newValue.Refresh += new EventHandler(OnElementRefresh);
				//
				if (index == Owner._selection)
					Owner.RaiseSelectionChanged();
				Owner.Invalidate(index);
				Owner.Update();
			}

			//clear
			protected override void OnClear()
			{
				if (Owner._selection != -1)
					Owner.RaiseBeforeSelectionChanged();
				foreach (DisplayItem elem in this.InnerList)
					elem.Refresh -= new EventHandler(OnElementRefresh);
			}

			protected override void OnClearComplete()
			{
				if (Owner._selection != -1) {
					Owner._selection = -1;
					Owner.RaiseSelectionChanged();
				}
				Owner.Reload();
			}

			#endregion
		}

		/// <summary>
		/// new control collection which rejects all controls
		/// </summary>
		public new class ControlCollection : Control.ControlCollection
		{
			public ControlCollection(DisplayList owner) : base(owner)
			{
			}

			public override void Add(Control value)
			{
				throw new NotSupportedException("no controls can be added");
			}

			public override void AddRange(Control[] controls)
			{
				throw new NotSupportedException("no controls can be added");
			}
		}

		#endregion

		#region variables

		private DisplayItemCollection _coll;
		//rendering config
		private System.Drawing.ContentAlignment _textalign =
			System.Drawing.ContentAlignment.TopCenter;

		private HorizontalAlignment _imagealign =
			HorizontalAlignment.Center;

		private ToolStripItemDisplayStyle _displayStyle =
			ToolStripItemDisplayStyle.ImageAndText;

		private bool _textvertical = false,
		             _includeindex = false,
		             _autoselect = true;

		//temp variables
		private DisplayDrags _drags = DisplayDrags.ElementCopy;
		private Rectangle _dragrct;

		private int _hover = -1,
		            _selection = -1,
		            _suspend = 0;

		#endregion

		public DisplayList()
		{
			this._coll = CreateItemCollection();
			if (_coll == null)
				throw new ArgumentException("collection not initialized");
			this.SetStyle(
				ControlStyles.DoubleBuffer |
				ControlStyles.UserPaint |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.ResizeRedraw, true);
			this.BackColor = SystemColors.Window;
		}

		#region layout

		/// <summary>
		/// override this member and adjust the scrollbars
		/// according to the content
		/// </summary>
		protected abstract Size GetTotalSize(Size clientsize, int count);

		/// <summary>
		/// override this member and return an index of the
		/// element at the specified position. collection
		/// boundaries don't have to be checked
		/// </summary>
		protected abstract int GetIndexAt(int x, int y);

		/// <summary>
		/// override this member and return the bounds
		/// of an element at the specified index.
		/// collection boundaries don't have to be checked
		/// </summary>
		protected abstract Rectangle GetBoundsAt(int index);

		/// <summary>
		/// gets the interval of elements that
		/// are in the given clip rectangle
		/// </summary>
		protected abstract void GetDrawingInterval(
			Rectangle clip, out int start, out int stop);

		#endregion

		//override this to intercept item clicking
		protected virtual void OnItemClicked(int index)
		{
			if (ItemClicked != null)
				ItemClicked(this, new ItemEventArgs(index,
				                                    index == -1 ? null : Items[index]));
		}

		//override this to intercept hovering
		protected virtual void OnHoverChanged(int index)
		{
		}

		#region rendering

		/// <summary>
		/// renders the background
		/// </summary>
		protected virtual void DrawSelectionFrame(PaintEventArgs e, Rectangle rct, ButtonState state)
		{
			VisualStyleElement elem = VisualStyleElement.CreateElement(
				"Listview", 6, state == ButtonState.Pushed ? 2 : 12);
			if (VisualStyleRenderer.IsSupported &&
			    VisualStyleRenderer.IsElementDefined(elem)) {
				VisualStyleRenderer rend = new VisualStyleRenderer(elem);
				rend.DrawBackground(e.Graphics, rct);
			}
			else {
				using (SolidBrush _brs = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
					e.Graphics.FillRectangle(_brs, rct);
				e.Graphics.DrawRectangle(SystemPens.Highlight,
				                         rct.X, rct.Y, rct.Width - 1, rct.Height - 1);
			}
		}

		/// <summary>
		/// create stringformat for element
		/// </summary>
		private StringFormat CreateStringFormat()
		{
			StringFormat _fmt = new StringFormat(StringFormatFlags.NoWrap);
			_fmt.Trimming = StringTrimming.EllipsisCharacter;
			if (_textvertical)
				_fmt.FormatFlags |=
					StringFormatFlags.DirectionVertical;
			//
			StringAlignment alignment, linealignment;
			GraphicsEx.GetStringAlignmentFromAlignment(_textalign,
			                                           out alignment, out linealignment);
			_fmt.Alignment = alignment;
			_fmt.LineAlignment = linealignment;
			return _fmt;
		}

		/// <summary>
		/// gets the area remaining for an image in an element,
		/// after text is drawn
		/// </summary>
		private Rectangle GetRemainingArea(Rectangle bounds, StringFormat fmt)
		{
			//text is center-bottom
			bounds.Inflate(-2, -2);
			if (fmt == null)
				return bounds;

			if ((fmt.FormatFlags & StringFormatFlags.DirectionVertical) != 0)
				switch (fmt.LineAlignment) {
					case StringAlignment.Near:
						bounds.X += base.Font.Height + 2;
						bounds.Width -= base.Font.Height + 2;
						break;
					case StringAlignment.Far:
						bounds.Width -= base.Font.Height + 2;
						break;
				}
			else
				switch (fmt.LineAlignment) {
					case StringAlignment.Near:
						bounds.Y += base.Font.Height + 2;
						bounds.Height -= base.Font.Height + 2;
						break;
					case StringAlignment.Far:
						bounds.Height -= base.Font.Height + 2;
						break;
				}
			return bounds;
		}

		/// <summary>
		/// override this member to implement own drawing of the
		/// text
		/// </summary>
		private void DrawElementText(Graphics gr, Rectangle bounds, StringFormat fmt, int index)
		{
			bounds.Inflate(-2, -2);
			string text = Items[index].Text;
			if (text == null)
				text = string.Empty;
			if (_includeindex)
				gr.DrawString((index + 1).ToString() + ">> " + text,
				              base.Font, Brushes.Black, bounds, fmt);
			else
				gr.DrawString(text, base.Font, Brushes.Black, bounds, fmt);
		}

		/// <summary>
		/// calculates the size an element should have
		/// to fit the specified bounds.
		/// aspect rate is kept.
		/// returned bounds are centered.
		/// </summary>
		private static Rectangle FitSizeZoom(Rectangle bounds, Size size, HorizontalAlignment align)
		{
			if (bounds.Width < 1 || bounds.Height < 1) return Rectangle.Empty;
			//calculate the minimum zoom
			int small = Math.Max(1000, Math.Max(
				(1000*size.Width)/bounds.Width,
				(1000*size.Height)/bounds.Height));
			//calculate the new size
			Rectangle ret = new Rectangle(0, 0,
			                              Math.Max(1, (1000*size.Width)/small),
			                              Math.Max(1, (1000*size.Height)/small));
			//center the rectangle
			switch (align) {
				case HorizontalAlignment.Left:
					ret.X = bounds.X;
					break;
				case HorizontalAlignment.Right:
					ret.X = bounds.Right - ret.Width;
					break;
				default:
					ret.X = bounds.X + (bounds.Width - ret.Width)/2;
					break;
			}
			ret.Y = bounds.Y + (bounds.Height - ret.Height)/2;
			return ret;
		}

		#endregion

		#region controller

		protected virtual DisplayItemCollection CreateItemCollection()
		{
			return new DisplayItemCollection(this);
		}

		protected override System.Windows.Forms.Control.ControlCollection CreateControlsInstance()
		{
			//new control collection which rejects all controls
			return new ControlCollection(this);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if (Items.Count < 1) return;
			e.Graphics.TranslateTransform(this.AutoScrollPosition.X,
			                              this.AutoScrollPosition.Y);
			e.Graphics.RenderingOrigin = this.AutoScrollPosition;
			//compute start and stop positions according to clipping
			//area, to prevent unnecessary drawing
			int start, stop;
			Rectangle clip = e.ClipRectangle;
			clip.Offset(-AutoScrollPosition.X, -AutoScrollPosition.Y);
			GetDrawingInterval(clip, out start, out stop);
			//
			start = Math.Max(0, start);
			stop = Math.Min(Items.Count - 1, stop);
			//loop through every item and draw it
			for (int i = start; i <= stop; i++) {
				Rectangle rct = GetBoundsAt(i);
				if (!clip.IntersectsWith(rct))
					continue;
				//if selected, draw frame
				if (i == _selection || i == _hover)
					DrawSelectionFrame(e, rct,
					                   i == _selection
					                   	? ButtonState.Checked
					                   	: ButtonState.Pushed);
				//draw item text
				if (TextVisible) {
					using (StringFormat fmt = CreateStringFormat()) {
						this.DrawElementText(e.Graphics, rct, fmt, i);
						rct = GetRemainingArea(rct, fmt);
					}
				}
				//draw item data
				if ((_displayStyle & ToolStripItemDisplayStyle.Image) != 0) {
					GraphicsState state = e.Graphics.Save();
					Items[i].Draw(e.Graphics,
					              FitSizeZoom(rct, Items[i].Size, _imagealign));
					e.Graphics.Restore(state);
				}
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button != MouseButtons.Left)
				return;
			//set up drag start position
			this._dragrct = new Rectangle(new Point(e.X, e.Y), SystemInformation.DragSize);
			this._dragrct.Offset(-this._dragrct.Width/2, -this._dragrct.Height/2);
			//set selected index, collection
			//boundaries are checked in the setting function
			int index = GetIndexAtPosition(e.Location);
			OnItemClicked(index);
			if (_autoselect)
				SelectedIndex = index;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.None) {
				//change hovering of items
				int index = GetIndexAtPosition(e.Location);
				if (index != _hover) {
					Invalidate(_hover);
					_hover = index;
					Invalidate(_hover);
					Update();
					OnHoverChanged(_hover);
				}
			}
			else if (e.Button == MouseButtons.Left &&
			         _selection != -1 &&
			         this._drags != DisplayDrags.NoDrag) {
				if (!_dragrct.Contains(e.X, e.Y)) {
					switch (this._drags) {
						case DisplayDrags.ElementCopy:
							{
								//drag the currently selected item
								this.DoDragDrop(this._coll[_selection], DragDropEffects.Move);
								break;
							}
						case DisplayDrags.ElementMove:
							{
								//drag the currently selected item and remove it from the collection
								if (this.DoDragDrop(this._coll[_selection], DragDropEffects.Move) == DragDropEffects.Move)
									this._coll.RemoveAt(_selection);
								break;
							}
						case DisplayDrags.TagCopy:
							{
								//drag the tag of the currently selected element, if there is one
								if (this._coll[_selection].Tag == null) return;
								this.DoDragDrop(this._coll[_selection].Tag, DragDropEffects.Copy);
								break;
							}
					}
				}
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if (_hover != -1) {
				Invalidate(_hover);
				_hover = -1;
				Update();
				OnHoverChanged(_hover);
			}
		}

		protected override void OnSystemColorsChanged(EventArgs e)
		{
			base.OnSystemColorsChanged(e);
			//make sure, the right selection color is used
			if (this._coll.Count > 0)
				this.Refresh();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			AutoScrollPosition = Point.Empty;
			AutoScrollMinSize = GetTotalSize(
				Size, Items.Count);
		}

		protected void Reload()
		{
			if (_suspend > 0)
				return;
			AutoScrollPosition = Point.Empty;
			this.AutoScrollMinSize = GetTotalSize(
				Size, Items.Count);
			this.Refresh();
		}

		/// <summary>
		/// invalidates the area specified by the index
		/// </summary>
		/// <param name="index"></param>
		protected void Invalidate(int index)
		{
			if (index < 0 || _suspend > 0)
				return;
			Rectangle bounds = this.GetBoundsAt(index);
			bounds.Offset(AutoScrollPosition.X,
			              AutoScrollPosition.Y);
			this.Invalidate(bounds);
		}

		/// <summary>
		/// ensures that the given index is visible
		/// </summary>
		public void EnsureVisible(int index)
		{
			index = Math.Min(Items.Count - 1, Math.Max(-1, index));
			if (index == -1)
				return;
			Rectangle rct = GetBoundsAt(index);
			//evaulate clipping border
			Point pt = AutoScrollPosition;
			if ((-pt.X) > rct.X)
				pt.X = -rct.X;
			else if ((ClientSize.Width - pt.X) < rct.Right)
				pt.X = ClientSize.Width - rct.Right;
			if ((-pt.Y) > rct.Y)
				pt.Y = -rct.Y;
			else if ((ClientSize.Height - pt.Y) < rct.Bottom)
				pt.Y = ClientSize.Height - rct.Bottom;
			//workaround, cause autoscrollposition
			//has to be mirrored
			pt.X = -pt.X;
			pt.Y = -pt.Y;
			AutoScrollPosition = pt;
		}

		/// <summary>
		/// if AutoSelect is set to true, this
		/// gets the closest valid index to the given client position,
		/// otherwise it gets the index the givent client position is above, or -1
		/// </summary>
		public int GetIndexAtPosition(Point client)
		{
			int index = GetIndexAt(client.X - AutoScrollPosition.X,
			                       client.Y - AutoScrollPosition.Y);
			if (_autoselect)
				return Math.Min(Items.Count - 1, Math.Max(0, index));
			else if (index < -1 || index >= _coll.Count)
				return -1;
			return index;
		}

		/// <summary>
		/// suspends updates, for adding a large number of items
		/// </summary>
		public void SuspendUpdates()
		{
			_suspend++;
		}

		/// <summary>
		/// resumes display updates
		/// </summary>
		public void ResumeUpdates()
		{
			if (_suspend > 0)
				_suspend--;
			if (_suspend == 0)
				Reload();
		}

		#endregion

		#region properties

		/// <summary>
		/// gets the collection of display elements
		/// </summary>
		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DisplayItemCollection Items
		{
			get { return this._coll; }
		}

		/// <summary>
		/// gets or sets the selected index
		/// </summary>
		[DefaultValue(-1)]
		public int SelectedIndex
		{
			get { return _selection; }
			set
			{
				value = Math.Min(_coll.Count - 1, Math.Max(-1, value));
				if (value == _selection) return;
				//raise event
				RaiseBeforeSelectionChanged();
				Invalidate(_selection);
				_selection = value;
				Invalidate(_selection);
				Update();
				//raise event
				RaiseSelectionChanged();
				EnsureVisible(_selection);
			}
		}

		/// <summary>
		/// gets or sets the selected item
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DisplayItem SelectedItem
		{
			get
			{
				if (_selection == -1)
					return null;
				return _coll[_selection];
			}
			set { SelectedIndex = _coll.IndexOf(value); }
		}

		/// <summary>
		/// specifies the drag'n'drop behaviour
		/// </summary>
		[Description("specifies the drag'n'drop behaviour"),
		 DefaultValue(typeof (DisplayDrags), "ElementCopy")]
		public DisplayDrags DisplayDrags
		{
			get { return this._drags; }
			set { this._drags = value; }
		}

		/// <summary>
		/// gets or sets the text alignment
		/// </summary>
		[DefaultValue(typeof (System.Drawing.ContentAlignment), "TopCenter")]
		public System.Drawing.ContentAlignment TextAlignment
		{
			get { return _textalign; }
			set
			{
				if (value == _textalign)
					return;
				_textalign = value;
				if (TextVisible)
					this.Refresh();
			}
		}

		[DefaultValue(typeof (HorizontalAlignment), "Center")]
		public HorizontalAlignment ImageAlignment
		{
			get { return _imagealign; }
			set
			{
				if (value == _imagealign)
					return;
				_imagealign = value;
				Refresh();
			}
		}

		/// <summary>
		/// gets or sets if the text is displayed vertically
		/// </summary>
		[DefaultValue(false)]
		public bool TextVertical
		{
			get { return _textvertical; }
			set
			{
				if (value == _textvertical)
					return;
				_textvertical = value;
				if (TextVisible)
					this.Refresh();
			}
		}

		/// <summary>
		/// gets or sets the display style
		/// </summary>
		[DefaultValue(typeof (ToolStripItemDisplayStyle), "ImageAndText")]
		public ToolStripItemDisplayStyle DisplayStyle
		{
			get { return _displayStyle; }
			set
			{
				if (value == _displayStyle)
					return;
				_displayStyle = value;
				Refresh();
			}
		}

		[Browsable(false)]
		public bool TextVisible
		{
			get { return (_displayStyle & ToolStripItemDisplayStyle.Text) != 0; }
		}

		/// <summary>
		/// gets or sets if the index is included in the text
		/// </summary>
		[DefaultValue(false)]
		public bool IncludeIndex
		{
			get { return _includeindex; }
			set
			{
				if (value == _includeindex) return;
				_includeindex = value;
				if ((_displayStyle & ToolStripItemDisplayStyle.Text) != 0)
					this.Refresh();
			}
		}

		/// <summary>
		/// gets or sets if items are selected on click
		/// </summary>
		[DefaultValue(true)]
		public bool AutoSelect
		{
			get { return _autoselect; }
			set { _autoselect = value; }
		}

		[DefaultValue(typeof (Color), "Window")]
		public override Color BackColor
		{
			get { return base.BackColor; }
			set { base.BackColor = value; }
		}

		#region designer overrides

		[DefaultValue(true)]
		public override bool AutoScroll
		{
			get { return base.AutoScroll; }
			set { base.AutoScroll = value; }
		}

		[Browsable(false),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Size AutoScrollMinSize
		{
			get { return base.AutoScrollMinSize; }
			set { base.AutoScrollMinSize = value; }
		}

		#endregion

		#endregion

		#region events

		/// <summary>
		/// raises the selectionchanged event
		/// </summary>
		protected void RaiseSelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		public event EventHandler SelectionChanged;

		/// <summary>
		/// raises the beforeselectionchanged event
		/// </summary>
		protected void RaiseBeforeSelectionChanged()
		{
			if (BeforeSelectionChanged != null)
				BeforeSelectionChanged(this, EventArgs.Empty);
		}

		public event EventHandler BeforeSelectionChanged;
		//
		public event EventHandler<ItemEventArgs> ItemClicked;

		#endregion
	}

	/// <summary>
	/// event args for item click
	/// </summary>
	public class ItemEventArgs : EventArgs
	{
		private int _index;
		private DisplayItem _item;

		public ItemEventArgs(int index, DisplayItem item)
		{
			_index = index;
			_item = item;
		}

		public int Index
		{
			get { return _index; }
		}

		public DisplayItem Item
		{
			get { return _item; }
		}
	}

	/// <summary>
	/// enum of all possible drag'n'drop behaviours
	/// </summary>
	public enum DisplayDrags
	{
		NoDrag = 0,
		TagCopy = 1,
		ElementMove = 2,
		ElementCopy = 3
	}
}
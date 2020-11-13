using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls.Theme;

// Dragging items in a ListView control with visual insertion guides
// http://www.cyotek.com/blog/dragging-items-in-a-listview-control-with-visual-insertion-guides

namespace Common.Controls.DragDropListView
{
	public class DragDropListView : ListView
	{
		#region Constants

		private const int WM_PAINT = 0xF;

		#endregion

		#region Instance Fields

		private bool _allowItemDrag;

		private Color _insertionLineColor;

		#endregion

		#region Public Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="DragDropListView"/> class.
		/// </summary>
		public DragDropListView()
		{
			DoubleBuffered = true;
			InsertionLineColor = ThemeColorTable.ForeColor;
			InsertionIndex = -1;

			DrawItem += List_DrawItem;
			DrawColumnHeader += List_DrawColumnHeader;
			DrawSubItem += List_DrawSubItem;
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the AllowItemDrag property value changes.
		/// </summary>
		[Category("Property Changed")]
		public event EventHandler AllowItemDragChanged;

		/// <summary>
		/// Occurs when the InsertionLineColor property value changes.
		/// </summary>
		[Category("Property Changed")]
		public event EventHandler InsertionLineColorChanged;

		/// <summary>
		/// Occurs when a drag-and-drop operation for an item is completed.
		/// </summary>
		[Category("Drag Drop")]
		public event EventHandler<ListViewItemDragEventArgs> ItemDragDrop;

		/// <summary>
		/// Occurs when a drag-and-drop operation for an item is completed.
		/// </summary>
		[Category("Drag Drop Completed")]
		public event EventHandler<ListViewItemDragEventArgs> ItemDragDropCompleted;

		/// <summary>
		/// Occurs when the user begins dragging an item.
		/// </summary>
		[Category("Drag Drop")]
		public event EventHandler<CancelListViewItemDragEventArgs> ItemDragging;

		#endregion

		#region Overridden Methods

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.DragDrop" /> event.
		/// </summary>
		/// <param name="drgevent">A <see cref="T:System.Windows.Forms.DragEventArgs" /> that contains the event data.</param>
		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			if (IsRowDragInProgress)
			{
				try
				{
					ListViewItem dropItem;

					dropItem = InsertionIndex != -1 ? Items[InsertionIndex] : null;

					if (dropItem != null)
					{
						ListViewItem dragItem;
						int dropIndex;

						dragItem = (ListViewItem) drgevent.Data.GetData(typeof(ListViewItem));
						dropIndex = dropItem.Index;

						if (dragItem.Index < dropIndex)
						{
							dropIndex--;
						}

						if (InsertionMode == InsertionMode.After && dragItem.Index < Items.Count - 1)
						{
							dropIndex++;
						}

						if (dropIndex != dragItem.Index)
						{
							ListViewItemDragEventArgs args;
							Point clientPoint;

							clientPoint = PointToClient(new Point(drgevent.X, drgevent.Y));
							args = new ListViewItemDragEventArgs(dragItem, dropItem, dropIndex, InsertionMode, clientPoint.X, clientPoint.Y);

							OnItemDragDrop(args);

							if (!args.Cancel)
							{
								Items.Remove(dragItem);
								Items.Insert(dropIndex, dragItem);
								SelectedItem = dragItem;
								OnItemDragDropCompleted(args);
							}
						}
					}
				}
				finally
				{
					InsertionIndex = -1;
					IsRowDragInProgress = false;
					Invalidate();
				}
			}

			base.OnDragDrop(drgevent);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.DragLeave" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
		protected override void OnDragLeave(EventArgs e)
		{
			InsertionIndex = -1;
			Invalidate();

			base.OnDragLeave(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.DragOver" /> event.
		/// </summary>
		/// <param name="drgevent">A <see cref="T:System.Windows.Forms.DragEventArgs" /> that contains the event data.</param>
		protected override void OnDragOver(DragEventArgs drgevent)
		{
			if (IsRowDragInProgress)
			{
				int insertionIndex;
				InsertionMode insertionMode;
				ListViewItem dropItem;
				Point clientPoint;

				clientPoint = PointToClient(new Point(drgevent.X, drgevent.Y));
				dropItem = GetItemAt(0,
					Math.Min(clientPoint.Y, Items[Items.Count - 1].GetBounds(ItemBoundsPortion.Entire).Bottom - 1));

				if (dropItem != null)
				{
					Rectangle bounds;

					bounds = dropItem.GetBounds(ItemBoundsPortion.Entire);
					insertionIndex = dropItem.Index;
					insertionMode = clientPoint.Y < bounds.Top + (bounds.Height / 2) ? InsertionMode.Before : InsertionMode.After;

					drgevent.Effect = DragDropEffects.Move;
				}
				else
				{
					insertionIndex = -1;
					insertionMode = InsertionMode;

					drgevent.Effect = DragDropEffects.None;
				}

				if (insertionIndex != InsertionIndex || insertionMode != InsertionMode)
				{
					InsertionMode = insertionMode;
					InsertionIndex = insertionIndex;
					Invalidate();
				}
			}

			base.OnDragOver(drgevent);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.ListView.ItemDrag" /> event.
		/// </summary>
		/// <param name="e">An <see cref="T:System.Windows.Forms.ItemDragEventArgs" /> that contains the event data.</param>
		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			if (AllowItemDrag && Items.Count > 1)
			{
				CancelListViewItemDragEventArgs args;

				args = new CancelListViewItemDragEventArgs((ListViewItem) e.Item);

				OnItemDragging(args);

				if (!args.Cancel)
				{
					IsRowDragInProgress = true;
					DoDragDrop(e.Item, DragDropEffects.Move);
				}
			}

			base.OnItemDrag(e);
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
		/// </summary>
		/// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data. </param>
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}

		/// <summary>
		/// Overrides <see cref="M:System.Windows.Forms.Control.WndProc(System.Windows.Forms.Message@)" />.
		/// </summary>
		/// <param name="m">The Windows <see cref="T:System.Windows.Forms.Message" /> to process.</param>
		[DebuggerStepThrough]
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			switch (m.Msg)
			{
				case WM_PAINT:
					OnWmPaint(ref m);
					break;
			}
		}

		#endregion

		#region Public Properties

		[Category("Behavior")]
		[DefaultValue(false)]
		public virtual bool AllowItemDrag
		{
			get { return _allowItemDrag; }
			set
			{
				if (AllowItemDrag != value)
				{
					_allowItemDrag = value;

					OnAllowItemDragChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the color of the insertion line drawn when dragging items within the control.
		/// </summary>
		/// <value>The color of the insertion line.</value>
		[Category("Appearance")]
		[DefaultValue(typeof(Color), "Red")]
		public virtual Color InsertionLineColor
		{
			get { return _insertionLineColor; }
			set
			{
				if (InsertionLineColor != value)
				{
					_insertionLineColor = value;

					OnInsertionLineColorChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets or sets the selected <see cref="ListViewItem"/>.
		/// </summary>
		/// <value>The selected <see cref="ListViewItem"/>.</value>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ListViewItem SelectedItem
		{
			get { return SelectedItems.Count != 0 ? SelectedItems[0] : null; }
			set
			{
				SelectedItems.Clear();
				if (value != null)
				{
					value.Selected = true;
				}

				FocusedItem = value;
			}
		}

		#endregion

		#region Protected Properties

		protected int InsertionIndex { get; set; }

		protected InsertionMode InsertionMode { get; set; }

		protected bool IsRowDragInProgress { get; set; }

		#endregion

		#region Protected Members

		/// <summary>
		/// Raises the <see cref="AllowItemDragChanged" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected virtual void OnAllowItemDragChanged(EventArgs e)
		{
			AllowItemDragChanged?.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="InsertionLineColorChanged" /> event.
		/// </summary>
		/// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
		protected virtual void OnInsertionLineColorChanged(EventArgs e)
		{
			InsertionLineColorChanged?.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="ItemDragDrop" /> event.
		/// </summary>
		/// <param name="e">The <see cref="ListViewItemDragEventArgs" /> instance containing the event data.</param>
		protected virtual void OnItemDragDrop(ListViewItemDragEventArgs e)
		{
			ItemDragDrop?.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="ItemDragDrop" /> event.
		/// </summary>
		/// <param name="e">The <see cref="ListViewItemDragEventArgs" /> instance containing the event data.</param>
		protected virtual void OnItemDragDropCompleted(ListViewItemDragEventArgs e)
		{
			ItemDragDropCompleted?.Invoke(this, e);
		}

		/// <summary>
		/// Raises the <see cref="ItemDragging" /> event.
		/// </summary>
		/// <param name="e">The <see cref="CancelListViewItemDragEventArgs" /> instance containing the event data.</param>
		protected virtual void OnItemDragging(CancelListViewItemDragEventArgs e)
		{
			ItemDragging?.Invoke(this, e);
		}

		protected virtual void OnWmPaint(ref Message m)
		{
			DrawInsertionLine();
		}

		#endregion

		#region Private Members

		private void DrawInsertionLine()
		{
			if (InsertionIndex != -1)
			{
				int index;

				index = InsertionIndex;

				if (index >= 0 && index < Items.Count)
				{
					Rectangle bounds;
					int x;
					int y;
					int width;

					bounds = Items[index].GetBounds(ItemBoundsPortion.Entire);
					x = 0; // aways fit the line to the client area, regardless of how the user is scrolling
					y = InsertionMode == InsertionMode.Before ? bounds.Top : bounds.Bottom;
					width = Math.Min(bounds.Width - bounds.Left,
						ClientSize.Width); // again, make sure the full width fits in the client area

					DrawInsertionLine(x, y, width);
				}
			}
		}

		private void DrawInsertionLine(int x1, int y, int width)
		{
			using (Graphics g = CreateGraphics())
			{
				Point[] leftArrowHead;
				Point[] rightArrowHead;
				int arrowHeadSize;
				int x2;

				x2 = x1 + width;
				arrowHeadSize = 7;
				leftArrowHead = new[]
				{
					new Point(x1, y - (arrowHeadSize / 2)), new Point(x1 + arrowHeadSize, y), new Point(x1, y + (arrowHeadSize / 2))
				};
				rightArrowHead = new[]
				{
					new Point(x2, y - (arrowHeadSize / 2)), new Point(x2 - arrowHeadSize, y), new Point(x2, y + (arrowHeadSize / 2))
				};

				using (Pen pen = new Pen(InsertionLineColor))
				{
					g.DrawLine(pen, x1, y, x2 - 1, y);
				}

				using (Brush brush = new SolidBrush(InsertionLineColor))
				{
					g.FillPolygon(brush, leftArrowHead);
					g.FillPolygon(brush, rightArrowHead);
				}
			}
		}

		#endregion

		#region Column Headers

		public void ColumnAutoSize()
		{
			AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			ColumnHeaderCollection cc = Columns;
			for (int i = 0; i < cc.Count; i++)
			{
				int colWidth = TextRenderer.MeasureText(cc[i].Text, Font).Width + 20;
				if (colWidth > cc[i].Width)
				{
					cc[i].Width = colWidth;
				}
			}
		}

		public void SetLastColumnWidth()
		{
			// Force the last ListView column width to occupy all the
			// available space.
			if (Columns.Count > 0)
			{
				Columns[Columns.Count - 1].Width = -2;
			}
		}


		#endregion

		#region Custom Theme


		private void List_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			// Fill header background with solid color.
			e.Graphics.FillRectangle(new SolidBrush(ThemeColorTable.BackgroundColor), e.Bounds);
			TextRenderer.DrawText(e.Graphics, e.Header.Text, Font, e.Bounds, ThemeColorTable.ForeColor, ThemeColorTable.BackgroundColor, TextFormatFlags.VerticalCenter);
		}

		private void List_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			var backgroundColor = ThemeColorTable.TextBoxBackgroundColor;

			//Using item selected here as this looks like a known bug since about 2006, in evidence when the ListView.HideSelection property is set to FALSE.
			//The only workaround on file is to use e.Item.Selected.
			if (e.Item.Selected)
			{
				// Draw the background and focus rectangle for a selected item.
				backgroundColor = ThemeColorTable.BackgroundColor;
				e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);
			}
			else
			{
				e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);
			}

			var textBounds = e.Bounds;
			if (CheckBoxes && e.ColumnIndex == 0)
			{
				Size cbSize = CalculateCheckBoxSize(e.SubItem);
				int top = e.Bounds.Top + (int)((e.Bounds.Height - cbSize.Height) / 2d);
				Rectangle cbBounds = new Rectangle(new Point(e.Bounds.X + 1, top), cbSize);

				ControlPaint.DrawCheckBox(e.Graphics, cbBounds, (e.Item.Checked ? ButtonState.Checked : ButtonState.Normal) | ButtonState.Flat);

				textBounds.X = textBounds.X + cbSize.Width + 1;
			}

			TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, textBounds, ThemeColorTable.ForeColor, backgroundColor, TextFormatFlags.VerticalCenter);
		}

		private Size CalculateCheckBoxSize(ListViewItem.ListViewSubItem lvsi)
		{
			return new Size(lvsi.Bounds.Height - 4, lvsi.Bounds.Height - 4);
		}

		private void List_DrawItem(object sender, DrawListViewItemEventArgs e)
		{

			if (View != View.Details)
			{
				var backgroundColor = ThemeColorTable.TextBoxBackgroundColor;
				if ((e.State & ListViewItemStates.Selected) != 0)
				{
					// Draw the background and focus rectangle for a selected item.
					backgroundColor = ThemeColorTable.BackgroundColor;
					e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);
					e.DrawFocusRectangle();
				}
				else
				{
					e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);
				}

				TextRenderer.DrawText(e.Graphics, e.Item.Text, e.Item.Font, e.Bounds, ThemeColorTable.ForeColor, backgroundColor, TextFormatFlags.VerticalCenter);
			}
		}



		#endregion

	}
}

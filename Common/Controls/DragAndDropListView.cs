using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace Common.Controls
{
	public class ListViewEx : ListView
	{
		private const string REORDER = "Reorder";
		private List<ListViewItem> _reorderExcludedRows = new List<ListViewItem>(); 

		private bool allowRowReorder = true;
		public bool AllowRowReorder
		{
			get
			{
				return this.allowRowReorder;
			}
			set
			{
				this.allowRowReorder = value;
				base.AllowDrop = value;
			}
		}

		public List<ListViewItem> ReorderExcludedItems
		{
			get { return _reorderExcludedRows.ToList(); }
		}

		public void ReorderExcludeItem(ListViewItem item)
		{
			_reorderExcludedRows.Add(item);
		}

		public new SortOrder Sorting
		{
			get
			{
				return SortOrder.None;
			}
			set
			{
				base.Sorting = SortOrder.None;
			}
		}

		public ListViewEx()
			: base()
		{
			OwnerDraw = true;
			AllowRowReorder = true;
			DrawItem += List_DrawItem;
			DrawColumnHeader += List_DrawColumnHeader;
			DrawSubItem += List_DrawSubItem;
		
			Layout += delegate
			{
				SetLastColumnWidth();
			};
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			
			if(!this.AllowRowReorder)
			{
				return;
			}
			if(base.SelectedItems.Count==0)
			{
				return;
			}
			Point cp = base.PointToClient(new Point(e.X, e.Y));
			ListViewItem dragToItem = base.GetItemAt(cp.X, cp.Y);
			if(dragToItem==null)
			{
				return;
			}
			int dropIndex = dragToItem.Index;
			if(dropIndex>base.SelectedItems[0].Index)
			{
				dropIndex++;
			}
			ArrayList insertItems = 
				new ArrayList(base.SelectedItems.Count);
			foreach(ListViewItem item in base.SelectedItems)
			{
				var newItem = (ListViewItem)item.Clone();
				newItem.Name = item.Name;
				insertItems.Add(newItem);
			}
			for(int i=insertItems.Count-1;i>=0;i--)
			{
				ListViewItem insertItem =
					(ListViewItem)insertItems[i];
				base.Items.Insert(dropIndex, insertItem);
			}
			foreach(ListViewItem removeItem in base.SelectedItems)
			{
				base.Items.Remove(removeItem);
			}
			base.OnDragDrop(e);
		}
		
		protected override void OnDragOver(DragEventArgs e)
		{
			if(!this.AllowRowReorder)
			{
				e.Effect = DragDropEffects.None;
				return;
			}
			if(!e.Data.GetDataPresent(DataFormats.Text))
			{
				e.Effect = DragDropEffects.None;
				return;
			}
			Point cp = base.PointToClient(new Point(e.X, e.Y));
			ListViewItem hoverItem = base.GetItemAt(cp.X, cp.Y);
			if(hoverItem==null || _reorderExcludedRows.Contains(hoverItem))
			{
				e.Effect = DragDropEffects.None;
				return;
			}
			foreach(ListViewItem moveItem in base.SelectedItems)
			{
				if(moveItem.Index==hoverItem.Index)
				{
					e.Effect = DragDropEffects.None;
					hoverItem.EnsureVisible();
					return;
				}
			}
			base.OnDragOver(e);
			String text = (String)e.Data.GetData(REORDER.GetType());
			if(text.CompareTo(REORDER)==0)
			{
				e.Effect = DragDropEffects.Move;
				hoverItem.EnsureVisible();
			}
			else
			{
				e.Effect = DragDropEffects.None;	
			}
		}		

		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			if(!this.AllowRowReorder)
			{
				e.Effect = DragDropEffects.None;
				return;
			}
			if(!e.Data.GetDataPresent(DataFormats.Text))
			{
				e.Effect = DragDropEffects.None;
				return;
			}
			base.OnDragEnter(e);
			String text = (String)e.Data.GetData(REORDER.GetType());
			if(text.CompareTo(REORDER)==0)
			{
				e.Effect = DragDropEffects.Move;
			}
			else
			{
				e.Effect = DragDropEffects.None;	
			}
		}

		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			if (_reorderExcludedRows.Contains(e.Item))
			{
				return;
			}
			base.OnItemDrag(e);
			if(!this.AllowRowReorder)
			{
				return;
			}
			base.DoDragDrop(REORDER, DragDropEffects.Move);
		}

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

		private void List_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
		{
			// Fill header background with solid color.
			e.Graphics.FillRectangle(new SolidBrush(ThemeColorTable.BackgroundColor), e.Bounds);
			TextRenderer.DrawText(e.Graphics, e.Header.Text, Font, e.Bounds, ThemeColorTable.ForeColor, ThemeColorTable.BackgroundColor, TextFormatFlags.VerticalCenter);
		}

		private void List_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
		{
			var backgroundColor = ThemeColorTable.TextBoxBackgroundColor;
			if ((e.ItemState & ListViewItemStates.Selected) != 0)
			{
				// Draw the background and focus rectangle for a selected item.
				backgroundColor = ThemeColorTable.BackgroundColor;
				e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);
			}
			else
			{
				e.Graphics.FillRectangle(new SolidBrush(backgroundColor), e.Bounds);
			}
			
			TextRenderer.DrawText(e.Graphics, e.SubItem.Text, e.Item.Font, e.Bounds, ThemeColorTable.ForeColor, backgroundColor, TextFormatFlags.VerticalCenter);
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

	}
}

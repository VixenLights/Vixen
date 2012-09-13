/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI {
	
	/// <summary>
	/// ListView component implementing the ILayerView interface.
	/// </summary>
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(LayerListView), "LayerListView.bmp")]
	public partial class LayerListView : ListView, ILayerView {

		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.LayerListView" />.
		/// </summary>
		public LayerListView() {
			DoubleBuffered = true;
			InitializeComponent();

			enableRenameLayer = false;

			selectedBrush = new SolidBrush(Color.FromArgb(128, Color.Gainsboro));
			backgroundBrush = new SolidBrush(BackColor);
			textBrush = new SolidBrush(ForeColor);

			// Fill image lists
			if (visibilityImageList.Images.Count == 0) {
				visibilityImageList.Images.Add(Properties.Resources.Unchecked);
				visibilityImageList.Images.Add(Properties.Resources.Checked);
			}
			if (stateImageList.Images.Count == 0) {
				stateImageList.Images.Add(Properties.Resources.Unchecked);
				stateImageList.Images.Add(Properties.Resources.Checked);
			}

			CreateColumns();
		}


		#region [Public] ILayerView Members

		#region Events

		/// <summary>
		/// Raised when a layer was selected.
		/// </summary>
		public event EventHandler<LayersEventArgs> SelectedLayerChanged;

		/// <summary>
		/// Raised when a layer was renamed.
		/// </summary>
		public event EventHandler<LayerRenamedEventArgs> LayerRenamed;

		/// <summary>
		/// Raised when the upper zoom treshold was changed.
		/// </summary>
		public event EventHandler<LayerZoomThresholdChangedEventArgs> LayerUpperZoomThresholdChanged;

		/// <summary>
		/// Raised when the lower zoom treshold was changed.
		/// </summary>
		public event EventHandler<LayerZoomThresholdChangedEventArgs> LayerLowerZoomThresholdChanged;

		/// <summary>
		/// Raised when a mouse button was pressed inside the control's boundaries.
		/// </summary>
		public event EventHandler<LayerMouseEventArgs> LayerViewMouseDown;

		/// <summary>
		/// Raised when the mouse was moved over the control.
		/// </summary>
		public event EventHandler<LayerMouseEventArgs> LayerViewMouseMove;

		/// <summary>
		/// Raised when a mouse button was released inside the control's boundaries.
		/// </summary>
		public event EventHandler<LayerMouseEventArgs> LayerViewMouseUp;

		#endregion


		void ILayerView.BeginUpdate() {
			SuspendLayout();
		}


		void ILayerView.EndUpdate() {
		   ResumeLayout();
		}

		
		void ILayerView.Clear() {
			ClearColumnsAndItems();
		}


		/// <override></override>
		public void AddLayer(Layer layer, bool isActive, bool isVisible) {
			if (layer == null) throw new ArgumentNullException("layer");
			if (FindItem(layer) != null) return;

			ListViewItem item = new ListViewItem(layer.Name);
			ListViewItem.ListViewSubItem subItem = null;
			for (int i = 0; i < Columns.Count; ++i) {
				if (i == idxColumnState)
					subItem = item.SubItems.Add(new ListViewItem.ListViewSubItem(item, layer.Name));
				else if (i == idxColumnName)
					item.SubItems.Add(new ListViewItem.ListViewSubItem(item, layer.Name));
				else if (i == idxColumnVisibility)
					item.SubItems.Add(new ListViewItem.ListViewSubItem(item, layer.Name));
				else if (i == idxColumnLowerZoomBound)
					item.SubItems.Add(new ListViewItem.ListViewSubItem(item, layer.LowerZoomThreshold.ToString()));
				else if (i == idxColumnUpperZoomBound)
					item.SubItems.Add(new ListViewItem.ListViewSubItem(item, layer.UpperZoomThreshold.ToString()));
			}
			item.Text = layer.Name;
			item.Tag = new LayerInfo(layer, isActive, isVisible);

			Items.Add(item);
			Refresh();
			//return Items.IndexOf(item);
		}


		/// <override></override>
		public void RefreshLayer(Layer layer, bool isActive, bool isVisible) {
			if (layer == null) throw new ArgumentNullException("layer");
			oldName = newName = string.Empty;
			ListViewItem item = FindItem(layer);
			if (item != null) {
				item.Text = layer.Name;
				item.Tag = new LayerInfo(layer, isActive, isVisible);
				Invalidate(item.Bounds);
			}
		}


		/// <override></override>
		public void RemoveLayer(Layer layer) {
			if (layer == null) throw new ArgumentNullException("layer");
			ListViewItem item = FindItem(layer);
			if (item != null) {
				Items.Remove(item);
				Invalidate(item.Bounds);
			}
		}


		/// <override></override>
		public void BeginEditLayerName(Layer layer) {
			if (LabelEdit) {
				enableRenameLayer = true;
				if (layer == null) throw new ArgumentNullException("layer");
				ListViewItem item = FindItem(layer);
				if (item != null && LabelEdit) item.BeginEdit();
			}
		}


		/// <override></override>
		public void BeginEditLayerMinZoomBound(Layer layer) {
			if (layer == null) throw new ArgumentNullException("layer");
			ListViewItem item = FindItem(layer);
			if (item != null) ShowUpDown(item, idxColumnLowerZoomBound);
		}


		/// <override></override>
		public void BeginEditLayerMaxZoomBound(Layer layer) {
			if (layer == null) throw new ArgumentNullException("layer");
			ListViewItem item = FindItem(layer);
			if (item != null) ShowUpDown(item, idxColumnUpperZoomBound);
		}


		/// <override></override>
		public void OpenContextMenu(int x, int y, IEnumerable<MenuItemDef> actions, Project project) {
			if (actions == null) throw new ArgumentNullException("actions");
			if (project == null) throw new ArgumentNullException("project");
			if (showDefaultContextMenu && contextMenuStrip != null) {
				contextMenuStrip.SuspendLayout();
				contextMenuStrip.Left = x;
				contextMenuStrip.Top = y;
				WinFormHelpers.BuildContextMenu(contextMenuStrip, actions, project, hideMenuItemsIfNotGranted);
				contextMenuStrip.Closing += contextMenuStrip_Closing;
				contextMenuStrip.Show(x, y);
				contextMenuStrip.ResumeLayout();
			}
		}


		/// <override></override>
		void ILayerView.Invalidate() {
		   Invalidate();
		}

		#endregion


		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		[Browsable(true)]
		public new string ProductVersion {
			get { return base.ProductVersion; }
		}


		/// <summary>
		/// Specifies if MenuItemDefs that are not granted should appear as MenuItems in the dynamic context menu.
		/// </summary>
		[Category("Behavior")]
		public bool HideDeniedMenuItems {
			get { return hideMenuItemsIfNotGranted; }
			set { hideMenuItemsIfNotGranted = value; }
		}


		/// <summary>
		/// If true, the standard context menu is created from MenuItemDefs. 
		/// If false, a user defined context menu is shown without creating additional menu items.
		/// </summary>
		[Category("Behavior")]
		public bool ShowDefaultContextMenu {
			get { return showDefaultContextMenu; }
			set { showDefaultContextMenu = value; }
		}


		#region [Protected] Overridden Methods

		/// <override></override>
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if (LayerViewMouseDown != null) 
				LayerViewMouseDown(this, GetMouseEventArgs(MouseEventType.MouseDown, e));
		}


		/// <override></override>
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if (LayerViewMouseMove != null)
				LayerViewMouseMove(this, GetMouseEventArgs(MouseEventType.MouseMove, e));
		}


		/// <override></override>
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if (LayerViewMouseUp!= null)
				LayerViewMouseUp(this, GetMouseEventArgs(MouseEventType.MouseUp, e));
			// If the selected item was changed, LabelEdit was deactivated so we have to reactivate it here
			if (!LabelEdit) LabelEdit = true;
		}


		/// <override></override>
		protected override void OnSelectedIndexChanged(EventArgs e) {
			if (LabelEdit) LabelEdit = false;
			base.OnSelectedIndexChanged(e);
			if (SelectedLayerChanged != null) SelectedLayerChanged(this, new LayersEventArgs(GetSelectedLayers()));
		}


		/// <override></override>
		protected override void OnBeforeLabelEdit(LabelEditEventArgs e) {
			if (enableRenameLayer) {
				LayerInfo layerInfo = (LayerInfo)Items[e.Item].Tag;
				oldName = layerInfo.layer.Name;
				newName = string.Empty;
			} else e.CancelEdit = true;
			base.OnBeforeLabelEdit(e);
		}


		/// <override></override>
		protected override void OnAfterLabelEdit(LabelEditEventArgs e) {
			base.OnAfterLabelEdit(e);
			enableRenameLayer = false;
			newName = e.Label;
			if (newName != null && oldName != newName && LayerRenamed != null) {
				LayerInfo layerInfo = (LayerInfo)Items[e.Item].Tag;
				LayerRenamed(this, new LayerRenamedEventArgs(layerInfo.layer, oldName, newName));
			}
			oldName = newName = string.Empty;
		}


		/// <override></override>
		protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e) {
			e.DrawDefault = true;
			base.OnDrawColumnHeader(e);
		}


		/// <override></override>
		protected override void OnDrawItem(DrawListViewItemEventArgs e) {
			base.OnDrawItem(e);

			Rectangle lineBounds = Rectangle.Empty;
			lineBounds.X = 0;
			lineBounds.Y = e.Bounds.Y;
			lineBounds.Width = Width;
			lineBounds.Height = e.Bounds.Height;

			if (e.ItemIndex % 2 == 0)
				backgroundBrush = Brushes.AliceBlue;
			else 
				backgroundBrush = Brushes.White;
			e.Graphics.FillRectangle(backgroundBrush, lineBounds);
			
			// This is a workaround for the disappearing subitems
			// ToDo: Find out why subitems keep disappearing and find a better solution than this
			for (int i = 0; i < e.Item.SubItems.Count; ++i)
				OnDrawSubItem(new DrawListViewSubItemEventArgs(e.Graphics, e.Bounds, e.Item, e.Item.SubItems[i], e.ItemIndex, i, null, e.State));

			if (SelectedItems.Count > 0 && e.Item.Selected)
				e.Graphics.FillRectangle(selectedBrush, e.Bounds);
		}


		/// <override></override>
		protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e) {
			LayerInfo layerInfo = (LayerInfo)e.Item.Tag;
			int imgIdx;
			if (e.ColumnIndex == idxColumnState) {
				imgIdx = layerInfo.isActive ? 1 : 0;
				e.Graphics.DrawImageUnscaled(stateImageList.Images[imgIdx], e.SubItem.Bounds.Location);
			} else if (e.ColumnIndex == idxColumnVisibility) {
				imgIdx = layerInfo.isVisible ? 1 : 0;
				e.Graphics.DrawImageUnscaled(visibilityImageList.Images[imgIdx], e.SubItem.Bounds.Location);
			} else if (e.ColumnIndex == idxColumnName) {
				Rectangle bounds = e.Item.GetBounds(ItemBoundsPortion.Label);
				e.Graphics.DrawString(e.Item.Text, Font, textBrush, bounds);
			} else if (e.ColumnIndex == idxColumnLowerZoomBound) {
				string txt;
				if (layerInfo.layer.LowerZoomThreshold == int.MinValue)
					txt = float.NegativeInfinity.ToString();
				else txt = string.Format("{0:D1} %",layerInfo.layer.LowerZoomThreshold);
				e.Graphics.DrawString(txt, Font, textBrush, e.SubItem.Bounds);
			} else if (e.ColumnIndex == idxColumnUpperZoomBound) {
				string txt;
				if (layerInfo.layer.UpperZoomThreshold == int.MaxValue)
					txt = float.PositiveInfinity.ToString();
				else txt = string.Format("{0:D1} %", layerInfo.layer.UpperZoomThreshold);
				e.Graphics.DrawString(txt, Font, textBrush, e.SubItem.Bounds);
			} else
				e.DrawDefault = true;
			base.OnDrawSubItem(e);
		}

		#endregion


		#region [Private] Methods

		private IEnumerable<Layer> GetSelectedLayers() {
			for (int i = 0; i < SelectedItems.Count; ++i)
				yield return ((LayerInfo)SelectedItems[i].Tag).layer;
		}


		private LayerMouseEventArgs GetMouseEventArgs(MouseEventType eventType, MouseEventArgs eventArgs) {
			// Find clicked layer item
			Layer layer = null;
			LayerItem layerItem = LayerItem.None;

			ListViewHitTestInfo hitTestInfo = HitTest(eventArgs.Location);
			if (hitTestInfo != null && hitTestInfo.Item != null) {
				// Set Layer
				layer = ((LayerInfo)hitTestInfo.Item.Tag).layer;
				// Determine subitem type
				int colIdx = hitTestInfo.Item.SubItems.IndexOf(hitTestInfo.SubItem);
				if (colIdx == idxColumnState)
					layerItem = LayerItem.ActiveState;
				else if (colIdx == idxColumnVisibility)
					layerItem = LayerItem.Visibility;
				else if (colIdx == idxColumnName) {
					// Check if the click was inside the layer name's text bounds
					Size txtSize = TextRenderer.MeasureText(hitTestInfo.SubItem.Text, Font);
					if (Geometry.RectangleContainsPoint(hitTestInfo.SubItem.Bounds.X, hitTestInfo.SubItem.Bounds.Y, txtSize.Width, txtSize.Height, eventArgs.X, eventArgs.Y))
						layerItem = LayerItem.Name;
				} else if (colIdx == idxColumnLowerZoomBound)
					layerItem = LayerItem.MinZoom;
				else if (colIdx == idxColumnUpperZoomBound)
					layerItem = LayerItem.MaxZoom;
			}
			// Create EventArgs and fire event
			layerItemMouseEventArgs.SetMouseEvent(layer, layerItem, eventType, eventArgs);
			return layerItemMouseEventArgs;
		}


		private ListViewItem FindItem(Layer layer) {
			ListViewItem result = null;
			for (int i = 0; i < Items.Count; ++i) {
				if (layer  == ((LayerInfo)Items[i].Tag).layer) {
					result = Items[i];
					break;
				}
			}
			return result;
		}


		private void CreateColumns() {
			SuspendLayout();
			Items.Clear();

			// first, create Columns
			Columns.Clear();
			Columns.Add(keyColumnName, "Name", 100);
			Columns.Add(keyColumnState, "Active", 17);
			Columns.Add(keyColumnVisibility, "Visible", 17);
			Columns.Add(keyColumnLowerZoomBound, "Min Zoom", 50);
			Columns.Add(keyColumnUpperZoomBound, "Max Zoom", 50);

			idxColumnState = Columns.IndexOfKey(keyColumnState);
			idxColumnVisibility = Columns.IndexOfKey(keyColumnVisibility);
			idxColumnName = Columns.IndexOfKey(keyColumnName);
			idxColumnLowerZoomBound = Columns.IndexOfKey(keyColumnLowerZoomBound);
			idxColumnUpperZoomBound = Columns.IndexOfKey(keyColumnUpperZoomBound);

			SetNumericUpdown(Columns[keyColumnLowerZoomBound]);
			SetNumericUpdown(Columns[keyColumnUpperZoomBound]);

			ResumeLayout();
			Invalidate();
		}


		private void ClearColumnsAndItems() {
			SuspendLayout();
			// Clear items
			Items.Clear();
			// Clear columns
			Columns.Clear();
			CreateColumns();
			//
			Invalidate();
			ResumeLayout();
		}


		private void SetNumericUpdown(ColumnHeader columnHeader) {
			NumericUpDown upDown = new NumericUpDown();
			upDown.Visible = false;
			upDown.Parent = this;
			upDown.Minimum = 0;
			upDown.Maximum = int.MaxValue;
			columnHeader.Tag = upDown;
		}


		private void ShowUpDown(ListViewItem item, int columnIndex) {
			LayerInfo layerInfo = (LayerInfo)item.Tag;
			int value;
			string columnKey;
			if (columnIndex == idxColumnUpperZoomBound) {
				columnKey = keyColumnUpperZoomBound;
				value = layerInfo.layer.UpperZoomThreshold;
			} else {
				columnKey = keyColumnLowerZoomBound;
				value = layerInfo.layer.LowerZoomThreshold;
			}

			NumericUpDown upDown = (NumericUpDown)Columns[columnKey].Tag;
			Rectangle bounds = item.SubItems[columnIndex].Bounds;
			upDown.Bounds = bounds;
			upDown.Top = (int)Math.Round(bounds.Top + (bounds.Height / 2f) - (upDown.Height / 2f));
			upDown.ValueChanged += upDown_ValueChanged;
			upDown.Leave += upDown_Leave;
			upDown.Tag = layerInfo.layer;
			upDown.Value = value;
			upDown.Visible = true;
			upDown.Focus();
		}


		private void upDown_Leave(object sender, EventArgs e) {
			if (sender is NumericUpDown) {
				NumericUpDown upDown = (NumericUpDown)sender;
				Layer layer = (Layer)upDown.Tag;
				
				upDown.Tag = null;
				upDown.Leave -= upDown_Leave;
				upDown.ValueChanged -= upDown_ValueChanged;
				upDown.Visible = false;

				if (sender == Columns[keyColumnLowerZoomBound].Tag) {
					if (LayerLowerZoomThresholdChanged != null)
						LayerLowerZoomThresholdChanged(this, new LayerZoomThresholdChangedEventArgs(layer, layer.LowerZoomThreshold, (int)upDown.Value));
				} else if (sender == Columns[keyColumnUpperZoomBound].Tag) {
					if (LayerUpperZoomThresholdChanged != null)
						LayerUpperZoomThresholdChanged(this, new LayerZoomThresholdChangedEventArgs(layer, layer.UpperZoomThreshold, (int)upDown.Value));
				}

			} else { }
		}

		
		private void upDown_ValueChanged(object sender, EventArgs e) {
			//
		}


		private void contextMenuStrip_Closing(object sender, CancelEventArgs e) {
			if (showDefaultContextMenu && sender == contextMenuStrip) {
				contextMenuStrip.Closing -= contextMenuStrip_Closing;
				WinFormHelpers.CleanUpContextMenu(this.contextMenuStrip);
			}
		}

		#endregion


		#region [Private] Types

		private struct LayerInfo : IEquatable<LayerInfo> {

			public LayerInfo(Layer layer, bool isActive, bool isVisible) {
				this.layer = layer;
				this.isActive = isActive;
				this.isVisible = isVisible;
			}

			public Layer layer;

			public bool isActive;

			public bool isVisible;

			public bool Equals(LayerInfo other) {
				return (other.layer == this.layer
					&& other.isActive == this.isActive
					&& other.isVisible == this.isVisible);
			}
		}


		private class LayerListViewMouseEventArgs : LayerMouseEventArgs {
			public LayerListViewMouseEventArgs(Layer layer, LayerItem item,
				MouseEventType eventType, MouseButtonsDg buttons, int clickCount, int wheelDelta,
				Point position, KeysDg modifiers)
				: base(layer, item, eventType, buttons, clickCount, wheelDelta, position, modifiers) {
			}


			protected internal LayerListViewMouseEventArgs()
				: base() { }
		

			protected internal void SetMouseEvent(Layer layer, LayerItem item, MouseEventType eventType, MouseEventArgs eventArgs) {
				this.SetMouseEvent(eventType, (MouseButtonsDg)eventArgs.Button, eventArgs.Clicks, eventArgs.Delta, eventArgs.Location, (KeysDg)Control.ModifierKeys);
				this.Layer = layer;
				this.Item = item;
			}


			protected internal void SetMouseEvent(Layer layer, LayerItem item, MouseEventType eventType, MouseEventArgsDg eventArgs) {
				this.SetMouseEvent(eventType, eventArgs.Buttons, eventArgs.Clicks, eventArgs.WheelDelta, eventArgs.Position, eventArgs.Modifiers);
				this.Item = item;
				this.Layer = layer;
			}
		}

		#endregion


		#region Fields

		private const string keyColumnState = "StateColumn";
		private const string keyColumnVisibility = "VisibilityColumn";
		private const string keyColumnName = "NameColumn";
		private const string keyColumnLowerZoomBound = "LowerZoomThresholdColumn";
		private const string keyColumnUpperZoomBound = "UpperZoomThresholdColumn";

		private int idxColumnState = -1;
		private int idxColumnVisibility = -1;
		private int idxColumnName = -1;
		private int idxColumnLowerZoomBound = -1;
		private int idxColumnUpperZoomBound = -1;

		private string oldName;
		private string newName;
		private bool enableRenameLayer;
		private bool showDefaultContextMenu = true;
		private bool hideMenuItemsIfNotGranted = false;

		// prawing and painting
		Brush selectedBrush = new SolidBrush(Color.FromArgb(128, Color.Gainsboro));
		Brush backgroundBrush = Brushes.White;
		Brush textBrush;

		LayerListViewMouseEventArgs layerItemMouseEventArgs = new LayerListViewMouseEventArgs();

		#endregion
	}
}

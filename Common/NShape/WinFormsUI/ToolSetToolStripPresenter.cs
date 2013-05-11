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
using System.ComponentModel;
using System.Windows.Forms;

using Dataweb.NShape.Advanced;
using Dataweb.NShape.Controllers;


namespace Dataweb.NShape.WinFormsUI {
	
	/// <summary>
	/// Implementation of a ToolSetPresenter based on a System.Windows.Forms.ToolStrip.
	/// </summary>
	public partial class ToolSetToolStripPresenter : ToolStrip {
		
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ToolSetToolStripPresenter" />.
		/// </summary>
		public ToolSetToolStripPresenter() {
			InitializeComponent();
		}


		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.NShape.WinFormsUI.ToolSetToolStripPresenter" />.
		/// </summary>
		public ToolSetToolStripPresenter(IContainer container) {
			container.Add(this);
			InitializeComponent();
		}


		/// <summary>
		/// Specifies the version of the assembly containing the component.
		/// </summary>
		[Category("NShape")]
		[Browsable(true)]
		public new string ProductVersion {
			get { return base.ProductVersion; }
		}


		/// <summary>
		/// Specifies the controller for this presenter.
		/// </summary>
		[Category("NShape")]
		public ToolSetController ToolSetController {
			get { return toolSetController; }
			set {
				if (toolSetController != null) UnregisterToolBoxEventHandlers();
				toolSetController = value;
				if (toolSetController != null) RegisterToolBoxEventHandlers();
			}
		}


		/// <summary>
		/// Specifies whether MenuItemDefs should not appear in the dynamic context menu if they are not granted.
		/// </summary>
		[Category("Behavior")]
		public bool HideDeniedMenuItems {
			get { return hideMenuItemsIfNotGranted; }
			set { hideMenuItemsIfNotGranted = value; }
		}


		private ToolStripItem FindItem(Tool tool) {
			for (int i = Items.Count - 1; i >= 0; --i) {
				if (Items[i].Tag == tool)
					return Items[i];
			}
			return null;
		}


		private ToolStripItem CreateItem(Tool tool) {
			ToolStripButton button = new ToolStripButton(null, tool.SmallIcon);
			button.Tag = tool;
			button.CheckOnClick = true;
			//button.Click += toolStripItem_Click;
			//button.DoubleClick += toolBoxStrip_DoubleClick;
			button.ToolTipText = tool.ToolTipText;
			button.DoubleClickEnabled = true;
			return button;
		}


		#region [Private] (Un)Registering for events

		private void RegisterToolStripEvents() {
			this.ItemClicked += new ToolStripItemClickedEventHandler(ToolSetToolStripPresenter_ItemClicked);
		}


		private void RegisterToolBoxEventHandlers() {
			toolSetController.Cleared += toolSetController_Cleared;
			toolSetController.ToolAdded += toolSetController_ToolAdded;
			toolSetController.ToolChanged += toolSetController_ToolChanged;
			toolSetController.ToolRemoved += toolSetController_ToolRemoved;
			toolSetController.ToolSelected += toolSetController_ToolSelected;
		}


		private void UnregisterToolBoxEventHandlers() {
			toolSetController.ToolSelected -= toolSetController_ToolSelected;
			toolSetController.ToolRemoved -= toolSetController_ToolRemoved;
			toolSetController.ToolChanged -= toolSetController_ToolChanged;
			toolSetController.ToolAdded -= toolSetController_ToolAdded;
			toolSetController.Cleared -= toolSetController_Cleared;
		}

		#endregion


		#region [Private] ToolStrip event handler implementations

		private void ToolSetToolStripPresenter_ItemClicked(object sender, ToolStripItemClickedEventArgs e) {
			if (e.ClickedItem.Tag is Tool) toolSetController.SelectTool((Tool)e.ClickedItem.Tag);
		}


		private void toolStripItem_Click(object sender, EventArgs e) {
		}

		#endregion


		#region [Private] ToolSetController event handler implementations

		private void toolSetController_ToolSelected(object sender, ToolEventArgs e) {
			ToolStripItem item = FindItem(e.Tool);
			if (item != null) item.Select();
		}


		private void toolSetController_Cleared(object sender, EventArgs args) {
			Items.Clear();
		}


		private void toolSetController_ToolAdded(object sender, ToolEventArgs e) {
			// SaveChanges the list view: Move this to ToolSetListViewPresenter
			if (FindItem(e.Tool) != null)
				throw new NShapeException(string.Format("Tool {0} already exists.", e.Tool.Title));
			ToolStripItem item = CreateItem(e.Tool);
			// ToDo: Put the tool into the right group, seperrate groups by seperators
			//   if (!string.IsNullOrEmpty(e.Tool.Category)) {
			//      foreach (ListViewGroup group in listView.Groups) {
			//         if (group.Name.Equals(e.Tool.Category, StringComparison.InvariantCultureIgnoreCase)) {
			//            item.Group = group;
			//            break;
			//         }
			//      }
			//      if (item.Group == null) {
			//         ListViewGroup group = new ListViewGroup(e.Tool.Category, e.Tool.Category);
			//         listView.Groups.Add(group);
			//         item.Group = group;
			//      }
			//   }
			//   // Adjust the heading column in the list view
			//   if (listView.Columns[headerName] != null) {
			//      Graphics gfx = Graphics.FromHwnd(listView.Handle);
			//      if (gfx != null) {
			//         SizeF txtSize = gfx.MeasureString(e.Tool.Title, listView.Font);
			//         if (listView.Columns[headerName].Width < txtSize.Width + listView.SmallImageList.ImageSize.Width)
			//            listView.Columns[headerName].Width = (int)Math.Ceiling(txtSize.Width + listView.SmallImageList.ImageSize.Width);
			//         gfx.Dispose();
			//         gfx = null;
			//      }
			//   }
			// Add the item and select if default
			Items.Add(item);
		}


		private void toolSetController_ToolChanged(object sender, ToolEventArgs e) {
			//if (listView != null && e.Tool is TemplateTool) {
			//   listView.BeginUpdate();
			//   ListViewItem item = FindItem(e.Tool);
			//   if (item != null) {
			//      TemplateTool tool = (TemplateTool)e.Tool;
			//      largeImageList.Images[item.ImageIndex] = tool.LargeIcon;
			//      smallImageList.Images[item.ImageIndex] = tool.SmallIcon;
			//   }
			//   listView.EndUpdate();
			//}
		}


		private void toolSetController_ToolRemoved(object sender, ToolEventArgs e) {
			//listView.SuspendLayout();
			//ListViewItem item = FindItem(e.Tool);
			//listView.Items.Remove(item);
			//listView.ResumeLayout();
		}

		#endregion


		private ToolSetController toolSetController;
		private bool hideMenuItemsIfNotGranted = false;
	}
}

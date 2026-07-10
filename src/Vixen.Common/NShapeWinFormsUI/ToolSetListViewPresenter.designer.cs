namespace Dataweb.NShape.WinFormsUI {

	partial class ToolSetListViewPresenter {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing) {
				// Release managed resources
				if (toolSetController != null) UnregisterToolBoxEventHandlers();
				if (listView != null) UnregisterListViewEventHandlers();
				smallImageList.Dispose();
				largeImageList.Dispose();
				if (components != null) components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.menuItemLoadLibrary = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemNewShapeTemplate = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemNewModelTemplate = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemEditTemplate = new System.Windows.Forms.ToolStripMenuItem();
			this.menuItemDeleteTemplate = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.menuItemEditStyles = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.presenterPrivateContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			// 
			// menuItemLoadLibrary
			// 
			this.menuItemLoadLibrary.Name = "menuItemLoadLibrary";
			this.menuItemLoadLibrary.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 6);
			// 
			// menuItemNewShapeTemplate
			// 
			this.menuItemNewShapeTemplate.Name = "menuItemNewShapeTemplate";
			this.menuItemNewShapeTemplate.Size = new System.Drawing.Size(32, 19);
			// 
			// menuItemNewModelTemplate
			// 
			this.menuItemNewModelTemplate.Name = "menuItemNewModelTemplate";
			this.menuItemNewModelTemplate.Size = new System.Drawing.Size(32, 19);
			// 
			// menuItemEditTemplate
			// 
			this.menuItemEditTemplate.Name = "menuItemEditTemplate";
			this.menuItemEditTemplate.Size = new System.Drawing.Size(32, 19);
			// 
			// menuItemDeleteTemplate
			// 
			this.menuItemDeleteTemplate.Name = "menuItemDeleteTemplate";
			this.menuItemDeleteTemplate.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 6);
			// 
			// menuItemEditStyles
			// 
			this.menuItemEditStyles.Name = "menuItemEditStyles";
			this.menuItemEditStyles.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 6);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(32, 19);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 6);
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(32, 19);
			// 
			// adapterContextMenu
			// 
			this.presenterPrivateContextMenu.Name = "adapterContextMenuStrip";
			this.presenterPrivateContextMenu.Size = new System.Drawing.Size(61, 4);

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem menuItemLoadLibrary;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem menuItemNewShapeTemplate;
		private System.Windows.Forms.ToolStripMenuItem menuItemNewModelTemplate;
		private System.Windows.Forms.ToolStripMenuItem menuItemEditTemplate;
		private System.Windows.Forms.ToolStripMenuItem menuItemDeleteTemplate;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem menuItemEditStyles;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
		private System.Windows.Forms.ContextMenuStrip presenterPrivateContextMenu;
	}
}

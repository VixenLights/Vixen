namespace Dataweb.NShape.WinFormsUI {
	partial class LayerEditor {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
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
			this.controller = new Dataweb.NShape.Controllers.LayerController();
			this.presenter = new Dataweb.NShape.Controllers.LayerPresenter();
			this.layerListView = new Dataweb.NShape.WinFormsUI.LayerListView();
			this.SuspendLayout();
			// 
			// controller
			// 
			this.controller.DiagramSetController = null;
			// 
			// presenter
			// 
			this.presenter.DiagramPresenter = null;
			this.presenter.HideDeniedMenuItems = false;
			this.presenter.LayerController = this.controller;
			this.presenter.LayerView = this.layerListView;
			// 
			// layerListView
			// 
			this.layerListView.AllowColumnReorder = true;
			this.layerListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layerListView.FullRowSelect = true;
			this.layerListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.layerListView.HideDeniedMenuItems = false;
			this.layerListView.HideSelection = false;
			this.layerListView.LabelEdit = true;
			this.layerListView.LabelWrap = false;
			this.layerListView.Location = new System.Drawing.Point(0, 0);
			this.layerListView.Name = "layerListView";
			this.layerListView.OwnerDraw = true;
			this.layerListView.ShowDefaultContextMenu = true;
			this.layerListView.ShowGroups = false;
			this.layerListView.Size = new System.Drawing.Size(244, 357);
			this.layerListView.TabIndex = 0;
			this.layerListView.UseCompatibleStateImageBehavior = false;
			this.layerListView.View = System.Windows.Forms.View.Details;
			// 
			// LayerEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layerListView);
			this.Name = "LayerEditor";
			this.Size = new System.Drawing.Size(244, 357);
			this.ResumeLayout(false);

		}

		#endregion

		private Dataweb.NShape.Controllers.LayerController controller;
		private Dataweb.NShape.Controllers.LayerPresenter presenter;
		private LayerListView layerListView;
	}
}

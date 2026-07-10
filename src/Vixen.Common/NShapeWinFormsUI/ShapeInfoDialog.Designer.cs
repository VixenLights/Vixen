namespace Dataweb.NShape.WinFormsUI {

	/// <summary>
	/// Displays information on a shape
	/// </summary>
	partial class ShapeInfoDialog {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.display = new Dataweb.NShape.WinFormsUI.Display();
			this.diagramSetController = new Dataweb.NShape.Controllers.DiagramSetController();
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.templateNameLbl = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.shapeTypeLbl = new System.Windows.Forms.Label();
			this.libraryNameLbl = new System.Windows.Forms.Label();
			this.fullNameLbl = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.permissionsLbl = new System.Windows.Forms.Label();
			this.ctrlPointListView = new System.Windows.Forms.ListView();
			this.columnId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnCapabilities = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnConnectedShapes = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel1 = new System.Windows.Forms.Panel();
			this.okButton = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tableLayoutPanel.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.ctrlPointListView);
			this.splitContainer1.Size = new System.Drawing.Size(688, 441);
			this.splitContainer1.SplitterDistance = 213;
			this.splitContainer1.SplitterWidth = 5;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.display);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel);
			this.splitContainer2.Size = new System.Drawing.Size(688, 213);
			this.splitContainer2.SplitterDistance = 419;
			this.splitContainer2.SplitterWidth = 5;
			this.splitContainer2.TabIndex = 0;
			// 
			// display
			// 
			this.display.AllowDrop = true;
			this.display.BackColorGradient = System.Drawing.SystemColors.Control;
			this.display.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.display.DiagramSetController = this.diagramSetController;
			this.display.Dock = System.Windows.Forms.DockStyle.Fill;
			this.display.GridColor = System.Drawing.Color.Gainsboro;
			this.display.GridSize = 19;
			this.display.ImeMode = System.Windows.Forms.ImeMode.NoControl;
			this.display.Location = new System.Drawing.Point(0, 0);
			this.display.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.display.Name = "display";
			this.display.PropertyController = null;
			this.display.SelectionHilightColor = System.Drawing.Color.Firebrick;
			this.display.SelectionInactiveColor = System.Drawing.Color.Gray;
			this.display.SelectionInteriorColor = System.Drawing.Color.WhiteSmoke;
			this.display.SelectionNormalColor = System.Drawing.Color.DarkGreen;
			this.display.Size = new System.Drawing.Size(419, 213);
			this.display.SnapToGrid = false;
			this.display.TabIndex = 0;
			this.display.ToolPreviewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
			this.display.ToolPreviewColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(70)))), ((int)(((byte)(130)))), ((int)(((byte)(180)))));
			this.display.ZoomWithMouseWheel = true;
			// 
			// diagramSetController
			// 
			this.diagramSetController.ActiveTool = null;
			this.diagramSetController.Project = null;
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.Controls.Add(this.label1, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.templateNameLbl, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.label4, 0, 4);
			this.tableLayoutPanel.Controls.Add(this.label3, 0, 3);
			this.tableLayoutPanel.Controls.Add(this.label2, 0, 2);
			this.tableLayoutPanel.Controls.Add(this.shapeTypeLbl, 1, 2);
			this.tableLayoutPanel.Controls.Add(this.libraryNameLbl, 1, 3);
			this.tableLayoutPanel.Controls.Add(this.fullNameLbl, 1, 4);
			this.tableLayoutPanel.Controls.Add(this.label5, 0, 6);
			this.tableLayoutPanel.Controls.Add(this.permissionsLbl, 1, 6);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 7;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.tableLayoutPanel.Size = new System.Drawing.Size(264, 213);
			this.tableLayoutPanel.TabIndex = 6;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label1.Location = new System.Drawing.Point(4, 0);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(62, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Template";
			// 
			// templateNameLbl
			// 
			this.templateNameLbl.AutoSize = true;
			this.templateNameLbl.Location = new System.Drawing.Point(74, 0);
			this.templateNameLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.templateNameLbl.Name = "templateNameLbl";
			this.templateNameLbl.Size = new System.Drawing.Size(0, 13);
			this.templateNameLbl.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(4, 64);
			this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Full Name";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(4, 51);
			this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(38, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Library";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(4, 38);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(31, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Type";
			// 
			// shapeTypeLbl
			// 
			this.shapeTypeLbl.AutoSize = true;
			this.shapeTypeLbl.Location = new System.Drawing.Point(74, 38);
			this.shapeTypeLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.shapeTypeLbl.Name = "shapeTypeLbl";
			this.shapeTypeLbl.Size = new System.Drawing.Size(0, 13);
			this.shapeTypeLbl.TabIndex = 5;
			// 
			// libraryNameLbl
			// 
			this.libraryNameLbl.AutoSize = true;
			this.libraryNameLbl.Location = new System.Drawing.Point(74, 51);
			this.libraryNameLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.libraryNameLbl.Name = "libraryNameLbl";
			this.libraryNameLbl.Size = new System.Drawing.Size(0, 13);
			this.libraryNameLbl.TabIndex = 6;
			// 
			// fullNameLbl
			// 
			this.fullNameLbl.AutoSize = true;
			this.fullNameLbl.Location = new System.Drawing.Point(74, 64);
			this.fullNameLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.fullNameLbl.Name = "fullNameLbl";
			this.fullNameLbl.Size = new System.Drawing.Size(0, 13);
			this.fullNameLbl.TabIndex = 7;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(4, 102);
			this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(62, 13);
			this.label5.TabIndex = 8;
			this.label5.Text = "Permissions";
			// 
			// permissionsLbl
			// 
			this.permissionsLbl.AutoSize = true;
			this.permissionsLbl.Location = new System.Drawing.Point(74, 102);
			this.permissionsLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.permissionsLbl.Name = "permissionsLbl";
			this.permissionsLbl.Size = new System.Drawing.Size(0, 13);
			this.permissionsLbl.TabIndex = 9;
			// 
			// ctrlPointListView
			// 
			this.ctrlPointListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnId,
            this.columnCapabilities,
            this.columnConnectedShapes});
			this.ctrlPointListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ctrlPointListView.FullRowSelect = true;
			this.ctrlPointListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.ctrlPointListView.HideSelection = false;
			this.ctrlPointListView.Location = new System.Drawing.Point(0, 0);
			this.ctrlPointListView.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.ctrlPointListView.Name = "ctrlPointListView";
			this.ctrlPointListView.Size = new System.Drawing.Size(688, 223);
			this.ctrlPointListView.TabIndex = 0;
			this.ctrlPointListView.UseCompatibleStateImageBehavior = false;
			this.ctrlPointListView.View = System.Windows.Forms.View.Details;
			this.ctrlPointListView.SelectedIndexChanged += new System.EventHandler(this.ctrlPointListView_SelectedIndexChanged);
			// 
			// columnId
			// 
			this.columnId.Text = "Point Id";
			// 
			// columnCapabilities
			// 
			this.columnCapabilities.Text = "Capabilities";
			this.columnCapabilities.Width = 200;
			// 
			// columnConnectedShapes
			// 
			this.columnConnectedShapes.Text = "Connected Shapes";
			this.columnConnectedShapes.Width = 250;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.okButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 441);
			this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(688, 62);
			this.panel1.TabIndex = 1;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(572, 18);
			this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(100, 28);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// ShapeInfoDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(688, 503);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.panel1);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "ShapeInfoDialog";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Shape Info";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Label shapeTypeLbl;
		private System.Windows.Forms.Label templateNameLbl;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.ListView ctrlPointListView;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label libraryNameLbl;
		private System.Windows.Forms.Label fullNameLbl;
		private Display display;
		private System.Windows.Forms.ColumnHeader columnId;
		private System.Windows.Forms.ColumnHeader columnCapabilities;
		private System.Windows.Forms.ColumnHeader columnConnectedShapes;
		private Controllers.DiagramSetController diagramSetController;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label permissionsLbl;
	}

}
namespace Dataweb.NShape.WinFormsUI {

	partial class TemplatePresenter {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.templateNameLabel = new System.Windows.Forms.Label();
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.titleTextBox = new System.Windows.Forms.TextBox();
			this.descriptionTextBox = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.headerSplitContainer = new System.Windows.Forms.SplitContainer();
			this.modelObjectComboBox = new System.Windows.Forms.ComboBox();
			this.shapeComboBox = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.templateShapeLabel = new System.Windows.Forms.Label();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.shapePropertiesTab = new System.Windows.Forms.TabPage();
			this.shapePropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.modelPropertiesTab = new System.Windows.Forms.TabPage();
			this.modelPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.controlPointsTab = new System.Windows.Forms.TabPage();
			this.controlPointMappingGrid = new System.Windows.Forms.DataGridView();
			this.ControlPointColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TerminalColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.propertyMappingTab = new System.Windows.Forms.TabPage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.propertyMappingGrid = new System.Windows.Forms.DataGridView();
			this.modelPropertyColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.shapePropertyColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.label1 = new System.Windows.Forms.Label();
			this.valueMappingGrid = new System.Windows.Forms.DataGridView();
			this.modelValueColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.shapeValueColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.label2 = new System.Windows.Forms.Label();
			this.previewPanel = new System.Windows.Forms.Panel();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.propertyController = new Dataweb.NShape.Controllers.PropertyController();
			this.propertyPresenter = new Dataweb.NShape.WinFormsUI.PropertyPresenter();
			this.headerSplitContainer.Panel1.SuspendLayout();
			this.headerSplitContainer.Panel2.SuspendLayout();
			this.headerSplitContainer.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.shapePropertiesTab.SuspendLayout();
			this.modelPropertiesTab.SuspendLayout();
			this.controlPointsTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.controlPointMappingGrid)).BeginInit();
			this.propertyMappingTab.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.propertyMappingGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.valueMappingGrid)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// templateNameLabel
			// 
			this.templateNameLabel.AutoSize = true;
			this.templateNameLabel.Location = new System.Drawing.Point(3, 60);
			this.templateNameLabel.Name = "templateNameLabel";
			this.templateNameLabel.Size = new System.Drawing.Size(38, 13);
			this.templateNameLabel.TabIndex = 0;
			this.templateNameLabel.Text = "Name:";
			// 
			// nameTextBox
			// 
			this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.nameTextBox.Location = new System.Drawing.Point(50, 57);
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.Size = new System.Drawing.Size(309, 20);
			this.nameTextBox.TabIndex = 2;
			this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(3, 86);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(30, 13);
			this.label4.TabIndex = 5;
			this.label4.Text = "Title:";
			// 
			// titleTextBox
			// 
			this.titleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.titleTextBox.Location = new System.Drawing.Point(50, 83);
			this.titleTextBox.Name = "titleTextBox";
			this.titleTextBox.Size = new System.Drawing.Size(309, 20);
			this.titleTextBox.TabIndex = 3;
			this.titleTextBox.TextChanged += new System.EventHandler(this.titleTextBox_TextChanged);
			// 
			// descriptionTextBox
			// 
			this.descriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.descriptionTextBox.Location = new System.Drawing.Point(6, 22);
			this.descriptionTextBox.Multiline = true;
			this.descriptionTextBox.Name = "descriptionTextBox";
			this.descriptionTextBox.Size = new System.Drawing.Size(355, 81);
			this.descriptionTextBox.TabIndex = 4;
			this.descriptionTextBox.TextChanged += new System.EventHandler(this.descriptionTextBox_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 6);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(63, 13);
			this.label5.TabIndex = 7;
			this.label5.Text = "Description:";
			// 
			// headerSplitContainer
			// 
			this.headerSplitContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.headerSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.headerSplitContainer.Name = "headerSplitContainer";
			// 
			// headerSplitContainer.Panel1
			// 
			this.headerSplitContainer.Panel1.Controls.Add(this.titleTextBox);
			this.headerSplitContainer.Panel1.Controls.Add(this.label4);
			this.headerSplitContainer.Panel1.Controls.Add(this.modelObjectComboBox);
			this.headerSplitContainer.Panel1.Controls.Add(this.shapeComboBox);
			this.headerSplitContainer.Panel1.Controls.Add(this.templateNameLabel);
			this.headerSplitContainer.Panel1.Controls.Add(this.nameTextBox);
			this.headerSplitContainer.Panel1.Controls.Add(this.label3);
			this.headerSplitContainer.Panel1.Controls.Add(this.templateShapeLabel);
			// 
			// headerSplitContainer.Panel2
			// 
			this.headerSplitContainer.Panel2.Controls.Add(this.label5);
			this.headerSplitContainer.Panel2.Controls.Add(this.descriptionTextBox);
			this.headerSplitContainer.Size = new System.Drawing.Size(731, 111);
			this.headerSplitContainer.SplitterDistance = 362;
			this.headerSplitContainer.TabIndex = 4;
			this.headerSplitContainer.TabStop = false;
			// 
			// modelObjectComboBox
			// 
			this.modelObjectComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.modelObjectComboBox.FormattingEnabled = true;
			this.modelObjectComboBox.Location = new System.Drawing.Point(50, 30);
			this.modelObjectComboBox.Name = "modelObjectComboBox";
			this.modelObjectComboBox.Size = new System.Drawing.Size(309, 21);
			this.modelObjectComboBox.TabIndex = 1;
			this.modelObjectComboBox.SelectedIndexChanged += new System.EventHandler(this.modelObjectComboBox_SelectedIndexChanged);
			// 
			// shapeComboBox
			// 
			this.shapeComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
							| System.Windows.Forms.AnchorStyles.Right)));
			this.shapeComboBox.FormattingEnabled = true;
			this.shapeComboBox.Location = new System.Drawing.Point(50, 3);
			this.shapeComboBox.MaxDropDownItems = 15;
			this.shapeComboBox.Name = "shapeComboBox";
			this.shapeComboBox.Size = new System.Drawing.Size(309, 21);
			this.shapeComboBox.TabIndex = 0;
			this.shapeComboBox.SelectedIndexChanged += new System.EventHandler(this.shapesComboBox_SelectedIndexChanged);
			this.shapeComboBox.Click += new System.EventHandler(this.shapesComboBox_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(3, 33);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Model:";
			// 
			// templateShapeLabel
			// 
			this.templateShapeLabel.AutoSize = true;
			this.templateShapeLabel.Location = new System.Drawing.Point(3, 6);
			this.templateShapeLabel.Name = "templateShapeLabel";
			this.templateShapeLabel.Size = new System.Drawing.Size(41, 13);
			this.templateShapeLabel.TabIndex = 2;
			this.templateShapeLabel.Text = "Shape:";
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.shapePropertiesTab);
			this.tabControl.Controls.Add(this.modelPropertiesTab);
			this.tabControl.Controls.Add(this.controlPointsTab);
			this.tabControl.Controls.Add(this.propertyMappingTab);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(365, 396);
			this.tabControl.TabIndex = 11;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// shapePropertiesTab
			// 
			this.shapePropertiesTab.Controls.Add(this.shapePropertyGrid);
			this.shapePropertiesTab.Location = new System.Drawing.Point(4, 22);
			this.shapePropertiesTab.Name = "shapePropertiesTab";
			this.shapePropertiesTab.Padding = new System.Windows.Forms.Padding(3);
			this.shapePropertiesTab.Size = new System.Drawing.Size(357, 370);
			this.shapePropertiesTab.TabIndex = 0;
			this.shapePropertiesTab.Text = "Presentation";
			this.shapePropertiesTab.UseVisualStyleBackColor = true;
			// 
			// shapePropertyGrid
			// 
			this.shapePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.shapePropertyGrid.Location = new System.Drawing.Point(3, 3);
			this.shapePropertyGrid.Name = "shapePropertyGrid";
			this.shapePropertyGrid.Size = new System.Drawing.Size(351, 364);
			this.shapePropertyGrid.TabIndex = 6;
			// 
			// modelPropertiesTab
			// 
			this.modelPropertiesTab.Controls.Add(this.modelPropertyGrid);
			this.modelPropertiesTab.Location = new System.Drawing.Point(4, 22);
			this.modelPropertiesTab.Name = "modelPropertiesTab";
			this.modelPropertiesTab.Padding = new System.Windows.Forms.Padding(3);
			this.modelPropertiesTab.Size = new System.Drawing.Size(357, 370);
			this.modelPropertiesTab.TabIndex = 3;
			this.modelPropertiesTab.Text = "Model";
			this.modelPropertiesTab.UseVisualStyleBackColor = true;
			// 
			// modelPropertyGrid
			// 
			this.modelPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.modelPropertyGrid.Location = new System.Drawing.Point(3, 3);
			this.modelPropertyGrid.Name = "modelPropertyGrid";
			this.modelPropertyGrid.Size = new System.Drawing.Size(351, 364);
			this.modelPropertyGrid.TabIndex = 8;
			// 
			// controlPointsTab
			// 
			this.controlPointsTab.Controls.Add(this.controlPointMappingGrid);
			this.controlPointsTab.Location = new System.Drawing.Point(4, 22);
			this.controlPointsTab.Name = "controlPointsTab";
			this.controlPointsTab.Padding = new System.Windows.Forms.Padding(3);
			this.controlPointsTab.Size = new System.Drawing.Size(357, 370);
			this.controlPointsTab.TabIndex = 1;
			this.controlPointsTab.Text = "Connection Points";
			this.controlPointsTab.UseVisualStyleBackColor = true;
			// 
			// controlPointMappingGrid
			// 
			this.controlPointMappingGrid.AllowUserToAddRows = false;
			this.controlPointMappingGrid.AllowUserToDeleteRows = false;
			this.controlPointMappingGrid.AllowUserToResizeRows = false;
			this.controlPointMappingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.controlPointMappingGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ControlPointColumn,
            this.TerminalColumn});
			this.controlPointMappingGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.controlPointMappingGrid.Location = new System.Drawing.Point(3, 3);
			this.controlPointMappingGrid.Name = "controlPointMappingGrid";
			this.controlPointMappingGrid.RowHeadersVisible = false;
			this.controlPointMappingGrid.Size = new System.Drawing.Size(351, 364);
			this.controlPointMappingGrid.TabIndex = 10;
			this.controlPointMappingGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.controlPointMappingGrid_CellValueChanged);
			this.controlPointMappingGrid.Resize += new System.EventHandler(this.dataGridView_Resize);
			// 
			// ControlPointColumn
			// 
			this.ControlPointColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.ControlPointColumn.HeaderText = "Control Point";
			this.ControlPointColumn.Name = "ControlPointColumn";
			this.ControlPointColumn.ReadOnly = true;
			this.ControlPointColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.ControlPointColumn.Width = 148;
			// 
			// TerminalColumn
			// 
			this.TerminalColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.TerminalColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
			this.TerminalColumn.HeaderText = "Terminal";
			this.TerminalColumn.Name = "TerminalColumn";
			this.TerminalColumn.Width = 200;
			// 
			// propertyMappingTab
			// 
			this.propertyMappingTab.Controls.Add(this.splitContainer2);
			this.propertyMappingTab.Location = new System.Drawing.Point(4, 22);
			this.propertyMappingTab.Name = "propertyMappingTab";
			this.propertyMappingTab.Padding = new System.Windows.Forms.Padding(3);
			this.propertyMappingTab.Size = new System.Drawing.Size(357, 370);
			this.propertyMappingTab.TabIndex = 2;
			this.propertyMappingTab.Text = "Model Visualization";
			this.propertyMappingTab.UseVisualStyleBackColor = true;
			// 
			// splitContainer2
			// 
			this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(3, 3);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.propertyMappingGrid);
			this.splitContainer2.Panel1.Controls.Add(this.label1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.valueMappingGrid);
			this.splitContainer2.Panel2.Controls.Add(this.label2);
			this.splitContainer2.Size = new System.Drawing.Size(351, 364);
			this.splitContainer2.SplitterDistance = 175;
			this.splitContainer2.TabIndex = 2;
			// 
			// propertyMappingGrid
			// 
			this.propertyMappingGrid.AllowUserToAddRows = false;
			this.propertyMappingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.propertyMappingGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.modelPropertyColumn,
            this.shapePropertyColumn});
			this.propertyMappingGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyMappingGrid.Location = new System.Drawing.Point(0, 17);
			this.propertyMappingGrid.Name = "propertyMappingGrid";
			this.propertyMappingGrid.RowHeadersVisible = false;
			this.propertyMappingGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.propertyMappingGrid.Size = new System.Drawing.Size(349, 156);
			this.propertyMappingGrid.TabIndex = 12;
			this.propertyMappingGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.propertyMappingGrid_CellValueChanged);
			this.propertyMappingGrid.Resize += new System.EventHandler(this.dataGridView_Resize);
			this.propertyMappingGrid.SelectionChanged += new System.EventHandler(this.propertyMappingGrid_SelectionChanged);
			// 
			// modelPropertyColumn
			// 
			this.modelPropertyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.modelPropertyColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
			this.modelPropertyColumn.HeaderText = "Model Property";
			this.modelPropertyColumn.Name = "modelPropertyColumn";
			this.modelPropertyColumn.Sorted = true;
			this.modelPropertyColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.modelPropertyColumn.Width = 173;
			// 
			// shapePropertyColumn
			// 
			this.shapePropertyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.shapePropertyColumn.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
			this.shapePropertyColumn.HeaderText = "Shape Property";
			this.shapePropertyColumn.Items.AddRange(new object[] {
            "Color",
            "StrikeThrough",
            "Value"});
			this.shapePropertyColumn.Name = "shapePropertyColumn";
			this.shapePropertyColumn.Sorted = true;
			this.shapePropertyColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.shapePropertyColumn.Width = 173;
			// 
			// label1
			// 
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(349, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "Property Mapping";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// valueMappingGrid
			// 
			this.valueMappingGrid.AllowUserToAddRows = false;
			this.valueMappingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.valueMappingGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.modelValueColumn,
            this.shapeValueColumn});
			this.valueMappingGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.valueMappingGrid.Location = new System.Drawing.Point(0, 17);
			this.valueMappingGrid.Name = "valueMappingGrid";
			this.valueMappingGrid.RowHeadersVisible = false;
			this.valueMappingGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.valueMappingGrid.Size = new System.Drawing.Size(349, 166);
			this.valueMappingGrid.TabIndex = 13;
			this.valueMappingGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.valueMappingGrid_CellValueChanged);
			this.valueMappingGrid.Resize += new System.EventHandler(this.dataGridView_Resize);
			// 
			// modelValueColumn
			// 
			this.modelValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.modelValueColumn.HeaderText = "Model Value";
			this.modelValueColumn.Name = "modelValueColumn";
			this.modelValueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.modelValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// shapeValueColumn
			// 
			this.shapeValueColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.shapeValueColumn.HeaderText = "Shape Value";
			this.shapeValueColumn.Name = "shapeValueColumn";
			this.shapeValueColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.shapeValueColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// label2
			// 
			this.label2.Dock = System.Windows.Forms.DockStyle.Top;
			this.label2.Location = new System.Drawing.Point(0, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(349, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Value Mapping";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// previewPanel
			// 
			this.previewPanel.BackColor = System.Drawing.SystemColors.Control;
			this.previewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewPanel.Location = new System.Drawing.Point(0, 0);
			this.previewPanel.Name = "previewPanel";
			this.previewPanel.Size = new System.Drawing.Size(362, 396);
			this.previewPanel.TabIndex = 9;
			this.previewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.previewPanel_Paint);
			this.previewPanel.Resize += new System.EventHandler(this.previewPanel_Resize);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 111);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.previewPanel);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tabControl);
			this.splitContainer1.Size = new System.Drawing.Size(731, 396);
			this.splitContainer1.SplitterDistance = 362;
			this.splitContainer1.TabIndex = 0;
			this.splitContainer1.TabStop = false;
			// 
			// propertyController
			// 
			this.propertyController.Project = null;
			this.propertyController.PropertyChanged += new System.EventHandler<Dataweb.NShape.Controllers.PropertyControllerPropertyChangedEventArgs>(this.propertyController_PropertyChanged);
			// 
			// propertyPresenter
			// 
			this.propertyPresenter.PrimaryPropertyGrid = this.shapePropertyGrid;
			this.propertyPresenter.PropertyController = this.propertyController;
			this.propertyPresenter.SecondaryPropertyGrid = this.modelPropertyGrid;
			// 
			// TemplatePresenter
			// 
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.headerSplitContainer);
			this.Name = "TemplatePresenter";
			this.Size = new System.Drawing.Size(731, 507);
			this.headerSplitContainer.Panel1.ResumeLayout(false);
			this.headerSplitContainer.Panel1.PerformLayout();
			this.headerSplitContainer.Panel2.ResumeLayout(false);
			this.headerSplitContainer.Panel2.PerformLayout();
			this.headerSplitContainer.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.shapePropertiesTab.ResumeLayout(false);
			this.modelPropertiesTab.ResumeLayout(false);
			this.controlPointsTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.controlPointMappingGrid)).EndInit();
			this.propertyMappingTab.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.propertyMappingGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.valueMappingGrid)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.Label templateNameLabel;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage shapePropertiesTab;
		private System.Windows.Forms.TabPage controlPointsTab;
		private System.Windows.Forms.TabPage propertyMappingTab;
		private System.Windows.Forms.PropertyGrid shapePropertyGrid;
		private System.Windows.Forms.ComboBox shapeComboBox;
		private System.Windows.Forms.Label templateShapeLabel;
		private System.Windows.Forms.Panel previewPanel;
		private System.Windows.Forms.TabPage modelPropertiesTab;
		private System.Windows.Forms.DataGridView controlPointMappingGrid;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.DataGridView propertyMappingGrid;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.DataGridView valueMappingGrid;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.PropertyGrid modelPropertyGrid;
		private System.Windows.Forms.SplitContainer headerSplitContainer;
		private System.Windows.Forms.ComboBox modelObjectComboBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox descriptionTextBox;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox titleTextBox;
		private System.Windows.Forms.Label label4;
		private PropertyPresenter propertyPresenter;
		private Dataweb.NShape.Controllers.PropertyController propertyController;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.DataGridViewComboBoxColumn modelValueColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn shapeValueColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn modelPropertyColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn shapePropertyColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn ControlPointColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn TerminalColumn;
	}
}
namespace VixenApplication.Setup
{
	partial class SetupElementsTree
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			ListViewItem listViewItem1 = new ListViewItem("1234");
			ListViewItem listViewItem2 = new ListViewItem("qwer qwer qwer asdf zxcv zxcv asdf qwerd qwer ");
			ListViewItem listViewItem3 = new ListViewItem("asdf");
			ListViewItem listViewItem4 = new ListViewItem("zxcv");
			label1 = new Label();
			comboBoxNewItemType = new ComboBox();
			buttonAddTemplate = new Button();
			groupBoxSelectedItems = new GroupBox();
			panel1 = new Panel();
			buttonConfigureProperty = new Button();
			buttonRemoveProperty = new Button();
			buttonAddProperty = new Button();
			label3 = new Label();
			listViewProperties = new ListView();
			columnHeader2 = new ColumnHeader();
			buttonRunHelperSetup = new Button();
			comboBoxSetupHelperType = new ComboBox();
			label2 = new Label();
			toolTip1 = new ToolTip(components);
			buttonSelectDestinationOutputs = new Button();
			buttonDeleteElements = new Button();
			buttonRenameElements = new Button();
			flowLayoutPanel1 = new FlowLayoutPanel();
			flowLayoutPanel2 = new FlowLayoutPanel();
			tableLayoutPanel1 = new TableLayoutPanel();
			elementTree = new Common.Controls.ElementTree();
			groupBoxSelectedItems.SuspendLayout();
			panel1.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			flowLayoutPanel2.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(4, 14);
			label1.Margin = new Padding(4, 14, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(32, 15);
			label1.TabIndex = 29;
			label1.Text = "Add:";
			// 
			// comboBoxNewItemType
			// 
			comboBoxNewItemType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			comboBoxNewItemType.DrawMode = DrawMode.OwnerDrawFixed;
			comboBoxNewItemType.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxNewItemType.FlatStyle = FlatStyle.Flat;
			comboBoxNewItemType.FormattingEnabled = true;
			comboBoxNewItemType.Location = new Point(44, 9);
			comboBoxNewItemType.Margin = new Padding(4, 9, 4, 3);
			comboBoxNewItemType.Name = "comboBoxNewItemType";
			comboBoxNewItemType.Size = new Size(170, 24);
			comboBoxNewItemType.TabIndex = 30;
			comboBoxNewItemType.DrawItem += comboBox_DrawItem;
			comboBoxNewItemType.SelectedIndexChanged += comboBoxNewItemType_SelectedIndexChanged;
			// 
			// buttonAddTemplate
			// 
			buttonAddTemplate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonAddTemplate.BackColor = Color.Transparent;
			buttonAddTemplate.Enabled = false;
			buttonAddTemplate.FlatAppearance.BorderSize = 0;
			buttonAddTemplate.FlatStyle = FlatStyle.Flat;
			buttonAddTemplate.Location = new Point(222, 7);
			buttonAddTemplate.Margin = new Padding(4, 7, 4, 3);
			buttonAddTemplate.Name = "buttonAddTemplate";
			buttonAddTemplate.Size = new Size(28, 28);
			buttonAddTemplate.TabIndex = 31;
			buttonAddTemplate.Text = "+";
			toolTip1.SetToolTip(buttonAddTemplate, "Add Elements");
			buttonAddTemplate.UseVisualStyleBackColor = false;
			buttonAddTemplate.Click += buttonAddTemplate_Click;
			// 
			// groupBoxSelectedItems
			// 
			groupBoxSelectedItems.Controls.Add(panel1);
			groupBoxSelectedItems.Dock = DockStyle.Fill;
			groupBoxSelectedItems.Location = new Point(4, 555);
			groupBoxSelectedItems.Margin = new Padding(4, 3, 4, 3);
			groupBoxSelectedItems.Name = "groupBoxSelectedItems";
			groupBoxSelectedItems.Padding = new Padding(4, 3, 4, 3);
			groupBoxSelectedItems.Size = new Size(290, 183);
			groupBoxSelectedItems.TabIndex = 33;
			groupBoxSelectedItems.TabStop = false;
			groupBoxSelectedItems.Text = "Selected Item(s):";
			groupBoxSelectedItems.Paint += groupBoxes_Paint;
			// 
			// panel1
			// 
			panel1.Controls.Add(buttonConfigureProperty);
			panel1.Controls.Add(buttonRemoveProperty);
			panel1.Controls.Add(buttonAddProperty);
			panel1.Controls.Add(label3);
			panel1.Controls.Add(listViewProperties);
			panel1.Controls.Add(buttonRunHelperSetup);
			panel1.Controls.Add(comboBoxSetupHelperType);
			panel1.Controls.Add(label2);
			panel1.Dock = DockStyle.Fill;
			panel1.Location = new Point(4, 19);
			panel1.Margin = new Padding(4, 3, 4, 3);
			panel1.Name = "panel1";
			panel1.Size = new Size(282, 161);
			panel1.TabIndex = 42;
			// 
			// buttonConfigureProperty
			// 
			buttonConfigureProperty.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonConfigureProperty.BackColor = Color.Transparent;
			buttonConfigureProperty.Enabled = false;
			buttonConfigureProperty.FlatAppearance.BorderSize = 0;
			buttonConfigureProperty.FlatStyle = FlatStyle.Flat;
			buttonConfigureProperty.Location = new Point(154, 126);
			buttonConfigureProperty.Margin = new Padding(4, 3, 4, 3);
			buttonConfigureProperty.Name = "buttonConfigureProperty";
			buttonConfigureProperty.Size = new Size(28, 28);
			buttonConfigureProperty.TabIndex = 41;
			buttonConfigureProperty.Text = "C";
			toolTip1.SetToolTip(buttonConfigureProperty, "Configure Property");
			buttonConfigureProperty.UseVisualStyleBackColor = false;
			buttonConfigureProperty.Click += buttonConfigureProperty_Click;
			// 
			// buttonRemoveProperty
			// 
			buttonRemoveProperty.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonRemoveProperty.BackColor = Color.Transparent;
			buttonRemoveProperty.Enabled = false;
			buttonRemoveProperty.FlatAppearance.BorderSize = 0;
			buttonRemoveProperty.FlatStyle = FlatStyle.Flat;
			buttonRemoveProperty.Location = new Point(119, 126);
			buttonRemoveProperty.Margin = new Padding(4, 3, 4, 3);
			buttonRemoveProperty.Name = "buttonRemoveProperty";
			buttonRemoveProperty.Size = new Size(28, 28);
			buttonRemoveProperty.TabIndex = 40;
			buttonRemoveProperty.Text = "-";
			toolTip1.SetToolTip(buttonRemoveProperty, "Delete Property");
			buttonRemoveProperty.UseVisualStyleBackColor = false;
			buttonRemoveProperty.Click += buttonRemoveProperty_Click;
			// 
			// buttonAddProperty
			// 
			buttonAddProperty.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonAddProperty.BackColor = Color.Transparent;
			buttonAddProperty.Enabled = false;
			buttonAddProperty.FlatAppearance.BorderSize = 0;
			buttonAddProperty.FlatStyle = FlatStyle.Flat;
			buttonAddProperty.Location = new Point(84, 126);
			buttonAddProperty.Margin = new Padding(4, 3, 4, 3);
			buttonAddProperty.Name = "buttonAddProperty";
			buttonAddProperty.Size = new Size(28, 28);
			buttonAddProperty.TabIndex = 39;
			buttonAddProperty.Text = "+";
			toolTip1.SetToolTip(buttonAddProperty, "Add Property");
			buttonAddProperty.UseVisualStyleBackColor = false;
			buttonAddProperty.Click += buttonAddProperty_Click;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(2, 52);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(63, 15);
			label3.TabIndex = 38;
			label3.Text = "Properties:";
			// 
			// listViewProperties
			// 
			listViewProperties.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			listViewProperties.Columns.AddRange(new ColumnHeader[] { columnHeader2 });
			listViewProperties.HeaderStyle = ColumnHeaderStyle.None;
			listViewProperties.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3, listViewItem4 });
			listViewProperties.Location = new Point(74, 52);
			listViewProperties.Margin = new Padding(4, 3, 4, 3);
			listViewProperties.Name = "listViewProperties";
			listViewProperties.Size = new Size(197, 66);
			listViewProperties.TabIndex = 37;
			listViewProperties.UseCompatibleStateImageBehavior = false;
			listViewProperties.View = View.Details;
			listViewProperties.SelectedIndexChanged += listViewProperties_SelectedIndexChanged;
			listViewProperties.MouseDoubleClick += listViewProperties_MouseDoubleClick;
			// 
			// columnHeader2
			// 
			columnHeader2.Width = 90;
			// 
			// buttonRunHelperSetup
			// 
			buttonRunHelperSetup.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonRunHelperSetup.BackColor = Color.Transparent;
			buttonRunHelperSetup.Enabled = false;
			buttonRunHelperSetup.FlatAppearance.BorderSize = 0;
			buttonRunHelperSetup.FlatStyle = FlatStyle.Flat;
			buttonRunHelperSetup.Location = new Point(243, 9);
			buttonRunHelperSetup.Margin = new Padding(4, 3, 4, 3);
			buttonRunHelperSetup.Name = "buttonRunHelperSetup";
			buttonRunHelperSetup.Size = new Size(28, 28);
			buttonRunHelperSetup.TabIndex = 36;
			buttonRunHelperSetup.Text = "->";
			toolTip1.SetToolTip(buttonRunHelperSetup, "Configure");
			buttonRunHelperSetup.UseVisualStyleBackColor = false;
			buttonRunHelperSetup.Click += buttonRunSetupHelper_Click;
			// 
			// comboBoxSetupHelperType
			// 
			comboBoxSetupHelperType.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			comboBoxSetupHelperType.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBoxSetupHelperType.FlatStyle = FlatStyle.Flat;
			comboBoxSetupHelperType.FormattingEnabled = true;
			comboBoxSetupHelperType.Location = new Point(74, 12);
			comboBoxSetupHelperType.Margin = new Padding(4, 3, 4, 3);
			comboBoxSetupHelperType.Name = "comboBoxSetupHelperType";
			comboBoxSetupHelperType.Size = new Size(150, 23);
			comboBoxSetupHelperType.TabIndex = 35;
			comboBoxSetupHelperType.DrawItem += comboBox_DrawItem;
			comboBoxSetupHelperType.SelectedIndexChanged += comboBoxSetupHelperType_SelectedIndexChanged;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(2, 15);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(63, 15);
			label2.TabIndex = 34;
			label2.Text = "Configure:";
			// 
			// toolTip1
			// 
			toolTip1.AutomaticDelay = 200;
			toolTip1.AutoPopDelay = 5000;
			toolTip1.InitialDelay = 200;
			toolTip1.ReshowDelay = 40;
			// 
			// buttonSelectDestinationOutputs
			// 
			buttonSelectDestinationOutputs.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			buttonSelectDestinationOutputs.BackColor = Color.Transparent;
			buttonSelectDestinationOutputs.FlatAppearance.BorderSize = 0;
			buttonSelectDestinationOutputs.FlatStyle = FlatStyle.Flat;
			buttonSelectDestinationOutputs.Location = new Point(77, 3);
			buttonSelectDestinationOutputs.Margin = new Padding(4, 3, 4, 3);
			buttonSelectDestinationOutputs.Name = "buttonSelectDestinationOutputs";
			buttonSelectDestinationOutputs.Size = new Size(28, 28);
			buttonSelectDestinationOutputs.TabIndex = 41;
			buttonSelectDestinationOutputs.Text = "S";
			toolTip1.SetToolTip(buttonSelectDestinationOutputs, "Find outputs these elements are patched to");
			buttonSelectDestinationOutputs.UseVisualStyleBackColor = false;
			buttonSelectDestinationOutputs.Click += buttonSelectDestinationOutputs_Click;
			// 
			// buttonDeleteElements
			// 
			buttonDeleteElements.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			buttonDeleteElements.BackColor = Color.Transparent;
			buttonDeleteElements.FlatAppearance.BorderSize = 0;
			buttonDeleteElements.FlatStyle = FlatStyle.Flat;
			buttonDeleteElements.Location = new Point(4, 3);
			buttonDeleteElements.Margin = new Padding(4, 3, 4, 3);
			buttonDeleteElements.Name = "buttonDeleteElements";
			buttonDeleteElements.Size = new Size(28, 28);
			buttonDeleteElements.TabIndex = 42;
			buttonDeleteElements.Text = "-";
			toolTip1.SetToolTip(buttonDeleteElements, "Delete Elements");
			buttonDeleteElements.UseVisualStyleBackColor = false;
			buttonDeleteElements.Click += buttonDeleteElements_Click;
			// 
			// buttonRenameElements
			// 
			buttonRenameElements.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			buttonRenameElements.BackColor = Color.Transparent;
			buttonRenameElements.FlatAppearance.BorderSize = 0;
			buttonRenameElements.FlatStyle = FlatStyle.Flat;
			buttonRenameElements.Location = new Point(40, 3);
			buttonRenameElements.Margin = new Padding(4, 3, 4, 3);
			buttonRenameElements.Name = "buttonRenameElements";
			buttonRenameElements.Size = new Size(29, 28);
			buttonRenameElements.TabIndex = 43;
			buttonRenameElements.Text = "R";
			toolTip1.SetToolTip(buttonRenameElements, "Rename Elements");
			buttonRenameElements.UseVisualStyleBackColor = false;
			buttonRenameElements.Click += buttonRenameElements_Click;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			flowLayoutPanel1.Controls.Add(label1);
			flowLayoutPanel1.Controls.Add(comboBoxNewItemType);
			flowLayoutPanel1.Controls.Add(buttonAddTemplate);
			flowLayoutPanel1.Dock = DockStyle.Fill;
			flowLayoutPanel1.Location = new Point(4, 3);
			flowLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(290, 43);
			flowLayoutPanel1.TabIndex = 44;
			flowLayoutPanel1.WrapContents = false;
			// 
			// flowLayoutPanel2
			// 
			flowLayoutPanel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			flowLayoutPanel2.Controls.Add(buttonDeleteElements);
			flowLayoutPanel2.Controls.Add(buttonRenameElements);
			flowLayoutPanel2.Controls.Add(buttonSelectDestinationOutputs);
			flowLayoutPanel2.Dock = DockStyle.Fill;
			flowLayoutPanel2.Location = new Point(4, 519);
			flowLayoutPanel2.Margin = new Padding(4, 3, 4, 3);
			flowLayoutPanel2.Name = "flowLayoutPanel2";
			flowLayoutPanel2.Size = new Size(290, 30);
			flowLayoutPanel2.TabIndex = 45;
			flowLayoutPanel2.WrapContents = false;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.ColumnCount = 1;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 0);
			tableLayoutPanel1.Controls.Add(groupBoxSelectedItems, 0, 3);
			tableLayoutPanel1.Controls.Add(elementTree, 0, 1);
			tableLayoutPanel1.Controls.Add(flowLayoutPanel2, 0, 2);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 0);
			tableLayoutPanel1.Margin = new Padding(4, 3, 4, 3);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 4;
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.RowStyles.Add(new RowStyle());
			tableLayoutPanel1.Size = new Size(298, 741);
			tableLayoutPanel1.TabIndex = 46;
			// 
			// elementTree
			// 
			elementTree.AllowDragging = true;
			elementTree.AllowPropertyEdit = true;
			elementTree.AllowWireExport = true;
			elementTree.AutoScroll = true;
			elementTree.AutoSize = true;
			elementTree.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			elementTree.Dock = DockStyle.Fill;
			elementTree.ExportDiagram = null;
			elementTree.Location = new Point(5, 52);
			elementTree.Margin = new Padding(5, 3, 5, 3);
			elementTree.Name = "elementTree";
			elementTree.Size = new Size(288, 461);
			elementTree.TabIndex = 28;
			elementTree.treeviewDeselected += elementTree_treeviewDeselected;
			elementTree.treeviewAfterSelect += elementTree_treeviewAfterSelect;
			elementTree.ElementsChanged += elementTree_ElementsChanged;
			// 
			// SetupElementsTree
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			Controls.Add(tableLayoutPanel1);
			DoubleBuffered = true;
			Margin = new Padding(4, 3, 4, 3);
			Name = "SetupElementsTree";
			Size = new Size(298, 741);
			groupBoxSelectedItems.ResumeLayout(false);
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			flowLayoutPanel2.ResumeLayout(false);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Common.Controls.ElementTree elementTree;
		private Label label1;
		private ComboBox comboBoxNewItemType;
		private Button buttonAddTemplate;
		private GroupBox groupBoxSelectedItems;
		private Button buttonRunHelperSetup;
		private ComboBox comboBoxSetupHelperType;
		private Label label2;
		private ListView listViewProperties;
		private ColumnHeader columnHeader2;
		private Label label3;
		private Button buttonAddProperty;
		private ToolTip toolTip1;
		private Button buttonRemoveProperty;
		private Button buttonConfigureProperty;
		private Button buttonSelectDestinationOutputs;
		private Button buttonDeleteElements;
		private Button buttonRenameElements;
		private Panel panel1;
		private FlowLayoutPanel flowLayoutPanel1;
		private FlowLayoutPanel flowLayoutPanel2;
		private TableLayoutPanel tableLayoutPanel1;
	}
}

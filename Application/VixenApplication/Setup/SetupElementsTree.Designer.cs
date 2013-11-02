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
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ListViewItem listViewItem17 = new System.Windows.Forms.ListViewItem("1234");
			System.Windows.Forms.ListViewItem listViewItem18 = new System.Windows.Forms.ListViewItem("qwer qwer qwer asdf zxcv zxcv asdf qwerd qwer ");
			System.Windows.Forms.ListViewItem listViewItem19 = new System.Windows.Forms.ListViewItem("asdf");
			System.Windows.Forms.ListViewItem listViewItem20 = new System.Windows.Forms.ListViewItem("zxcv");
			this.label1 = new System.Windows.Forms.Label();
			this.comboBoxNewItemType = new System.Windows.Forms.ComboBox();
			this.buttonAddTemplate = new System.Windows.Forms.Button();
			this.groupBoxSelectedItems = new System.Windows.Forms.GroupBox();
			this.buttonAddProperty = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.listViewProperties = new System.Windows.Forms.ListView();
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.buttonRunHelperSetup = new System.Windows.Forms.Button();
			this.comboBoxSetupHelperType = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.elementTree = new Common.Controls.ElementTree();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.buttonRemoveProperty = new System.Windows.Forms.Button();
			this.buttonConfigureProperty = new System.Windows.Forms.Button();
			this.groupBoxSelectedItems.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 29;
			this.label1.Text = "Add:";
			// 
			// comboBoxNewItemType
			// 
			this.comboBoxNewItemType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxNewItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxNewItemType.FormattingEnabled = true;
			this.comboBoxNewItemType.Location = new System.Drawing.Point(47, 12);
			this.comboBoxNewItemType.Name = "comboBoxNewItemType";
			this.comboBoxNewItemType.Size = new System.Drawing.Size(146, 21);
			this.comboBoxNewItemType.TabIndex = 30;
			this.comboBoxNewItemType.SelectedIndexChanged += new System.EventHandler(this.comboBoxNewItemType_SelectedIndexChanged);
			// 
			// buttonAddTemplate
			// 
			this.buttonAddTemplate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddTemplate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonAddTemplate.Enabled = false;
			this.buttonAddTemplate.Location = new System.Drawing.Point(209, 10);
			this.buttonAddTemplate.Name = "buttonAddTemplate";
			this.buttonAddTemplate.Size = new System.Drawing.Size(24, 24);
			this.buttonAddTemplate.TabIndex = 31;
			this.buttonAddTemplate.Text = "+";
			this.toolTip1.SetToolTip(this.buttonAddTemplate, "Add Elements");
			this.buttonAddTemplate.UseVisualStyleBackColor = true;
			this.buttonAddTemplate.Click += new System.EventHandler(this.buttonAddTemplate_Click);
			// 
			// groupBoxSelectedItems
			// 
			this.groupBoxSelectedItems.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxSelectedItems.Controls.Add(this.buttonConfigureProperty);
			this.groupBoxSelectedItems.Controls.Add(this.buttonRemoveProperty);
			this.groupBoxSelectedItems.Controls.Add(this.buttonAddProperty);
			this.groupBoxSelectedItems.Controls.Add(this.label3);
			this.groupBoxSelectedItems.Controls.Add(this.listViewProperties);
			this.groupBoxSelectedItems.Controls.Add(this.buttonRunHelperSetup);
			this.groupBoxSelectedItems.Controls.Add(this.comboBoxSetupHelperType);
			this.groupBoxSelectedItems.Controls.Add(this.label2);
			this.groupBoxSelectedItems.Location = new System.Drawing.Point(3, 387);
			this.groupBoxSelectedItems.Name = "groupBoxSelectedItems";
			this.groupBoxSelectedItems.Size = new System.Drawing.Size(244, 159);
			this.groupBoxSelectedItems.TabIndex = 33;
			this.groupBoxSelectedItems.TabStop = false;
			this.groupBoxSelectedItems.Text = "Selected Item(s):";
			// 
			// buttonAddProperty
			// 
			this.buttonAddProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAddProperty.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonAddProperty.Enabled = false;
			this.buttonAddProperty.Location = new System.Drawing.Point(70, 125);
			this.buttonAddProperty.Name = "buttonAddProperty";
			this.buttonAddProperty.Size = new System.Drawing.Size(24, 24);
			this.buttonAddProperty.TabIndex = 39;
			this.buttonAddProperty.Text = "+";
			this.toolTip1.SetToolTip(this.buttonAddProperty, "Add Property");
			this.buttonAddProperty.UseVisualStyleBackColor = true;
			this.buttonAddProperty.Click += new System.EventHandler(this.buttonAddProperty_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(9, 61);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(57, 13);
			this.label3.TabIndex = 38;
			this.label3.Text = "Properties:";
			// 
			// listViewProperties
			// 
			this.listViewProperties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
			this.listViewProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.listViewProperties.HideSelection = false;
			this.listViewProperties.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem17,
            listViewItem18,
            listViewItem19,
            listViewItem20});
			this.listViewProperties.Location = new System.Drawing.Point(70, 61);
			this.listViewProperties.Name = "listViewProperties";
			this.listViewProperties.Size = new System.Drawing.Size(160, 58);
			this.listViewProperties.TabIndex = 37;
			this.listViewProperties.UseCompatibleStateImageBehavior = false;
			this.listViewProperties.View = System.Windows.Forms.View.Details;
			this.listViewProperties.SelectedIndexChanged += new System.EventHandler(this.listViewProperties_SelectedIndexChanged);
			this.listViewProperties.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewProperties_MouseDoubleClick);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Width = 90;
			// 
			// buttonRunHelperSetup
			// 
			this.buttonRunHelperSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRunHelperSetup.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonRunHelperSetup.Enabled = false;
			this.buttonRunHelperSetup.Location = new System.Drawing.Point(206, 24);
			this.buttonRunHelperSetup.Name = "buttonRunHelperSetup";
			this.buttonRunHelperSetup.Size = new System.Drawing.Size(24, 24);
			this.buttonRunHelperSetup.TabIndex = 36;
			this.buttonRunHelperSetup.Text = "->";
			this.toolTip1.SetToolTip(this.buttonRunHelperSetup, "Configure");
			this.buttonRunHelperSetup.UseVisualStyleBackColor = true;
			this.buttonRunHelperSetup.Click += new System.EventHandler(this.buttonRunSetupHelper_Click);
			// 
			// comboBoxSetupHelperType
			// 
			this.comboBoxSetupHelperType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBoxSetupHelperType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxSetupHelperType.FormattingEnabled = true;
			this.comboBoxSetupHelperType.Location = new System.Drawing.Point(70, 26);
			this.comboBoxSetupHelperType.Name = "comboBoxSetupHelperType";
			this.comboBoxSetupHelperType.Size = new System.Drawing.Size(120, 21);
			this.comboBoxSetupHelperType.TabIndex = 35;
			this.comboBoxSetupHelperType.SelectedIndexChanged += new System.EventHandler(this.comboBoxSetupHelperType_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 13);
			this.label2.TabIndex = 34;
			this.label2.Text = "Configure:";
			// 
			// elementTree
			// 
			this.elementTree.AllowDragging = true;
			this.elementTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.elementTree.Location = new System.Drawing.Point(3, 46);
			this.elementTree.Name = "elementTree";
			this.elementTree.Size = new System.Drawing.Size(244, 335);
			this.elementTree.TabIndex = 28;
			this.elementTree.treeviewDeselected += new System.EventHandler(this.elementTree_treeviewDeselected);
			this.elementTree.treeviewAfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.elementTree_treeviewAfterSelect);
			this.elementTree.ElementsChanged += new System.EventHandler(this.elementTree_ElementsChanged);
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 200;
			this.toolTip1.AutoPopDelay = 5000;
			this.toolTip1.InitialDelay = 200;
			this.toolTip1.ReshowDelay = 40;
			// 
			// buttonRemoveProperty
			// 
			this.buttonRemoveProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemoveProperty.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonRemoveProperty.Enabled = false;
			this.buttonRemoveProperty.Location = new System.Drawing.Point(100, 125);
			this.buttonRemoveProperty.Name = "buttonRemoveProperty";
			this.buttonRemoveProperty.Size = new System.Drawing.Size(24, 24);
			this.buttonRemoveProperty.TabIndex = 40;
			this.buttonRemoveProperty.Text = "-";
			this.toolTip1.SetToolTip(this.buttonRemoveProperty, "Delete Property");
			this.buttonRemoveProperty.UseVisualStyleBackColor = true;
			this.buttonRemoveProperty.Click += new System.EventHandler(this.buttonRemoveProperty_Click);
			// 
			// buttonConfigureProperty
			// 
			this.buttonConfigureProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonConfigureProperty.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.buttonConfigureProperty.Enabled = false;
			this.buttonConfigureProperty.Location = new System.Drawing.Point(130, 125);
			this.buttonConfigureProperty.Name = "buttonConfigureProperty";
			this.buttonConfigureProperty.Size = new System.Drawing.Size(24, 24);
			this.buttonConfigureProperty.TabIndex = 41;
			this.buttonConfigureProperty.Text = "C";
			this.toolTip1.SetToolTip(this.buttonConfigureProperty, "Configure Property");
			this.buttonConfigureProperty.UseVisualStyleBackColor = true;
			this.buttonConfigureProperty.Click += new System.EventHandler(this.buttonConfigureProperty_Click);
			// 
			// SetupElementsTree
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.groupBoxSelectedItems);
			this.Controls.Add(this.buttonAddTemplate);
			this.Controls.Add(this.comboBoxNewItemType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.elementTree);
			this.DoubleBuffered = true;
			this.Name = "SetupElementsTree";
			this.Size = new System.Drawing.Size(250, 550);
			this.groupBoxSelectedItems.ResumeLayout(false);
			this.groupBoxSelectedItems.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Common.Controls.ElementTree elementTree;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBoxNewItemType;
		private System.Windows.Forms.Button buttonAddTemplate;
		private System.Windows.Forms.GroupBox groupBoxSelectedItems;
		private System.Windows.Forms.Button buttonRunHelperSetup;
		private System.Windows.Forms.ComboBox comboBoxSetupHelperType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ListView listViewProperties;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button buttonAddProperty;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button buttonRemoveProperty;
		private System.Windows.Forms.Button buttonConfigureProperty;
	}
}

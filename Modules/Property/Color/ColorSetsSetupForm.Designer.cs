namespace VixenModules.Property.Color
{
	partial class ColorSetsSetupForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("lkjhlkjh");
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("kvgvhgv");
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("uytruyt");
			this.listViewColorSets = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxColorSet = new System.Windows.Forms.GroupBox();
			this.buttonAddColor = new System.Windows.Forms.Button();
			this.buttonUpdate = new System.Windows.Forms.Button();
			this.tableLayoutPanelColors = new System.Windows.Forms.TableLayoutPanel();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.buttonRemoveColorSet = new System.Windows.Forms.Button();
			this.buttonAddColorSet = new System.Windows.Forms.Button();
			this.groupBoxColorSet.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewColorSets
			// 
			this.listViewColorSets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.listViewColorSets.FullRowSelect = true;
			this.listViewColorSets.GridLines = true;
			this.listViewColorSets.HideSelection = false;
			this.listViewColorSets.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3});
			this.listViewColorSets.Location = new System.Drawing.Point(14, 30);
			this.listViewColorSets.MultiSelect = false;
			this.listViewColorSets.Name = "listViewColorSets";
			this.listViewColorSets.Size = new System.Drawing.Size(98, 95);
			this.listViewColorSets.TabIndex = 0;
			this.listViewColorSets.UseCompatibleStateImageBehavior = false;
			this.listViewColorSets.View = System.Windows.Forms.View.List;
			this.listViewColorSets.SelectedIndexChanged += new System.EventHandler(this.listViewColorSets_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Defined Color Sets:";
			// 
			// groupBoxColorSet
			// 
			this.groupBoxColorSet.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxColorSet.Controls.Add(this.buttonAddColor);
			this.groupBoxColorSet.Controls.Add(this.buttonUpdate);
			this.groupBoxColorSet.Controls.Add(this.tableLayoutPanelColors);
			this.groupBoxColorSet.Controls.Add(this.label3);
			this.groupBoxColorSet.Controls.Add(this.label2);
			this.groupBoxColorSet.Controls.Add(this.textBoxName);
			this.groupBoxColorSet.Location = new System.Drawing.Point(129, 12);
			this.groupBoxColorSet.Name = "groupBoxColorSet";
			this.groupBoxColorSet.Size = new System.Drawing.Size(302, 199);
			this.groupBoxColorSet.TabIndex = 4;
			this.groupBoxColorSet.TabStop = false;
			this.groupBoxColorSet.Text = "Selected Color Set";
			// 
			// buttonAddColor
			// 
			this.buttonAddColor.Image = global::VixenModules.Property.Color.Properties.Resources.add_24;
			this.buttonAddColor.Location = new System.Drawing.Point(250, 65);
			this.buttonAddColor.Name = "buttonAddColor";
			this.buttonAddColor.Size = new System.Drawing.Size(32, 32);
			this.buttonAddColor.TabIndex = 12;
			this.buttonAddColor.UseVisualStyleBackColor = true;
			this.buttonAddColor.Click += new System.EventHandler(this.buttonAddColor_Click);
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Location = new System.Drawing.Point(73, 155);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(120, 25);
			this.buttonUpdate.TabIndex = 11;
			this.buttonUpdate.Text = "Make New Color Set";
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			// 
			// tableLayoutPanelColors
			// 
			this.tableLayoutPanelColors.ColumnCount = 4;
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.Location = new System.Drawing.Point(73, 61);
			this.tableLayoutPanelColors.Name = "tableLayoutPanelColors";
			this.tableLayoutPanelColors.RowCount = 2;
			this.tableLayoutPanelColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelColors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelColors.Size = new System.Drawing.Size(160, 80);
			this.tableLayoutPanelColors.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(19, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 13);
			this.label3.TabIndex = 3;
			this.label3.Text = "Colors:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(19, 29);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "Name:";
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(73, 26);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(130, 20);
			this.textBoxName.TabIndex = 0;
			this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
			// 
			// button1
			// 
			this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.button1.Location = new System.Drawing.Point(341, 223);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(90, 25);
			this.button1.TabIndex = 10;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// buttonRemoveColorSet
			// 
			this.buttonRemoveColorSet.Image = global::VixenModules.Property.Color.Properties.Resources.delete_24;
			this.buttonRemoveColorSet.Location = new System.Drawing.Point(70, 134);
			this.buttonRemoveColorSet.Name = "buttonRemoveColorSet";
			this.buttonRemoveColorSet.Size = new System.Drawing.Size(32, 32);
			this.buttonRemoveColorSet.TabIndex = 3;
			this.buttonRemoveColorSet.UseVisualStyleBackColor = true;
			this.buttonRemoveColorSet.Click += new System.EventHandler(this.buttonRemoveColorSet_Click);
			// 
			// buttonAddColorSet
			// 
			this.buttonAddColorSet.Image = global::VixenModules.Property.Color.Properties.Resources.add_24;
			this.buttonAddColorSet.Location = new System.Drawing.Point(24, 134);
			this.buttonAddColorSet.Name = "buttonAddColorSet";
			this.buttonAddColorSet.Size = new System.Drawing.Size(32, 32);
			this.buttonAddColorSet.TabIndex = 2;
			this.buttonAddColorSet.UseVisualStyleBackColor = true;
			this.buttonAddColorSet.Click += new System.EventHandler(this.buttonAddColorSet_Click);
			// 
			// ColorSetsSetupForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(443, 260);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBoxColorSet);
			this.Controls.Add(this.buttonRemoveColorSet);
			this.Controls.Add(this.buttonAddColorSet);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listViewColorSets);
			this.DoubleBuffered = true;
			this.Name = "ColorSetsSetupForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Sets";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorSetsSetupForm_FormClosing);
			this.Load += new System.EventHandler(this.ColorSetsSetupForm_Load);
			this.groupBoxColorSet.ResumeLayout(false);
			this.groupBoxColorSet.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView listViewColorSets;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonAddColorSet;
		private System.Windows.Forms.Button buttonRemoveColorSet;
		private System.Windows.Forms.GroupBox groupBoxColorSet;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelColors;
		private System.Windows.Forms.Button buttonUpdate;
		private System.Windows.Forms.Button buttonAddColor;
		private System.Windows.Forms.ColumnHeader columnHeader1;
	}
}
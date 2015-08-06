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
			this.panelColorSet = new System.Windows.Forms.Panel();
			this.groupBoxColorSet.SuspendLayout();
			this.panelColorSet.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewColorSets
			// 
			this.listViewColorSets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
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
			this.groupBoxColorSet.Controls.Add(this.panelColorSet);
			this.groupBoxColorSet.Controls.Add(this.label3);
			this.groupBoxColorSet.Controls.Add(this.label2);
			this.groupBoxColorSet.Location = new System.Drawing.Point(129, 12);
			this.groupBoxColorSet.Name = "groupBoxColorSet";
			this.groupBoxColorSet.Size = new System.Drawing.Size(302, 199);
			this.groupBoxColorSet.TabIndex = 4;
			this.groupBoxColorSet.TabStop = false;
			this.groupBoxColorSet.Text = "Selected Color Set";
			this.groupBoxColorSet.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// buttonAddColor
			// 
			this.buttonAddColor.BackColor = System.Drawing.Color.Transparent;
			this.buttonAddColor.FlatAppearance.BorderSize = 0;
			this.buttonAddColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddColor.Location = new System.Drawing.Point(193, 47);
			this.buttonAddColor.Name = "buttonAddColor";
			this.buttonAddColor.Size = new System.Drawing.Size(28, 28);
			this.buttonAddColor.TabIndex = 12;
			this.buttonAddColor.Text = "+";
			this.buttonAddColor.UseVisualStyleBackColor = false;
			this.buttonAddColor.Click += new System.EventHandler(this.buttonAddColor_Click);
			// 
			// buttonUpdate
			// 
			this.buttonUpdate.Location = new System.Drawing.Point(16, 137);
			this.buttonUpdate.Name = "buttonUpdate";
			this.buttonUpdate.Size = new System.Drawing.Size(120, 25);
			this.buttonUpdate.TabIndex = 11;
			this.buttonUpdate.Text = "Make New Color Set";
			this.buttonUpdate.UseVisualStyleBackColor = true;
			this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
			this.buttonUpdate.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonUpdate.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// tableLayoutPanelColors
			// 
			this.tableLayoutPanelColors.ColumnCount = 4;
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this.tableLayoutPanelColors.Location = new System.Drawing.Point(16, 43);
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
			this.textBoxName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textBoxName.Location = new System.Drawing.Point(16, 8);
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
			this.button1.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.button1.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonRemoveColorSet
			// 
			this.buttonRemoveColorSet.BackColor = System.Drawing.Color.Transparent;
			this.buttonRemoveColorSet.FlatAppearance.BorderSize = 0;
			this.buttonRemoveColorSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonRemoveColorSet.Location = new System.Drawing.Point(70, 134);
			this.buttonRemoveColorSet.Name = "buttonRemoveColorSet";
			this.buttonRemoveColorSet.Size = new System.Drawing.Size(28, 28);
			this.buttonRemoveColorSet.TabIndex = 3;
			this.buttonRemoveColorSet.Text = "-";
			this.buttonRemoveColorSet.UseVisualStyleBackColor = false;
			this.buttonRemoveColorSet.Click += new System.EventHandler(this.buttonRemoveColorSet_Click);
			// 
			// buttonAddColorSet
			// 
			this.buttonAddColorSet.BackColor = System.Drawing.Color.Transparent;
			this.buttonAddColorSet.FlatAppearance.BorderSize = 0;
			this.buttonAddColorSet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddColorSet.Location = new System.Drawing.Point(24, 134);
			this.buttonAddColorSet.Name = "buttonAddColorSet";
			this.buttonAddColorSet.Size = new System.Drawing.Size(28, 28);
			this.buttonAddColorSet.TabIndex = 2;
			this.buttonAddColorSet.Text = "+";
			this.buttonAddColorSet.UseVisualStyleBackColor = false;
			this.buttonAddColorSet.Click += new System.EventHandler(this.buttonAddColorSet_Click);
			// 
			// panelColorSet
			// 
			this.panelColorSet.Controls.Add(this.buttonAddColor);
			this.panelColorSet.Controls.Add(this.buttonUpdate);
			this.panelColorSet.Controls.Add(this.tableLayoutPanelColors);
			this.panelColorSet.Controls.Add(this.textBoxName);
			this.panelColorSet.Location = new System.Drawing.Point(57, 18);
			this.panelColorSet.Name = "panelColorSet";
			this.panelColorSet.Size = new System.Drawing.Size(238, 172);
			this.panelColorSet.TabIndex = 13;
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
			this.MaximumSize = new System.Drawing.Size(459, 299);
			this.MinimumSize = new System.Drawing.Size(459, 299);
			this.Name = "ColorSetsSetupForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Sets";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorSetsSetupForm_FormClosing);
			this.Load += new System.EventHandler(this.ColorSetsSetupForm_Load);
			this.groupBoxColorSet.ResumeLayout(false);
			this.groupBoxColorSet.PerformLayout();
			this.panelColorSet.ResumeLayout(false);
			this.panelColorSet.PerformLayout();
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
		private System.Windows.Forms.Panel panelColorSet;
	}
}
namespace Common.Controls.NameGeneration
{
	partial class SubstitutionRenamer
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.listViewPatterns = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel2 = new System.Windows.Forms.Panel();
			this.buttonMovePatternDown = new System.Windows.Forms.Button();
			this.buttonMovePatternUp = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonAddNewPattern = new System.Windows.Forms.Button();
			this.buttonDeletePattern = new System.Windows.Forms.Button();
			this.lblReplace = new System.Windows.Forms.Label();
			this.txtReplace = new System.Windows.Forms.TextBox();
			this.lblFind = new System.Windows.Forms.Label();
			this.txtFind = new System.Windows.Forms.TextBox();
			this.lblAddRemove = new System.Windows.Forms.Label();
			this.listViewNames = new Common.Controls.ListViewEx();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.tableLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.listViewNames, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(708, 508);
			this.tableLayoutPanel1.TabIndex = 3;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this.listViewPatterns, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.panel2, 2, 1);
			this.tableLayoutPanel3.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel3.Controls.Add(this.lblReplace, 0, 3);
			this.tableLayoutPanel3.Controls.Add(this.txtReplace, 1, 3);
			this.tableLayoutPanel3.Controls.Add(this.lblFind, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this.txtFind, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this.lblAddRemove, 0, 0);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 5;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(348, 460);
			this.tableLayoutPanel3.TabIndex = 4;
			// 
			// listViewPatterns
			// 
			this.listViewPatterns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewPatterns.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3});
			this.tableLayoutPanel3.SetColumnSpan(this.listViewPatterns, 2);
			this.listViewPatterns.HideSelection = false;
			this.listViewPatterns.Location = new System.Drawing.Point(3, 53);
			this.listViewPatterns.MultiSelect = false;
			this.listViewPatterns.Name = "listViewPatterns";
			this.listViewPatterns.OwnerDraw = true;
			this.listViewPatterns.Size = new System.Drawing.Size(291, 205);
			this.listViewPatterns.TabIndex = 36;
			this.listViewPatterns.TabStop = false;
			this.listViewPatterns.UseCompatibleStateImageBehavior = false;
			this.listViewPatterns.View = System.Windows.Forms.View.List;
			this.listViewPatterns.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.listViewPatterns_DrawItem);
			this.listViewPatterns.SelectedIndexChanged += new System.EventHandler(this.listViewPatterns_SelectedIndexChanged);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Width = 190;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.buttonMovePatternDown);
			this.panel2.Controls.Add(this.buttonMovePatternUp);
			this.panel2.Location = new System.Drawing.Point(300, 53);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(45, 140);
			this.panel2.TabIndex = 35;
			// 
			// buttonMovePatternDown
			// 
			this.buttonMovePatternDown.BackColor = System.Drawing.Color.Transparent;
			this.buttonMovePatternDown.FlatAppearance.BorderSize = 0;
			this.buttonMovePatternDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonMovePatternDown.Location = new System.Drawing.Point(1, 53);
			this.buttonMovePatternDown.Name = "buttonMovePatternDown";
			this.buttonMovePatternDown.Size = new System.Drawing.Size(35, 29);
			this.buttonMovePatternDown.TabIndex = 7;
			this.buttonMovePatternDown.TabStop = false;
			this.buttonMovePatternDown.Text = "D";
			this.buttonMovePatternDown.UseVisualStyleBackColor = false;
			this.buttonMovePatternDown.Click += new System.EventHandler(this.buttonMovePatternDown_Click);
			// 
			// buttonMovePatternUp
			// 
			this.buttonMovePatternUp.BackColor = System.Drawing.Color.Transparent;
			this.buttonMovePatternUp.FlatAppearance.BorderSize = 0;
			this.buttonMovePatternUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonMovePatternUp.Location = new System.Drawing.Point(1, 18);
			this.buttonMovePatternUp.Name = "buttonMovePatternUp";
			this.buttonMovePatternUp.Size = new System.Drawing.Size(35, 29);
			this.buttonMovePatternUp.TabIndex = 6;
			this.buttonMovePatternUp.TabStop = false;
			this.buttonMovePatternUp.Text = "U";
			this.buttonMovePatternUp.UseVisualStyleBackColor = false;
			this.buttonMovePatternUp.Click += new System.EventHandler(this.buttonMovePatternUp_Click);
			// 
			// panel1
			// 
			this.tableLayoutPanel3.SetColumnSpan(this.panel1, 2);
			this.panel1.Controls.Add(this.buttonAddNewPattern);
			this.panel1.Controls.Add(this.buttonDeletePattern);
			this.panel1.Location = new System.Drawing.Point(88, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(161, 44);
			this.panel1.TabIndex = 34;
			// 
			// buttonAddNewPattern
			// 
			this.buttonAddNewPattern.BackColor = System.Drawing.Color.Transparent;
			this.buttonAddNewPattern.FlatAppearance.BorderSize = 0;
			this.buttonAddNewPattern.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAddNewPattern.Location = new System.Drawing.Point(17, 6);
			this.buttonAddNewPattern.Name = "buttonAddNewPattern";
			this.buttonAddNewPattern.Size = new System.Drawing.Size(28, 28);
			this.buttonAddNewPattern.TabIndex = 3;
			this.buttonAddNewPattern.TabStop = false;
			this.buttonAddNewPattern.Text = "+";
			this.buttonAddNewPattern.UseVisualStyleBackColor = false;
			this.buttonAddNewPattern.Click += new System.EventHandler(this.buttonAddNewRule_Click);
			// 
			// buttonDeletePattern
			// 
			this.buttonDeletePattern.BackColor = System.Drawing.Color.Transparent;
			this.buttonDeletePattern.FlatAppearance.BorderSize = 0;
			this.buttonDeletePattern.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDeletePattern.Location = new System.Drawing.Point(58, 6);
			this.buttonDeletePattern.Name = "buttonDeletePattern";
			this.buttonDeletePattern.Size = new System.Drawing.Size(28, 28);
			this.buttonDeletePattern.TabIndex = 4;
			this.buttonDeletePattern.TabStop = false;
			this.buttonDeletePattern.Text = "-";
			this.buttonDeletePattern.UseVisualStyleBackColor = false;
			this.buttonDeletePattern.Click += new System.EventHandler(this.buttonDeleteRule_Click);
			// 
			// lblReplace
			// 
			this.lblReplace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.lblReplace.AutoSize = true;
			this.lblReplace.Location = new System.Drawing.Point(3, 297);
			this.lblReplace.Name = "lblReplace";
			this.lblReplace.Size = new System.Drawing.Size(79, 15);
			this.lblReplace.TabIndex = 4;
			this.lblReplace.Text = "Replace With:";
			// 
			// txtReplace
			// 
			this.txtReplace.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtReplace.Location = new System.Drawing.Point(88, 293);
			this.txtReplace.Name = "txtReplace";
			this.txtReplace.Size = new System.Drawing.Size(206, 23);
			this.txtReplace.TabIndex = 1;
			this.txtReplace.TextChanged += new System.EventHandler(this.txtReplace_TextChanged);
			// 
			// lblFind
			// 
			this.lblFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.lblFind.AutoSize = true;
			this.lblFind.Location = new System.Drawing.Point(3, 268);
			this.lblFind.Name = "lblFind";
			this.lblFind.Size = new System.Drawing.Size(79, 15);
			this.lblFind.TabIndex = 2;
			this.lblFind.Text = "Find What:";
			// 
			// txtFind
			// 
			this.txtFind.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtFind.Location = new System.Drawing.Point(88, 264);
			this.txtFind.Name = "txtFind";
			this.txtFind.Size = new System.Drawing.Size(206, 23);
			this.txtFind.TabIndex = 0;
			this.txtFind.TextChanged += new System.EventHandler(this.txtFind_TextChanged);
			// 
			// lblAddRemove
			// 
			this.lblAddRemove.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblAddRemove.AutoSize = true;
			this.lblAddRemove.Location = new System.Drawing.Point(3, 17);
			this.lblAddRemove.Name = "lblAddRemove";
			this.lblAddRemove.Size = new System.Drawing.Size(50, 15);
			this.lblAddRemove.TabIndex = 37;
			this.lblAddRemove.Text = "Patterns";
			// 
			// listViewNames
			// 
			this.listViewNames.AllowDrop = true;
			this.listViewNames.AllowRowReorder = true;
			this.listViewNames.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewNames.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listViewNames.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewNames.Location = new System.Drawing.Point(357, 3);
			this.listViewNames.Name = "listViewNames";
			this.listViewNames.OwnerDraw = true;
			this.listViewNames.Size = new System.Drawing.Size(348, 460);
			this.listViewNames.TabIndex = 5;
			this.listViewNames.UseCompatibleStateImageBehavior = false;
			this.listViewNames.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Old Name";
			this.columnHeader1.Width = 120;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "New Name";
			this.columnHeader2.Width = 224;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.buttonCancel, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.buttonOk, 0, 0);
			this.tableLayoutPanel2.Location = new System.Drawing.Point(357, 469);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 1;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(348, 36);
			this.tableLayoutPanel2.TabIndex = 3;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(240, 4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(105, 29);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(129, 4);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(105, 29);
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// SubstitutionRenamer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(708, 508);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "SubstitutionRenamer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Find / Replace Name";
			this.Load += new System.EventHandler(this.SubstitutionRenamer_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.panel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.TextBox txtFind;
		private System.Windows.Forms.TextBox txtReplace;
		private System.Windows.Forms.Label lblFind;
		private System.Windows.Forms.Label lblReplace;
		private ListViewEx listViewNames;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonAddNewPattern;
		private System.Windows.Forms.Button buttonDeletePattern;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button buttonMovePatternDown;
		private System.Windows.Forms.Button buttonMovePatternUp;
		private System.Windows.Forms.ListView listViewPatterns;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Label lblAddRemove;
	}
}
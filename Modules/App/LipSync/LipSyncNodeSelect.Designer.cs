namespace VixenModules.App.LipSyncApp
{
    partial class LipSyncNodeSelect
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LipSyncNodeSelect));
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.chosenTargets = new System.Windows.Forms.ListBox();
			this.buttonAdd = new System.Windows.Forms.Button();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonReset = new System.Windows.Forms.Button();
			this.recurseCB = new System.Windows.Forms.CheckBox();
			this.allowGroupsCheckbox = new System.Windows.Forms.CheckBox();
			this.stringsGroupBox = new System.Windows.Forms.GroupBox();
			this.colsRadioButton = new System.Windows.Forms.RadioButton();
			this.rowsRadioButton = new System.Windows.Forms.RadioButton();
			this.nodeTreeView = new Common.Controls.MultiSelectTreeview();
			this.panel1 = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.stringsGroupBox.SuspendLayout();
			this.panel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonOk.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOk.Location = new System.Drawing.Point(419, 3);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(87, 27);
			this.buttonOk.TabIndex = 0;
			this.buttonOk.Text = "OK";
			this.buttonOk.UseVisualStyleBackColor = true;
			this.buttonOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.Location = new System.Drawing.Point(512, 3);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(87, 27);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// chosenTargets
			// 
			this.chosenTargets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.chosenTargets.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.chosenTargets.Dock = System.Windows.Forms.DockStyle.Fill;
			this.chosenTargets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.chosenTargets.FormattingEnabled = true;
			this.chosenTargets.ItemHeight = 15;
			this.chosenTargets.Location = new System.Drawing.Point(369, 3);
			this.chosenTargets.Name = "chosenTargets";
			this.chosenTargets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.chosenTargets.Size = new System.Drawing.Size(220, 316);
			this.chosenTargets.TabIndex = 3;
			// 
			// buttonAdd
			// 
			this.buttonAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonAdd.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonAdd.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAdd.Location = new System.Drawing.Point(30, 3);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(78, 27);
			this.buttonAdd.TabIndex = 4;
			this.buttonAdd.Text = "->";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			this.buttonAdd.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonAdd.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonRemove.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonRemove.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonRemove.Location = new System.Drawing.Point(30, 36);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(78, 27);
			this.buttonRemove.TabIndex = 5;
			this.buttonRemove.Text = "<-";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.buttonRemove.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonRemove.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonReset
			// 
			this.buttonReset.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.buttonReset.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonReset.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonReset.Location = new System.Drawing.Point(30, 69);
			this.buttonReset.Name = "buttonReset";
			this.buttonReset.Size = new System.Drawing.Size(78, 27);
			this.buttonReset.TabIndex = 6;
			this.buttonReset.Text = "Reset";
			this.buttonReset.UseVisualStyleBackColor = true;
			this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
			this.buttonReset.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonReset.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// recurseCB
			// 
			this.recurseCB.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.recurseCB.AutoSize = true;
			this.recurseCB.Checked = true;
			this.recurseCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.recurseCB.Location = new System.Drawing.Point(14, 220);
			this.recurseCB.Name = "recurseCB";
			this.recurseCB.Size = new System.Drawing.Size(110, 19);
			this.recurseCB.TabIndex = 7;
			this.recurseCB.Text = "Add Recursively";
			this.recurseCB.UseVisualStyleBackColor = true;
			// 
			// allowGroupsCheckbox
			// 
			this.allowGroupsCheckbox.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.allowGroupsCheckbox.AutoSize = true;
			this.allowGroupsCheckbox.Location = new System.Drawing.Point(21, 195);
			this.allowGroupsCheckbox.Name = "allowGroupsCheckbox";
			this.allowGroupsCheckbox.Size = new System.Drawing.Size(97, 19);
			this.allowGroupsCheckbox.TabIndex = 8;
			this.allowGroupsCheckbox.Text = "Allow Groups";
			this.allowGroupsCheckbox.UseVisualStyleBackColor = true;
			this.allowGroupsCheckbox.CheckedChanged += new System.EventHandler(this.allowGroupsCheckbox_CheckedChanged);
			// 
			// stringsGroupBox
			// 
			this.stringsGroupBox.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.stringsGroupBox.Controls.Add(this.colsRadioButton);
			this.stringsGroupBox.Controls.Add(this.rowsRadioButton);
			this.stringsGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.stringsGroupBox.Location = new System.Drawing.Point(3, 102);
			this.stringsGroupBox.Name = "stringsGroupBox";
			this.stringsGroupBox.Size = new System.Drawing.Size(133, 87);
			this.stringsGroupBox.TabIndex = 10;
			this.stringsGroupBox.TabStop = false;
			this.stringsGroupBox.Text = "Strings are:";
			this.stringsGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// colsRadioButton
			// 
			this.colsRadioButton.AutoSize = true;
			this.colsRadioButton.Location = new System.Drawing.Point(15, 48);
			this.colsRadioButton.Name = "colsRadioButton";
			this.colsRadioButton.Size = new System.Drawing.Size(64, 19);
			this.colsRadioButton.TabIndex = 11;
			this.colsRadioButton.Text = "Vertical";
			this.colsRadioButton.UseVisualStyleBackColor = true;
			// 
			// rowsRadioButton
			// 
			this.rowsRadioButton.AutoSize = true;
			this.rowsRadioButton.Location = new System.Drawing.Point(15, 22);
			this.rowsRadioButton.Name = "rowsRadioButton";
			this.rowsRadioButton.Size = new System.Drawing.Size(80, 19);
			this.rowsRadioButton.TabIndex = 9;
			this.rowsRadioButton.Text = "Horizontal";
			this.rowsRadioButton.UseVisualStyleBackColor = true;
			// 
			// nodeTreeView
			// 
			this.nodeTreeView.AllowDrop = true;
			this.nodeTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.nodeTreeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.nodeTreeView.Cursor = System.Windows.Forms.Cursors.Default;
			this.nodeTreeView.CustomDragCursor = null;
			this.nodeTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.nodeTreeView.DragDefaultMode = System.Windows.Forms.DragDropEffects.Move;
			this.nodeTreeView.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.nodeTreeView.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.nodeTreeView.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
			this.nodeTreeView.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
			this.nodeTreeView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.nodeTreeView.Location = new System.Drawing.Point(3, 3);
			this.nodeTreeView.Name = "nodeTreeView";
			this.nodeTreeView.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("nodeTreeView.SelectedNodes")));
			this.nodeTreeView.Size = new System.Drawing.Size(218, 316);
			this.nodeTreeView.TabIndex = 2;
			this.nodeTreeView.UsingCustomDragCursor = false;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonOk);
			this.panel1.Controls.Add(this.buttonCancel);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 351);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(618, 38);
			this.panel1.TabIndex = 11;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.buttonAdd);
			this.flowLayoutPanel1.Controls.Add(this.buttonRemove);
			this.flowLayoutPanel1.Controls.Add(this.buttonReset);
			this.flowLayoutPanel1.Controls.Add(this.stringsGroupBox);
			this.flowLayoutPanel1.Controls.Add(this.allowGroupsCheckbox);
			this.flowLayoutPanel1.Controls.Add(this.recurseCB);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(227, 3);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(136, 316);
			this.flowLayoutPanel1.TabIndex = 12;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(this.nodeTreeView, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.chosenTargets, 2, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(14, 14);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(592, 322);
			this.tableLayoutPanel1.TabIndex = 13;
			// 
			// LipSyncNodeSelect
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(618, 389);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.panel1);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.Name = "LipSyncNodeSelect";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "LipSyncNodeSelect";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LipSyncNodeSelect_FormClosing);
			this.Load += new System.EventHandler(this.LipSyncNodeSelect_Load);
			this.stringsGroupBox.ResumeLayout(false);
			this.stringsGroupBox.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private Common.Controls.MultiSelectTreeview nodeTreeView;
        private System.Windows.Forms.ListBox chosenTargets;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.CheckBox recurseCB;
		private System.Windows.Forms.CheckBox allowGroupsCheckbox;
		private System.Windows.Forms.GroupBox stringsGroupBox;
		private System.Windows.Forms.RadioButton colsRadioButton;
		private System.Windows.Forms.RadioButton rowsRadioButton;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
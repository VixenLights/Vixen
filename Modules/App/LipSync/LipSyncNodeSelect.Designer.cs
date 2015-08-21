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
			this.stringsGroupBox.SuspendLayout();
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
			this.buttonOk.Location = new System.Drawing.Point(358, 292);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
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
			this.buttonCancel.Location = new System.Drawing.Point(439, 292);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
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
			this.chosenTargets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.chosenTargets.FormattingEnabled = true;
			this.chosenTargets.Location = new System.Drawing.Point(327, 12);
			this.chosenTargets.Name = "chosenTargets";
			this.chosenTargets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.chosenTargets.Size = new System.Drawing.Size(187, 262);
			this.chosenTargets.TabIndex = 3;
			// 
			// buttonAdd
			// 
			this.buttonAdd.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonAdd.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonAdd.Location = new System.Drawing.Point(231, 23);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.Size = new System.Drawing.Size(67, 23);
			this.buttonAdd.TabIndex = 4;
			this.buttonAdd.Text = "->";
			this.buttonAdd.UseVisualStyleBackColor = true;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			this.buttonAdd.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonAdd.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonRemove
			// 
			this.buttonRemove.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonRemove.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonRemove.Location = new System.Drawing.Point(231, 52);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(67, 23);
			this.buttonRemove.TabIndex = 5;
			this.buttonRemove.Text = "<-";
			this.buttonRemove.UseVisualStyleBackColor = true;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.buttonRemove.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonRemove.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonReset
			// 
			this.buttonReset.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonReset.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonReset.Location = new System.Drawing.Point(231, 81);
			this.buttonReset.Name = "buttonReset";
			this.buttonReset.Size = new System.Drawing.Size(67, 23);
			this.buttonReset.TabIndex = 6;
			this.buttonReset.Text = "Reset";
			this.buttonReset.UseVisualStyleBackColor = true;
			this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
			this.buttonReset.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonReset.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// recurseCB
			// 
			this.recurseCB.AutoSize = true;
			this.recurseCB.Checked = true;
			this.recurseCB.CheckState = System.Windows.Forms.CheckState.Checked;
			this.recurseCB.Location = new System.Drawing.Point(218, 246);
			this.recurseCB.Name = "recurseCB";
			this.recurseCB.Size = new System.Drawing.Size(103, 17);
			this.recurseCB.TabIndex = 7;
			this.recurseCB.Text = "Add Recursively";
			this.recurseCB.UseVisualStyleBackColor = true;
			// 
			// allowGroupsCheckbox
			// 
			this.allowGroupsCheckbox.AutoSize = true;
			this.allowGroupsCheckbox.Location = new System.Drawing.Point(218, 223);
			this.allowGroupsCheckbox.Name = "allowGroupsCheckbox";
			this.allowGroupsCheckbox.Size = new System.Drawing.Size(88, 17);
			this.allowGroupsCheckbox.TabIndex = 8;
			this.allowGroupsCheckbox.Text = "Allow Groups";
			this.allowGroupsCheckbox.UseVisualStyleBackColor = true;
			this.allowGroupsCheckbox.CheckedChanged += new System.EventHandler(this.allowGroupsCheckbox_CheckedChanged);
			// 
			// stringsGroupBox
			// 
			this.stringsGroupBox.Controls.Add(this.colsRadioButton);
			this.stringsGroupBox.Controls.Add(this.rowsRadioButton);
			this.stringsGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.stringsGroupBox.Location = new System.Drawing.Point(205, 121);
			this.stringsGroupBox.Name = "stringsGroupBox";
			this.stringsGroupBox.Size = new System.Drawing.Size(116, 75);
			this.stringsGroupBox.TabIndex = 10;
			this.stringsGroupBox.TabStop = false;
			this.stringsGroupBox.Text = "Strings are:";
			this.stringsGroupBox.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// colsRadioButton
			// 
			this.colsRadioButton.AutoSize = true;
			this.colsRadioButton.Location = new System.Drawing.Point(13, 42);
			this.colsRadioButton.Name = "colsRadioButton";
			this.colsRadioButton.Size = new System.Drawing.Size(60, 17);
			this.colsRadioButton.TabIndex = 11;
			this.colsRadioButton.Text = "Vertical";
			this.colsRadioButton.UseVisualStyleBackColor = true;
			// 
			// rowsRadioButton
			// 
			this.rowsRadioButton.AutoSize = true;
			this.rowsRadioButton.Location = new System.Drawing.Point(13, 19);
			this.rowsRadioButton.Name = "rowsRadioButton";
			this.rowsRadioButton.Size = new System.Drawing.Size(72, 17);
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
			this.nodeTreeView.DragDefaultMode = System.Windows.Forms.DragDropEffects.Move;
			this.nodeTreeView.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
			this.nodeTreeView.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
			this.nodeTreeView.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
			this.nodeTreeView.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
			this.nodeTreeView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.nodeTreeView.Location = new System.Drawing.Point(12, 12);
			this.nodeTreeView.Name = "nodeTreeView";
			this.nodeTreeView.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("nodeTreeView.SelectedNodes")));
			this.nodeTreeView.Size = new System.Drawing.Size(187, 262);
			this.nodeTreeView.TabIndex = 2;
			this.nodeTreeView.UsingCustomDragCursor = false;
			// 
			// LipSyncNodeSelect
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(530, 321);
			this.Controls.Add(this.stringsGroupBox);
			this.Controls.Add(this.allowGroupsCheckbox);
			this.Controls.Add(this.recurseCB);
			this.Controls.Add(this.buttonReset);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.buttonAdd);
			this.Controls.Add(this.chosenTargets);
			this.Controls.Add(this.nodeTreeView);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.MaximumSize = new System.Drawing.Size(546, 360);
			this.MinimumSize = new System.Drawing.Size(546, 360);
			this.Name = "LipSyncNodeSelect";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "LipSyncNodeSelect";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LipSyncNodeSelect_FormClosing);
			this.Load += new System.EventHandler(this.LipSyncNodeSelect_Load);
			this.stringsGroupBox.ResumeLayout(false);
			this.stringsGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

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
    }
}
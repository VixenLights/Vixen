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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.chosenTargets = new System.Windows.Forms.ListBox();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.resetButton = new System.Windows.Forms.Button();
            this.nodeTreeView = new Common.Controls.MultiSelectTreeview();
            this.recurseCB = new System.Windows.Forms.CheckBox();
            this.allowGroupsCheckbox = new System.Windows.Forms.CheckBox();
            this.rowsRadioButton = new System.Windows.Forms.RadioButton();
            this.stringsGroupBox = new System.Windows.Forms.GroupBox();
            this.bottomRightCheckBox = new System.Windows.Forms.CheckBox();
            this.colsRadioButton = new System.Windows.Forms.RadioButton();
            this.stringsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(327, 290);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(408, 290);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // chosenTargets
            // 
            this.chosenTargets.FormattingEnabled = true;
            this.chosenTargets.Location = new System.Drawing.Point(327, 12);
            this.chosenTargets.Name = "chosenTargets";
            this.chosenTargets.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.chosenTargets.Size = new System.Drawing.Size(187, 251);
            this.chosenTargets.TabIndex = 3;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(231, 23);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(67, 23);
            this.addButton.TabIndex = 4;
            this.addButton.Text = "->";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Location = new System.Drawing.Point(231, 52);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(67, 23);
            this.removeButton.TabIndex = 5;
            this.removeButton.Text = "<-";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // resetButton
            // 
            this.resetButton.Location = new System.Drawing.Point(231, 81);
            this.resetButton.Name = "resetButton";
            this.resetButton.Size = new System.Drawing.Size(67, 23);
            this.resetButton.TabIndex = 6;
            this.resetButton.Text = "Reset";
            this.resetButton.UseVisualStyleBackColor = true;
            this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
            // 
            // nodeTreeView
            // 
            this.nodeTreeView.AllowDrop = true;
            this.nodeTreeView.Cursor = System.Windows.Forms.Cursors.Default;
            this.nodeTreeView.CustomDragCursor = null;
            this.nodeTreeView.DragDefaultMode = System.Windows.Forms.DragDropEffects.Move;
            this.nodeTreeView.DragDestinationNodeBackColor = System.Drawing.SystemColors.Highlight;
            this.nodeTreeView.DragDestinationNodeForeColor = System.Drawing.SystemColors.HighlightText;
            this.nodeTreeView.DragSourceNodeBackColor = System.Drawing.SystemColors.ControlLight;
            this.nodeTreeView.DragSourceNodeForeColor = System.Drawing.SystemColors.ControlText;
            this.nodeTreeView.Location = new System.Drawing.Point(12, 12);
            this.nodeTreeView.Name = "nodeTreeView";
            this.nodeTreeView.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("nodeTreeView.SelectedNodes")));
            this.nodeTreeView.Size = new System.Drawing.Size(187, 256);
            this.nodeTreeView.TabIndex = 2;
            this.nodeTreeView.UsingCustomDragCursor = false;
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
            // rowsRadioButton
            // 
            this.rowsRadioButton.AutoSize = true;
            this.rowsRadioButton.Location = new System.Drawing.Point(13, 19);
            this.rowsRadioButton.Name = "rowsRadioButton";
            this.rowsRadioButton.Size = new System.Drawing.Size(52, 17);
            this.rowsRadioButton.TabIndex = 9;
            this.rowsRadioButton.Text = "Rows";
            this.rowsRadioButton.UseVisualStyleBackColor = true;
            this.rowsRadioButton.CheckedChanged += new System.EventHandler(this.rowsRadioButton_CheckedChanged);
            // 
            // stringsGroupBox
            // 
            this.stringsGroupBox.Controls.Add(this.bottomRightCheckBox);
            this.stringsGroupBox.Controls.Add(this.colsRadioButton);
            this.stringsGroupBox.Controls.Add(this.rowsRadioButton);
            this.stringsGroupBox.Location = new System.Drawing.Point(205, 121);
            this.stringsGroupBox.Name = "stringsGroupBox";
            this.stringsGroupBox.Size = new System.Drawing.Size(116, 86);
            this.stringsGroupBox.TabIndex = 10;
            this.stringsGroupBox.TabStop = false;
            this.stringsGroupBox.Text = "Strings are";
            // 
            // bottomRightCheckBox
            // 
            this.bottomRightCheckBox.AutoSize = true;
            this.bottomRightCheckBox.Checked = true;
            this.bottomRightCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bottomRightCheckBox.Location = new System.Drawing.Point(13, 65);
            this.bottomRightCheckBox.Name = "bottomRightCheckBox";
            this.bottomRightCheckBox.Size = new System.Drawing.Size(93, 17);
            this.bottomRightCheckBox.TabIndex = 12;
            this.bottomRightCheckBox.Text = "Bottom to Top";
            this.bottomRightCheckBox.UseVisualStyleBackColor = true;
            // 
            // colsRadioButton
            // 
            this.colsRadioButton.AutoSize = true;
            this.colsRadioButton.Location = new System.Drawing.Point(13, 42);
            this.colsRadioButton.Name = "colsRadioButton";
            this.colsRadioButton.Size = new System.Drawing.Size(45, 17);
            this.colsRadioButton.TabIndex = 11;
            this.colsRadioButton.Text = "Cols";
            this.colsRadioButton.UseVisualStyleBackColor = true;
            this.colsRadioButton.CheckedChanged += new System.EventHandler(this.colsRadioButton_CheckedChanged);
            // 
            // LipSyncNodeSelect
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(532, 327);
            this.Controls.Add(this.stringsGroupBox);
            this.Controls.Add(this.allowGroupsCheckbox);
            this.Controls.Add(this.recurseCB);
            this.Controls.Add(this.resetButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.chosenTargets);
            this.Controls.Add(this.nodeTreeView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Name = "LipSyncNodeSelect";
            this.Text = "LipSyncNodeSelect";
            this.Load += new System.EventHandler(this.LipSyncNodeSelect_Load);
            this.stringsGroupBox.ResumeLayout(false);
            this.stringsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Common.Controls.MultiSelectTreeview nodeTreeView;
        private System.Windows.Forms.ListBox chosenTargets;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button resetButton;
        private System.Windows.Forms.CheckBox recurseCB;
        private System.Windows.Forms.CheckBox allowGroupsCheckbox;
        private System.Windows.Forms.RadioButton rowsRadioButton;
        private System.Windows.Forms.GroupBox stringsGroupBox;
        private System.Windows.Forms.RadioButton colsRadioButton;
        private System.Windows.Forms.CheckBox bottomRightCheckBox;
    }
}
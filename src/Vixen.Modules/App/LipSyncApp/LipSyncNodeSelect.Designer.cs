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
			buttonOk = new Button();
			buttonCancel = new Button();
			chosenTargets = new ListBox();
			buttonAdd = new Button();
			buttonRemove = new Button();
			buttonReset = new Button();
			recurseCB = new CheckBox();
			allowGroupsCheckbox = new CheckBox();
			stringsGroupBox = new GroupBox();
			colsRadioButton = new RadioButton();
			rowsRadioButton = new RadioButton();
			nodeTreeView = new Common.Controls.MultiSelectTreeview();
			panel1 = new Panel();
			flowLayoutPanel1 = new FlowLayoutPanel();
			tableLayoutPanel1 = new TableLayoutPanel();
			stringsGroupBox.SuspendLayout();
			panel1.SuspendLayout();
			flowLayoutPanel1.SuspendLayout();
			tableLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// buttonOk
			// 
			buttonOk.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonOk.DialogResult = DialogResult.OK;
			buttonOk.Location = new Point(483, 6);
			buttonOk.Name = "buttonOk";
			buttonOk.Size = new Size(87, 27);
			buttonOk.TabIndex = 0;
			buttonOk.Text = "OK";
			buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.Location = new Point(576, 6);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(87, 27);
			buttonCancel.TabIndex = 1;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// chosenTargets
			// 
			chosenTargets.Dock = DockStyle.Fill;
			chosenTargets.FormattingEnabled = true;
			chosenTargets.ItemHeight = 15;
			chosenTargets.Location = new Point(411, 3);
			chosenTargets.Name = "chosenTargets";
			chosenTargets.SelectionMode = SelectionMode.MultiExtended;
			chosenTargets.Size = new Size(261, 389);
			chosenTargets.TabIndex = 3;
			// 
			// buttonAdd
			// 
			buttonAdd.Anchor = AnchorStyles.None;
			buttonAdd.Location = new Point(30, 3);
			buttonAdd.Name = "buttonAdd";
			buttonAdd.Size = new Size(78, 27);
			buttonAdd.TabIndex = 4;
			buttonAdd.Text = "->";
			buttonAdd.UseVisualStyleBackColor = true;
			buttonAdd.Click += buttonAdd_Click;
			// 
			// buttonRemove
			// 
			buttonRemove.Anchor = AnchorStyles.None;
			buttonRemove.Location = new Point(30, 36);
			buttonRemove.Name = "buttonRemove";
			buttonRemove.Size = new Size(78, 27);
			buttonRemove.TabIndex = 5;
			buttonRemove.Text = "<-";
			buttonRemove.UseVisualStyleBackColor = true;
			buttonRemove.Click += buttonRemove_Click;
			// 
			// buttonReset
			// 
			buttonReset.Anchor = AnchorStyles.None;
			buttonReset.Location = new Point(30, 69);
			buttonReset.Name = "buttonReset";
			buttonReset.Size = new Size(78, 27);
			buttonReset.TabIndex = 6;
			buttonReset.Text = "Reset";
			buttonReset.UseVisualStyleBackColor = true;
			buttonReset.Click += buttonReset_Click;
			// 
			// recurseCB
			// 
			recurseCB.Anchor = AnchorStyles.Top;
			recurseCB.AutoSize = true;
			recurseCB.Checked = true;
			recurseCB.CheckState = CheckState.Checked;
			recurseCB.Location = new Point(14, 220);
			recurseCB.Name = "recurseCB";
			recurseCB.Size = new Size(110, 19);
			recurseCB.TabIndex = 7;
			recurseCB.Text = "Add Recursively";
			recurseCB.UseVisualStyleBackColor = true;
			// 
			// allowGroupsCheckbox
			// 
			allowGroupsCheckbox.Anchor = AnchorStyles.Top;
			allowGroupsCheckbox.AutoSize = true;
			allowGroupsCheckbox.Location = new Point(21, 195);
			allowGroupsCheckbox.Name = "allowGroupsCheckbox";
			allowGroupsCheckbox.Size = new Size(97, 19);
			allowGroupsCheckbox.TabIndex = 8;
			allowGroupsCheckbox.Text = "Allow Groups";
			allowGroupsCheckbox.UseVisualStyleBackColor = true;
			allowGroupsCheckbox.CheckedChanged += allowGroupsCheckbox_CheckedChanged;
			// 
			// stringsGroupBox
			// 
			stringsGroupBox.Anchor = AnchorStyles.None;
			stringsGroupBox.Controls.Add(colsRadioButton);
			stringsGroupBox.Controls.Add(rowsRadioButton);
			stringsGroupBox.Location = new Point(3, 102);
			stringsGroupBox.Name = "stringsGroupBox";
			stringsGroupBox.Size = new Size(133, 87);
			stringsGroupBox.TabIndex = 10;
			stringsGroupBox.TabStop = false;
			stringsGroupBox.Text = "Strings are:";
			stringsGroupBox.Paint += groupBoxes_Paint;
			// 
			// colsRadioButton
			// 
			colsRadioButton.AutoSize = true;
			colsRadioButton.Location = new Point(15, 48);
			colsRadioButton.Name = "colsRadioButton";
			colsRadioButton.Size = new Size(63, 19);
			colsRadioButton.TabIndex = 11;
			colsRadioButton.Text = "Vertical";
			colsRadioButton.UseVisualStyleBackColor = true;
			// 
			// rowsRadioButton
			// 
			rowsRadioButton.AutoSize = true;
			rowsRadioButton.Location = new Point(15, 22);
			rowsRadioButton.Name = "rowsRadioButton";
			rowsRadioButton.Size = new Size(80, 19);
			rowsRadioButton.TabIndex = 9;
			rowsRadioButton.Text = "Horizontal";
			rowsRadioButton.UseVisualStyleBackColor = true;
			// 
			// nodeTreeView
			// 
			nodeTreeView.AllowDrop = true;
			nodeTreeView.CustomDragCursor = null;
			nodeTreeView.Dock = DockStyle.Fill;
			nodeTreeView.DragDefaultMode = DragDropEffects.Move;
			nodeTreeView.DragDestinationNodeBackColor = SystemColors.Highlight;
			nodeTreeView.DragDestinationNodeForeColor = SystemColors.HighlightText;
			nodeTreeView.DragSourceNodeBackColor = SystemColors.ControlLight;
			nodeTreeView.DragSourceNodeForeColor = SystemColors.ControlText;
			nodeTreeView.Location = new Point(3, 3);
			nodeTreeView.Name = "nodeTreeView";
			nodeTreeView.SelectedNodes = (List<TreeNode>)resources.GetObject("nodeTreeView.SelectedNodes");
			nodeTreeView.Size = new Size(260, 389);
			nodeTreeView.TabIndex = 2;
			nodeTreeView.UsingCustomDragCursor = false;
			// 
			// panel1
			// 
			panel1.Controls.Add(buttonOk);
			panel1.Controls.Add(buttonCancel);
			panel1.Dock = DockStyle.Bottom;
			panel1.Location = new Point(0, 395);
			panel1.Name = "panel1";
			panel1.Size = new Size(675, 38);
			panel1.TabIndex = 11;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.Controls.Add(buttonAdd);
			flowLayoutPanel1.Controls.Add(buttonRemove);
			flowLayoutPanel1.Controls.Add(buttonReset);
			flowLayoutPanel1.Controls.Add(stringsGroupBox);
			flowLayoutPanel1.Controls.Add(allowGroupsCheckbox);
			flowLayoutPanel1.Controls.Add(recurseCB);
			flowLayoutPanel1.Dock = DockStyle.Fill;
			flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
			flowLayoutPanel1.Location = new Point(269, 3);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new Size(136, 389);
			flowLayoutPanel1.TabIndex = 12;
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.ColumnCount = 3;
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			tableLayoutPanel1.Controls.Add(nodeTreeView, 0, 0);
			tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 1, 0);
			tableLayoutPanel1.Controls.Add(chosenTargets, 2, 0);
			tableLayoutPanel1.Dock = DockStyle.Fill;
			tableLayoutPanel1.Location = new Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new Size(675, 395);
			tableLayoutPanel1.TabIndex = 13;
			// 
			// LipSyncNodeSelect
			// 
			AcceptButton = buttonOk;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			CancelButton = buttonCancel;
			ClientSize = new Size(675, 433);
			Controls.Add(tableLayoutPanel1);
			Controls.Add(panel1);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "LipSyncNodeSelect";
			SizeGripStyle = SizeGripStyle.Hide;
			StartPosition = FormStartPosition.CenterParent;
			Text = "LipSync Mapping Node Select";
			FormClosing += LipSyncNodeSelect_FormClosing;
			Load += LipSyncNodeSelect_Load;
			stringsGroupBox.ResumeLayout(false);
			stringsGroupBox.PerformLayout();
			panel1.ResumeLayout(false);
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

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
namespace Dataweb.NShape.Designer {

	partial class DisplaySettingsForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
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
		private void InitializeComponent() {
			this.snapToGridCheckBox = new System.Windows.Forms.CheckBox();
			this.gridSizeUpDown = new System.Windows.Forms.NumericUpDown();
			this.snapDistanceUpDown = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.showGridCheckBox = new System.Windows.Forms.CheckBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.chooseGridColorButton = new System.Windows.Forms.Button();
			this.gridColorLabel = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.pointSizeUpDown = new System.Windows.Forms.NumericUpDown();
			this.connectionPointCombo = new System.Windows.Forms.ComboBox();
			this.resizePointCombo = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.showDynamicContextMenu = new System.Windows.Forms.CheckBox();
			this.hideDeniedMenuItemsCheckBox = new System.Windows.Forms.CheckBox();
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			((System.ComponentModel.ISupportInitialize)(this.gridSizeUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.snapDistanceUpDown)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pointSizeUpDown)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// snapToGridCheckBox
			// 
			this.snapToGridCheckBox.AutoSize = true;
			this.snapToGridCheckBox.Location = new System.Drawing.Point(6, 42);
			this.snapToGridCheckBox.Name = "snapToGridCheckBox";
			this.snapToGridCheckBox.Size = new System.Drawing.Size(85, 17);
			this.snapToGridCheckBox.TabIndex = 0;
			this.snapToGridCheckBox.Text = "Snap to Grid";
			this.snapToGridCheckBox.UseVisualStyleBackColor = true;
			// 
			// gridSizeUpDown
			// 
			this.gridSizeUpDown.AutoSize = true;
			this.gridSizeUpDown.Location = new System.Drawing.Point(225, 18);
			this.gridSizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.gridSizeUpDown.Name = "gridSizeUpDown";
			this.gridSizeUpDown.Size = new System.Drawing.Size(41, 20);
			this.gridSizeUpDown.TabIndex = 7;
			this.gridSizeUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// snapDistanceUpDown
			// 
			this.snapDistanceUpDown.AutoSize = true;
			this.snapDistanceUpDown.Location = new System.Drawing.Point(225, 41);
			this.snapDistanceUpDown.Name = "snapDistanceUpDown";
			this.snapDistanceUpDown.Size = new System.Drawing.Size(41, 20);
			this.snapDistanceUpDown.TabIndex = 8;
			this.snapDistanceUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.snapDistanceUpDown.ValueChanged += new System.EventHandler(this.snapDistanceUpDown_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(142, 43);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 13);
			this.label1.TabIndex = 11;
			this.label1.Text = "Snap Distance";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(142, 20);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(49, 13);
			this.label2.TabIndex = 12;
			this.label2.Text = "Grid Size";
			// 
			// showGridCheckBox
			// 
			this.showGridCheckBox.AutoSize = true;
			this.showGridCheckBox.Location = new System.Drawing.Point(6, 18);
			this.showGridCheckBox.Name = "showGridCheckBox";
			this.showGridCheckBox.Size = new System.Drawing.Size(75, 17);
			this.showGridCheckBox.TabIndex = 13;
			this.showGridCheckBox.Text = "Show Grid";
			this.showGridCheckBox.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(227, 337);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 14;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(146, 337);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 15;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(110, 13);
			this.label3.TabIndex = 16;
			this.label3.Text = "Resize Handle Shape";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.chooseGridColorButton);
			this.groupBox1.Controls.Add(this.gridColorLabel);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.showGridCheckBox);
			this.groupBox1.Controls.Add(this.snapDistanceUpDown);
			this.groupBox1.Controls.Add(this.gridSizeUpDown);
			this.groupBox1.Controls.Add(this.snapToGridCheckBox);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(289, 98);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Grid Settings";
			// 
			// chooseGridColorButton
			// 
			this.chooseGridColorButton.Location = new System.Drawing.Point(93, 67);
			this.chooseGridColorButton.Name = "chooseGridColorButton";
			this.chooseGridColorButton.Size = new System.Drawing.Size(24, 23);
			this.chooseGridColorButton.TabIndex = 16;
			this.chooseGridColorButton.Text = "...";
			this.chooseGridColorButton.UseVisualStyleBackColor = true;
			this.chooseGridColorButton.Click += new System.EventHandler(this.chooseGridColorButton_Click);
			// 
			// gridColorLabel
			// 
			this.gridColorLabel.BackColor = System.Drawing.Color.White;
			this.gridColorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gridColorLabel.Location = new System.Drawing.Point(64, 67);
			this.gridColorLabel.Name = "gridColorLabel";
			this.gridColorLabel.Size = new System.Drawing.Size(23, 23);
			this.gridColorLabel.TabIndex = 15;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(6, 72);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(52, 13);
			this.label6.TabIndex = 14;
			this.label6.Text = "Grid color";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.pointSizeUpDown);
			this.groupBox2.Controls.Add(this.connectionPointCombo);
			this.groupBox2.Controls.Add(this.resizePointCombo);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Location = new System.Drawing.Point(12, 116);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(289, 100);
			this.groupBox2.TabIndex = 18;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Control Point Settings";
			// 
			// pointSizeUpDown
			// 
			this.pointSizeUpDown.Location = new System.Drawing.Point(134, 67);
			this.pointSizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.pointSizeUpDown.Name = "pointSizeUpDown";
			this.pointSizeUpDown.Size = new System.Drawing.Size(37, 20);
			this.pointSizeUpDown.TabIndex = 21;
			this.pointSizeUpDown.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
			// 
			// connectionPointCombo
			// 
			this.connectionPointCombo.FormattingEnabled = true;
			this.connectionPointCombo.Location = new System.Drawing.Point(134, 40);
			this.connectionPointCombo.Name = "connectionPointCombo";
			this.connectionPointCombo.Size = new System.Drawing.Size(132, 21);
			this.connectionPointCombo.TabIndex = 20;
			// 
			// resizePointCombo
			// 
			this.resizePointCombo.FormattingEnabled = true;
			this.resizePointCombo.Location = new System.Drawing.Point(134, 13);
			this.resizePointCombo.Name = "resizePointCombo";
			this.resizePointCombo.Size = new System.Drawing.Size(132, 21);
			this.resizePointCombo.TabIndex = 19;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 69);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(87, 13);
			this.label5.TabIndex = 18;
			this.label5.Text = "ControlPoint Size";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 43);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(122, 13);
			this.label4.TabIndex = 17;
			this.label4.Text = "Connection Point Shape";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.showDynamicContextMenu);
			this.groupBox3.Controls.Add(this.hideDeniedMenuItemsCheckBox);
			this.groupBox3.Location = new System.Drawing.Point(12, 222);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(289, 100);
			this.groupBox3.TabIndex = 19;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Permissions";
			// 
			// showDynamicContextMenu
			// 
			this.showDynamicContextMenu.AutoSize = true;
			this.showDynamicContextMenu.Location = new System.Drawing.Point(6, 55);
			this.showDynamicContextMenu.Name = "showDynamicContextMenu";
			this.showDynamicContextMenu.Size = new System.Drawing.Size(167, 17);
			this.showDynamicContextMenu.TabIndex = 1;
			this.showDynamicContextMenu.Text = "Show NShape context menus";
			this.showDynamicContextMenu.UseVisualStyleBackColor = true;
			// 
			// hideDeniedMenuItemsCheckBox
			// 
			this.hideDeniedMenuItemsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hideDeniedMenuItemsCheckBox.Location = new System.Drawing.Point(6, 19);
			this.hideDeniedMenuItemsCheckBox.Name = "hideDeniedMenuItemsCheckBox";
			this.hideDeniedMenuItemsCheckBox.Size = new System.Drawing.Size(277, 30);
			this.hideDeniedMenuItemsCheckBox.TabIndex = 0;
			this.hideDeniedMenuItemsCheckBox.Text = "Hide menu items that are not allowed due to insufficent permissions.";
			this.hideDeniedMenuItemsCheckBox.UseVisualStyleBackColor = true;
			// 
			// colorDialog
			// 
			this.colorDialog.AnyColor = true;
			this.colorDialog.FullOpen = true;
			// 
			// DisplaySettingsForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(313, 372);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "DisplaySettingsForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Display Settings";
			((System.ComponentModel.ISupportInitialize)(this.gridSizeUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.snapDistanceUpDown)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pointSizeUpDown)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox snapToGridCheckBox;
		private System.Windows.Forms.NumericUpDown gridSizeUpDown;
		private System.Windows.Forms.NumericUpDown snapDistanceUpDown;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox showGridCheckBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown pointSizeUpDown;
		private System.Windows.Forms.ComboBox connectionPointCombo;
		private System.Windows.Forms.ComboBox resizePointCombo;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.CheckBox showDynamicContextMenu;
		private System.Windows.Forms.CheckBox hideDeniedMenuItemsCheckBox;
		private System.Windows.Forms.Label gridColorLabel;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.Button chooseGridColorButton;
	}
}
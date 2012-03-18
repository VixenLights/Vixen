namespace VixenApplication.PatchingControls {
	partial class ToNOutputsOverControllersEditor {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.comboBoxOutputIndex = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxStartingController = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxOutputPatchCount = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.checkedListBoxSubsequentControllers = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// comboBoxOutputIndex
			// 
			this.comboBoxOutputIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOutputIndex.FormattingEnabled = true;
			this.comboBoxOutputIndex.Location = new System.Drawing.Point(188, 103);
			this.comboBoxOutputIndex.Name = "comboBoxOutputIndex";
			this.comboBoxOutputIndex.Size = new System.Drawing.Size(98, 21);
			this.comboBoxOutputIndex.TabIndex = 11;
			this.comboBoxOutputIndex.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxOutputIndex_Validating);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(28, 108);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(39, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Output";
			// 
			// comboBoxStartingController
			// 
			this.comboBoxStartingController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxStartingController.FormattingEnabled = true;
			this.comboBoxStartingController.Location = new System.Drawing.Point(188, 76);
			this.comboBoxStartingController.Name = "comboBoxStartingController";
			this.comboBoxStartingController.Size = new System.Drawing.Size(194, 21);
			this.comboBoxStartingController.TabIndex = 9;
			this.comboBoxStartingController.SelectedIndexChanged += new System.EventHandler(this.comboBoxStartingController_SelectedIndexChanged);
			this.comboBoxStartingController.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxStartingController_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(28, 79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 13);
			this.label2.TabIndex = 8;
			this.label2.Text = "Starting on controller";
			// 
			// textBoxOutputPatchCount
			// 
			this.textBoxOutputPatchCount.Location = new System.Drawing.Point(188, 22);
			this.textBoxOutputPatchCount.Name = "textBoxOutputPatchCount";
			this.textBoxOutputPatchCount.Size = new System.Drawing.Size(82, 20);
			this.textBoxOutputPatchCount.TabIndex = 7;
			this.textBoxOutputPatchCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxOutputPatchCount_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 26);
			this.label1.TabIndex = 6;
			this.label1.Text = "Number of outputs \r\nto patch per channel";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(28, 167);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(96, 26);
			this.label4.TabIndex = 12;
			this.label4.Text = "Continuing through\r\ncontrollers";
			// 
			// checkedListBoxSubsequentControllers
			// 
			this.checkedListBoxSubsequentControllers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkedListBoxSubsequentControllers.CheckOnClick = true;
			this.checkedListBoxSubsequentControllers.FormattingEnabled = true;
			this.checkedListBoxSubsequentControllers.Location = new System.Drawing.Point(188, 167);
			this.checkedListBoxSubsequentControllers.Name = "checkedListBoxSubsequentControllers";
			this.checkedListBoxSubsequentControllers.Size = new System.Drawing.Size(194, 124);
			this.checkedListBoxSubsequentControllers.TabIndex = 13;
			// 
			// ToNOutputsOverControllersEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.checkedListBoxSubsequentControllers);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboBoxOutputIndex);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBoxStartingController);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxOutputPatchCount);
			this.Controls.Add(this.label1);
			this.Name = "ToNOutputsOverControllersEditor";
			this.Size = new System.Drawing.Size(413, 316);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxOutputIndex;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxStartingController;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxOutputPatchCount;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckedListBox checkedListBoxSubsequentControllers;
	}
}

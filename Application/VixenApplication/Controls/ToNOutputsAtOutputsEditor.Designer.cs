namespace VixenApplication.Controls {
	partial class ToNOutputsAtOutputsEditor {
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
			this.textBoxOutputPatchCount = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.checkedListBoxControllers = new System.Windows.Forms.CheckedListBox();
			this.SuspendLayout();
			// 
			// comboBoxOutputIndex
			// 
			this.comboBoxOutputIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.comboBoxOutputIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOutputIndex.FormattingEnabled = true;
			this.comboBoxOutputIndex.Location = new System.Drawing.Point(195, 189);
			this.comboBoxOutputIndex.Name = "comboBoxOutputIndex";
			this.comboBoxOutputIndex.Size = new System.Drawing.Size(98, 21);
			this.comboBoxOutputIndex.TabIndex = 5;
			this.comboBoxOutputIndex.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxOutputIndex_Validating);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(35, 189);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(98, 26);
			this.label3.TabIndex = 4;
			this.label3.Text = "Starting at output\r\n(on each controller)";
			// 
			// textBoxOutputPatchCount
			// 
			this.textBoxOutputPatchCount.Location = new System.Drawing.Point(195, 35);
			this.textBoxOutputPatchCount.Name = "textBoxOutputPatchCount";
			this.textBoxOutputPatchCount.Size = new System.Drawing.Size(82, 20);
			this.textBoxOutputPatchCount.TabIndex = 1;
			this.textBoxOutputPatchCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxOutputPatchCount_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(35, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 26);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number of outputs \r\nto patch per channel";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(35, 89);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "On controllers";
			// 
			// checkedListBoxControllers
			// 
			this.checkedListBoxControllers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.checkedListBoxControllers.CheckOnClick = true;
			this.checkedListBoxControllers.FormattingEnabled = true;
			this.checkedListBoxControllers.Location = new System.Drawing.Point(195, 89);
			this.checkedListBoxControllers.Name = "checkedListBoxControllers";
			this.checkedListBoxControllers.Size = new System.Drawing.Size(165, 79);
			this.checkedListBoxControllers.TabIndex = 3;
			this.checkedListBoxControllers.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBoxControllers_ItemCheck);
			this.checkedListBoxControllers.Validating += new System.ComponentModel.CancelEventHandler(this.checkedListBoxControllers_Validating);
			// 
			// ToNOutputsAtOutputsEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CausesValidation = false;
			this.Controls.Add(this.checkedListBoxControllers);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBoxOutputIndex);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxOutputPatchCount);
			this.Controls.Add(this.label1);
			this.Name = "ToNOutputsAtOutputsEditor";
			this.Size = new System.Drawing.Size(394, 259);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBoxOutputIndex;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBoxOutputPatchCount;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckedListBox checkedListBoxControllers;
	}
}

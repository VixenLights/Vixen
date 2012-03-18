namespace VixenApplication.PatchingControls {
	partial class ToNOutputsAtOutputEditor {
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
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxOutputPatchCount = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBoxController = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxOutputIndex = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 26);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number of outputs \r\nto patch per channel";
			// 
			// textBoxOutputPatchCount
			// 
			this.textBoxOutputPatchCount.Location = new System.Drawing.Point(178, 22);
			this.textBoxOutputPatchCount.Name = "textBoxOutputPatchCount";
			this.textBoxOutputPatchCount.Size = new System.Drawing.Size(82, 20);
			this.textBoxOutputPatchCount.TabIndex = 1;
			this.textBoxOutputPatchCount.Validating += new System.ComponentModel.CancelEventHandler(this.textBoxOutputPatchCount_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(18, 79);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 13);
			this.label2.TabIndex = 2;
			this.label2.Text = "On controller";
			// 
			// comboBoxController
			// 
			this.comboBoxController.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxController.FormattingEnabled = true;
			this.comboBoxController.Location = new System.Drawing.Point(178, 76);
			this.comboBoxController.Name = "comboBoxController";
			this.comboBoxController.Size = new System.Drawing.Size(194, 21);
			this.comboBoxController.TabIndex = 3;
			this.comboBoxController.SelectedIndexChanged += new System.EventHandler(this.comboBoxController_SelectedIndexChanged);
			this.comboBoxController.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxController_Validating);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(18, 137);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 13);
			this.label3.TabIndex = 4;
			this.label3.Text = "Starting at output";
			// 
			// comboBoxOutputIndex
			// 
			this.comboBoxOutputIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxOutputIndex.FormattingEnabled = true;
			this.comboBoxOutputIndex.Location = new System.Drawing.Point(178, 132);
			this.comboBoxOutputIndex.Name = "comboBoxOutputIndex";
			this.comboBoxOutputIndex.Size = new System.Drawing.Size(98, 21);
			this.comboBoxOutputIndex.TabIndex = 5;
			this.comboBoxOutputIndex.Validating += new System.ComponentModel.CancelEventHandler(this.comboBoxOutputIndex_Validating);
			// 
			// ToNOutputsAtOutputEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboBoxOutputIndex);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBoxController);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBoxOutputPatchCount);
			this.Controls.Add(this.label1);
			this.MinimumSize = new System.Drawing.Size(398, 200);
			this.Name = "ToNOutputsAtOutputEditor";
			this.Size = new System.Drawing.Size(398, 200);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxOutputPatchCount;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxController;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxOutputIndex;
	}
}

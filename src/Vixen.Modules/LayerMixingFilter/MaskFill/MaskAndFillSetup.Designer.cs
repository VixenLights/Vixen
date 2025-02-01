namespace VixenModules.LayerMixingFilter.MaskFill
{
	partial class MaskAndFillSetup
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
			chkExcludeZero = new CheckBox();
			btnOk = new Button();
			btnCancel = new Button();
			chkRequireMixingPartner = new CheckBox();
			SuspendLayout();
			// 
			// chkExcludeZero
			// 
			chkExcludeZero.AutoSize = true;
			chkExcludeZero.Location = new Point(33, 28);
			chkExcludeZero.Name = "chkExcludeZero";
			chkExcludeZero.Size = new Size(128, 19);
			chkExcludeZero.TabIndex = 0;
			chkExcludeZero.Text = "Exclude zero values";
			chkExcludeZero.UseVisualStyleBackColor = true;
			chkExcludeZero.CheckedChanged += chkExcludeZero_CheckedChanged;
			// 
			// btnOk
			// 
			btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnOk.DialogResult = DialogResult.OK;
			btnOk.Location = new Point(116, 88);
			btnOk.Name = "btnOk";
			btnOk.Size = new Size(75, 23);
			btnOk.TabIndex = 1;
			btnOk.Text = "OK";
			btnOk.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.Location = new Point(197, 88);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(75, 23);
			btnCancel.TabIndex = 2;
			btnCancel.Text = "Cancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// chkRequireMixingPartner
			// 
			chkRequireMixingPartner.AutoSize = true;
			chkRequireMixingPartner.Location = new Point(33, 53);
			chkRequireMixingPartner.Name = "chkRequireMixingPartner";
			chkRequireMixingPartner.Size = new Size(147, 19);
			chkRequireMixingPartner.TabIndex = 3;
			chkRequireMixingPartner.Text = "Require Mixing Partner";
			chkRequireMixingPartner.UseVisualStyleBackColor = true;
			chkRequireMixingPartner.CheckedChanged += chkRequireMixingPartner_CheckedChanged;
			// 
			// MaskAndFillSetup
			// 
			AcceptButton = btnOk;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			CancelButton = btnCancel;
			ClientSize = new Size(284, 123);
			Controls.Add(chkRequireMixingPartner);
			Controls.Add(btnCancel);
			Controls.Add(btnOk);
			Controls.Add(chkExcludeZero);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "MaskAndFillSetup";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Mask And Fill Configuration";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.CheckBox chkExcludeZero;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.CheckBox chkRequireMixingPartner;
	}
}
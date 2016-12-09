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
			this.chkExcludeZero = new System.Windows.Forms.CheckBox();
			this.btnOk = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// chkIncludeZero
			// 
			this.chkExcludeZero.AutoSize = true;
			this.chkExcludeZero.Location = new System.Drawing.Point(33, 28);
			this.chkExcludeZero.Name = "chkExcludeZero";
			this.chkExcludeZero.Size = new System.Drawing.Size(127, 19);
			this.chkExcludeZero.TabIndex = 0;
			this.chkExcludeZero.Text = "Exclude zero values";
			this.chkExcludeZero.UseVisualStyleBackColor = true;
			this.chkExcludeZero.CheckedChanged += new System.EventHandler(this.chkExcludeZero_CheckedChanged);
			// 
			// btnOk
			// 
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Location = new System.Drawing.Point(116, 88);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			this.btnOk.UseVisualStyleBackColor = true;
			this.btnOk.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnOk.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(197, 88);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.btnCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// MaskAndFillSetup
			// 
			this.AcceptButton = this.btnOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(284, 123);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.chkExcludeZero);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.HelpButton = true;
			this.Name = "MaskAndFillSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "MaskAndFillSetup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox chkExcludeZero;
		private System.Windows.Forms.Button btnOk;
		private System.Windows.Forms.Button btnCancel;
	}
}
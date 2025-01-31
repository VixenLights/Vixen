namespace VixenModules.Preview.VixenPreview
{
	partial class TemplateDialog
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
			this.buttonAddToPreview = new System.Windows.Forms.Button();
			this.comboBoxTemplates = new System.Windows.Forms.ComboBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonAddToPreview
			// 
			this.buttonAddToPreview.AutoSize = true;
			this.buttonAddToPreview.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonAddToPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonAddToPreview.Location = new System.Drawing.Point(14, 48);
			this.buttonAddToPreview.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.buttonAddToPreview.Name = "buttonAddToPreview";
			this.buttonAddToPreview.Size = new System.Drawing.Size(115, 31);
			this.buttonAddToPreview.TabIndex = 30;
			this.buttonAddToPreview.Text = "Add to Preview";
			this.buttonAddToPreview.UseVisualStyleBackColor = true;
			// 
			// comboBoxTemplates
			// 
			this.comboBoxTemplates.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			this.comboBoxTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxTemplates.FormattingEnabled = true;
			this.comboBoxTemplates.Location = new System.Drawing.Point(14, 15);
			this.comboBoxTemplates.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.comboBoxTemplates.Name = "comboBoxTemplates";
			this.comboBoxTemplates.Size = new System.Drawing.Size(237, 24);
			this.comboBoxTemplates.TabIndex = 26;
			this.comboBoxTemplates.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemplates_SelectedIndexChanged);
			// 
			// btnCancel
			// 
			this.btnCancel.AutoSize = true;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.Location = new System.Drawing.Point(136, 48);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(115, 31);
			this.btnCancel.TabIndex = 32;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// TemplateDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(264, 108);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.comboBoxTemplates);
			this.Controls.Add(this.buttonAddToPreview);
			this.Name = "TemplateDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			this.Text = "TemplateDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Button buttonAddToPreview;
		private System.Windows.Forms.ComboBox comboBoxTemplates;
		private System.Windows.Forms.Button btnCancel;
	}
}
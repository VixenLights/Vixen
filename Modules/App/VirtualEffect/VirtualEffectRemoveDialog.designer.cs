namespace VixenModules.App.VirtualEffect
{
	partial class VirtualEffectRemoveDialog
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
			this.listViewVirtualEffects = new System.Windows.Forms.ListView();
			this.buttonRemove = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.fontDialog1 = new System.Windows.Forms.FontDialog();
			this.SuspendLayout();
			// 
			// listViewVirtualEffects
			// 
			this.listViewVirtualEffects.Location = new System.Drawing.Point(12, 12);
			this.listViewVirtualEffects.Name = "listViewVirtualEffects";
			this.listViewVirtualEffects.Size = new System.Drawing.Size(260, 184);
			this.listViewVirtualEffects.TabIndex = 0;
			this.listViewVirtualEffects.UseCompatibleStateImageBehavior = false;
			this.listViewVirtualEffects.View = System.Windows.Forms.View.List;
			this.listViewVirtualEffects.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewVirtualEffects_ItemSelectionChanged);
			// 
			// buttonRemove
			// 
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Location = new System.Drawing.Point(197, 226);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.Size = new System.Drawing.Size(75, 23);
			this.buttonRemove.TabIndex = 1;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(116, 226);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// VirtualEffectRemoveDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonRemove);
			this.Controls.Add(this.listViewVirtualEffects);
			this.Name = "VirtualEffectRemoveDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Remove Virtual Effect";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listViewVirtualEffects;
		private System.Windows.Forms.Button buttonRemove;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.FontDialog fontDialog1;
	}
}
namespace VixenModules.App.ColorGradients
{
	partial class ColorGradientLibrarySelector
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
		private void InitializeComponent()
		{
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.listViewColorGradients = new System.Windows.Forms.ListView();
			this.buttonEditColorGradient = new System.Windows.Forms.Button();
			this.buttonDeleteColorGradient = new System.Windows.Forms.Button();
			this.buttonNewColorGradient = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.BackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonCancel.Location = new System.Drawing.Point(450, 315);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(93, 29);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = false;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.BackColor = System.Drawing.Color.Transparent;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonOK.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonOK.Location = new System.Drawing.Point(350, 315);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(93, 29);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = false;
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// listViewColorGradients
			// 
			this.listViewColorGradients.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listViewColorGradients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.listViewColorGradients.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.listViewColorGradients.Location = new System.Drawing.Point(14, 14);
			this.listViewColorGradients.Name = "listViewColorGradients";
			this.listViewColorGradients.Size = new System.Drawing.Size(529, 287);
			this.listViewColorGradients.TabIndex = 6;
			this.listViewColorGradients.UseCompatibleStateImageBehavior = false;
			this.listViewColorGradients.SelectedIndexChanged += new System.EventHandler(this.listViewColorGradients_SelectedIndexChanged);
			this.listViewColorGradients.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewColorGradients_MouseDoubleClick);
			// 
			// buttonEditColorGradient
			// 
			this.buttonEditColorGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonEditColorGradient.BackColor = System.Drawing.Color.Transparent;
			this.buttonEditColorGradient.Enabled = false;
			this.buttonEditColorGradient.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonEditColorGradient.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonEditColorGradient.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonEditColorGradient.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonEditColorGradient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonEditColorGradient.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonEditColorGradient.Location = new System.Drawing.Point(126, 315);
			this.buttonEditColorGradient.Name = "buttonEditColorGradient";
			this.buttonEditColorGradient.Size = new System.Drawing.Size(105, 29);
			this.buttonEditColorGradient.TabIndex = 7;
			this.buttonEditColorGradient.Text = "Edit Gradient";
			this.buttonEditColorGradient.UseVisualStyleBackColor = false;
			this.buttonEditColorGradient.Click += new System.EventHandler(this.buttonEditColorGradient_Click);
			this.buttonEditColorGradient.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonEditColorGradient.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonDeleteColorGradient
			// 
			this.buttonDeleteColorGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDeleteColorGradient.BackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteColorGradient.Enabled = false;
			this.buttonDeleteColorGradient.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonDeleteColorGradient.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteColorGradient.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteColorGradient.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonDeleteColorGradient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonDeleteColorGradient.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonDeleteColorGradient.Location = new System.Drawing.Point(238, 315);
			this.buttonDeleteColorGradient.Name = "buttonDeleteColorGradient";
			this.buttonDeleteColorGradient.Size = new System.Drawing.Size(105, 29);
			this.buttonDeleteColorGradient.TabIndex = 8;
			this.buttonDeleteColorGradient.Text = "Delete Gradient";
			this.buttonDeleteColorGradient.UseVisualStyleBackColor = false;
			this.buttonDeleteColorGradient.Click += new System.EventHandler(this.buttonDeleteColorGradient_Click);
			this.buttonDeleteColorGradient.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonDeleteColorGradient.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonNewColorGradient
			// 
			this.buttonNewColorGradient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonNewColorGradient.BackColor = System.Drawing.Color.Transparent;
			this.buttonNewColorGradient.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonNewColorGradient.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonNewColorGradient.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonNewColorGradient.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonNewColorGradient.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonNewColorGradient.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.buttonNewColorGradient.Location = new System.Drawing.Point(14, 315);
			this.buttonNewColorGradient.Name = "buttonNewColorGradient";
			this.buttonNewColorGradient.Size = new System.Drawing.Size(105, 29);
			this.buttonNewColorGradient.TabIndex = 9;
			this.buttonNewColorGradient.Text = "New Gradient";
			this.buttonNewColorGradient.UseVisualStyleBackColor = false;
			this.buttonNewColorGradient.Click += new System.EventHandler(this.buttonNewColorGradient_Click);
			this.buttonNewColorGradient.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonNewColorGradient.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// ColorGradientLibrarySelector
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(558, 359);
			this.Controls.Add(this.buttonNewColorGradient);
			this.Controls.Add(this.buttonDeleteColorGradient);
			this.Controls.Add(this.buttonEditColorGradient);
			this.Controls.Add(this.listViewColorGradients);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(574, 397);
			this.Name = "ColorGradientLibrarySelector";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Gradient Library";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ColorGradientLibrarySelector_FormClosing);
			this.Load += new System.EventHandler(this.ColorGradientLibrarySelector_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ColorGradientLibrarySelector_KeyDown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ListView listViewColorGradients;
		private System.Windows.Forms.Button buttonEditColorGradient;
		private System.Windows.Forms.Button buttonDeleteColorGradient;
		private System.Windows.Forms.Button buttonNewColorGradient;
	}
}
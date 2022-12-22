namespace VixenModules.App.ColorGradients
{
	partial class ColorGradientEditor
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
			this.groupBoxLibrary = new System.Windows.Forms.GroupBox();
			this.buttonEditLibraryItem = new System.Windows.Forms.Button();
			this.buttonUnlink = new System.Windows.Forms.Button();
			this.labelCurve = new System.Windows.Forms.Label();
			this.buttonSaveToLibrary = new System.Windows.Forms.Button();
			this.buttonLoadFromLibrary = new System.Windows.Forms.Button();
			this.gradientEditPanel = new VixenModules.App.ColorGradients.GradientEditPanel();
			this.groupBoxLibrary.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.AutoSize = true;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonCancel.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonCancel.Location = new System.Drawing.Point(399, 270);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(93, 29);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonCancel.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.AutoSize = true;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonOK.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonOK.Location = new System.Drawing.Point(299, 270);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(93, 29);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonOK.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// groupBoxLibrary
			// 
			this.groupBoxLibrary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBoxLibrary.Controls.Add(this.buttonEditLibraryItem);
			this.groupBoxLibrary.Controls.Add(this.buttonUnlink);
			this.groupBoxLibrary.Controls.Add(this.labelCurve);
			this.groupBoxLibrary.Controls.Add(this.buttonSaveToLibrary);
			this.groupBoxLibrary.Controls.Add(this.buttonLoadFromLibrary);
			this.groupBoxLibrary.Location = new System.Drawing.Point(14, 159);
			this.groupBoxLibrary.Name = "groupBoxLibrary";
			this.groupBoxLibrary.Size = new System.Drawing.Size(478, 98);
			this.groupBoxLibrary.TabIndex = 9;
			this.groupBoxLibrary.TabStop = false;
			this.groupBoxLibrary.Text = "Library";
			this.groupBoxLibrary.Paint += new System.Windows.Forms.PaintEventHandler(this.groupBoxes_Paint);
			// 
			// buttonEditLibraryItem
			// 
			this.buttonEditLibraryItem.AutoSize = true;
			this.buttonEditLibraryItem.BackColor = System.Drawing.Color.Transparent;
			this.buttonEditLibraryItem.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonEditLibraryItem.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonEditLibraryItem.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonEditLibraryItem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonEditLibraryItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonEditLibraryItem.Location = new System.Drawing.Point(247, 58);
			this.buttonEditLibraryItem.Name = "buttonEditLibraryItem";
			this.buttonEditLibraryItem.Size = new System.Drawing.Size(128, 29);
			this.buttonEditLibraryItem.TabIndex = 4;
			this.buttonEditLibraryItem.Text = "Edit Library Gradient";
			this.buttonEditLibraryItem.UseVisualStyleBackColor = false;
			this.buttonEditLibraryItem.Click += new System.EventHandler(this.buttonEditLibraryItem_Click);
			this.buttonEditLibraryItem.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonEditLibraryItem.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonUnlink
			// 
			this.buttonUnlink.AutoSize = true;
			this.buttonUnlink.BackColor = System.Drawing.Color.Transparent;
			this.buttonUnlink.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonUnlink.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonUnlink.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonUnlink.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonUnlink.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonUnlink.Location = new System.Drawing.Point(124, 58);
			this.buttonUnlink.Name = "buttonUnlink";
			this.buttonUnlink.Size = new System.Drawing.Size(117, 29);
			this.buttonUnlink.TabIndex = 3;
			this.buttonUnlink.Text = "Unlink Gradient";
			this.buttonUnlink.UseVisualStyleBackColor = false;
			this.buttonUnlink.Click += new System.EventHandler(this.buttonUnlink_Click);
			this.buttonUnlink.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonUnlink.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// labelCurve
			// 
			this.labelCurve.AutoSize = true;
			this.labelCurve.Location = new System.Drawing.Point(120, 29);
			this.labelCurve.Name = "labelCurve";
			this.labelCurve.Size = new System.Drawing.Size(277, 15);
			this.labelCurve.TabIndex = 2;
			this.labelCurve.Text = "This curve is linked to the library curve: \'ASDFASDF\'";
			// 
			// buttonSaveToLibrary
			// 
			this.buttonSaveToLibrary.AutoSize = true;
			this.buttonSaveToLibrary.BackColor = System.Drawing.Color.Transparent;
			this.buttonSaveToLibrary.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonSaveToLibrary.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonSaveToLibrary.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonSaveToLibrary.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonSaveToLibrary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonSaveToLibrary.Location = new System.Drawing.Point(14, 58);
			this.buttonSaveToLibrary.Name = "buttonSaveToLibrary";
			this.buttonSaveToLibrary.Size = new System.Drawing.Size(93, 29);
			this.buttonSaveToLibrary.TabIndex = 1;
			this.buttonSaveToLibrary.Text = "Save Preset";
			this.buttonSaveToLibrary.UseVisualStyleBackColor = false;
			this.buttonSaveToLibrary.Click += new System.EventHandler(this.buttonSaveToLibrary_Click);
			this.buttonSaveToLibrary.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonSaveToLibrary.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// buttonLoadFromLibrary
			// 
			this.buttonLoadFromLibrary.AutoSize = true;
			this.buttonLoadFromLibrary.BackColor = System.Drawing.Color.Transparent;
			this.buttonLoadFromLibrary.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonLoadFromLibrary.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
			this.buttonLoadFromLibrary.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
			this.buttonLoadFromLibrary.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
			this.buttonLoadFromLibrary.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonLoadFromLibrary.Location = new System.Drawing.Point(14, 22);
			this.buttonLoadFromLibrary.Name = "buttonLoadFromLibrary";
			this.buttonLoadFromLibrary.Size = new System.Drawing.Size(93, 29);
			this.buttonLoadFromLibrary.TabIndex = 0;
			this.buttonLoadFromLibrary.Text = "Load Preset";
			this.buttonLoadFromLibrary.UseVisualStyleBackColor = false;
			this.buttonLoadFromLibrary.Click += new System.EventHandler(this.buttonLoadFromLibrary_Click);
			this.buttonLoadFromLibrary.MouseLeave += new System.EventHandler(this.buttonBackground_MouseLeave);
			this.buttonLoadFromLibrary.MouseHover += new System.EventHandler(this.buttonBackground_MouseHover);
			// 
			// gradientEditPanel
			// 
			this.gradientEditPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gradientEditPanel.AutoSize = true;
			this.gradientEditPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.gradientEditPanel.DiscreteColors = false;
			this.gradientEditPanel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gradientEditPanel.ForeColor = System.Drawing.Color.Black;
			this.gradientEditPanel.Location = new System.Drawing.Point(14, 14);
			this.gradientEditPanel.LockColorEditorHSV_Value = false;
			this.gradientEditPanel.MinimumSize = new System.Drawing.Size(416, 138);
			this.gradientEditPanel.Name = "gradientEditPanel";
			this.gradientEditPanel.ReadOnly = false;
			this.gradientEditPanel.Size = new System.Drawing.Size(478, 138);
			this.gradientEditPanel.TabIndex = 0;
			this.gradientEditPanel.ValidDiscreteColors = null;
			// 
			// ColorGradientEditor
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(506, 314);
			this.Controls.Add(this.gradientEditPanel);
			this.Controls.Add(this.groupBoxLibrary);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(522, 352);
			this.Name = "ColorGradientEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Gradient Editor";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ColorGradientEditor_KeyDown);
			this.groupBoxLibrary.ResumeLayout(false);
			this.groupBoxLibrary.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private GradientEditPanel gradientEditPanel;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.GroupBox groupBoxLibrary;
		private System.Windows.Forms.Button buttonEditLibraryItem;
		private System.Windows.Forms.Button buttonUnlink;
		private System.Windows.Forms.Label labelCurve;
		private System.Windows.Forms.Button buttonSaveToLibrary;
		private System.Windows.Forms.Button buttonLoadFromLibrary;
	}
}
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
			this.gradientEditPanel = new VixenModules.App.ColorGradients.GradientEditPanel();
			this.groupBoxLibrary = new System.Windows.Forms.GroupBox();
			this.buttonEditLibraryItem = new System.Windows.Forms.Button();
			this.buttonUnlink = new System.Windows.Forms.Button();
			this.labelCurve = new System.Windows.Forms.Label();
			this.buttonSaveToLibrary = new System.Windows.Forms.Button();
			this.buttonLoadFromLibrary = new System.Windows.Forms.Button();
			this.groupBoxLibrary.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(342, 234);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 25);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(256, 234);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(80, 25);
			this.buttonOK.TabIndex = 4;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			// 
			// gradientEditPanel
			// 
			this.gradientEditPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gradientEditPanel.DiscreteColors = false;
			this.gradientEditPanel.Location = new System.Drawing.Point(12, 12);
			this.gradientEditPanel.LockColorEditorHSV_Value = true;
			this.gradientEditPanel.MinimumSize = new System.Drawing.Size(357, 120);
			this.gradientEditPanel.Name = "gradientEditPanel";
			this.gradientEditPanel.ReadOnly = false;
			this.gradientEditPanel.Size = new System.Drawing.Size(410, 120);
			this.gradientEditPanel.TabIndex = 0;
			this.gradientEditPanel.ValidDiscreteColors = null;
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
			this.groupBoxLibrary.Location = new System.Drawing.Point(12, 138);
			this.groupBoxLibrary.Name = "groupBoxLibrary";
			this.groupBoxLibrary.Size = new System.Drawing.Size(410, 85);
			this.groupBoxLibrary.TabIndex = 9;
			this.groupBoxLibrary.TabStop = false;
			this.groupBoxLibrary.Text = "Library";
			// 
			// buttonEditLibraryItem
			// 
			this.buttonEditLibraryItem.Location = new System.Drawing.Point(212, 50);
			this.buttonEditLibraryItem.Name = "buttonEditLibraryItem";
			this.buttonEditLibraryItem.Size = new System.Drawing.Size(110, 25);
			this.buttonEditLibraryItem.TabIndex = 4;
			this.buttonEditLibraryItem.Text = "Edit Library Gradient";
			this.buttonEditLibraryItem.UseVisualStyleBackColor = true;
			this.buttonEditLibraryItem.Click += new System.EventHandler(this.buttonEditLibraryItem_Click);
			// 
			// buttonUnlink
			// 
			this.buttonUnlink.Location = new System.Drawing.Point(106, 50);
			this.buttonUnlink.Name = "buttonUnlink";
			this.buttonUnlink.Size = new System.Drawing.Size(100, 25);
			this.buttonUnlink.TabIndex = 3;
			this.buttonUnlink.Text = "Unlink Gradient";
			this.buttonUnlink.UseVisualStyleBackColor = true;
			this.buttonUnlink.Click += new System.EventHandler(this.buttonUnlink_Click);
			// 
			// labelCurve
			// 
			this.labelCurve.AutoSize = true;
			this.labelCurve.Location = new System.Drawing.Point(103, 25);
			this.labelCurve.Name = "labelCurve";
			this.labelCurve.Size = new System.Drawing.Size(254, 13);
			this.labelCurve.TabIndex = 2;
			this.labelCurve.Text = "This curve is linked to the library curve: \'ASDFASDF\'";
			// 
			// buttonSaveToLibrary
			// 
			this.buttonSaveToLibrary.Location = new System.Drawing.Point(12, 50);
			this.buttonSaveToLibrary.Name = "buttonSaveToLibrary";
			this.buttonSaveToLibrary.Size = new System.Drawing.Size(80, 25);
			this.buttonSaveToLibrary.TabIndex = 1;
			this.buttonSaveToLibrary.Text = "Save Preset";
			this.buttonSaveToLibrary.UseVisualStyleBackColor = true;
			this.buttonSaveToLibrary.Click += new System.EventHandler(this.buttonSaveToLibrary_Click);
			// 
			// buttonLoadFromLibrary
			// 
			this.buttonLoadFromLibrary.Location = new System.Drawing.Point(12, 19);
			this.buttonLoadFromLibrary.Name = "buttonLoadFromLibrary";
			this.buttonLoadFromLibrary.Size = new System.Drawing.Size(80, 25);
			this.buttonLoadFromLibrary.TabIndex = 0;
			this.buttonLoadFromLibrary.Text = "Load Preset";
			this.buttonLoadFromLibrary.UseVisualStyleBackColor = true;
			this.buttonLoadFromLibrary.Click += new System.EventHandler(this.buttonLoadFromLibrary_Click);
			// 
			// ColorGradientEditor
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(434, 271);
			this.Controls.Add(this.groupBoxLibrary);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.gradientEditPanel);
			this.KeyPreview = true;
			this.Name = "ColorGradientEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Color Gradient Editor";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ColorGradientEditor_KeyDown);
			this.groupBoxLibrary.ResumeLayout(false);
			this.groupBoxLibrary.PerformLayout();
			this.ResumeLayout(false);

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
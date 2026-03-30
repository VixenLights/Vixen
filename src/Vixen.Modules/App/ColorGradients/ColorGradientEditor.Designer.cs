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
			buttonCancel = new Button();
			buttonOK = new Button();
			groupBoxLibrary = new GroupBox();
			buttonEditLibraryItem = new Button();
			buttonUnlink = new Button();
			labelCurve = new Label();
			buttonSaveToLibrary = new Button();
			buttonLoadFromLibrary = new Button();
			gradientEditPanel = new GradientEditPanel();
			groupBoxLibrary.SuspendLayout();
			SuspendLayout();
			// 
			// buttonCancel
			// 
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.AutoSize = true;
			buttonCancel.DialogResult = DialogResult.Cancel;
			buttonCancel.Location = new Point(399, 270);
			buttonCancel.Name = "buttonCancel";
			buttonCancel.Size = new Size(93, 29);
			buttonCancel.TabIndex = 5;
			buttonCancel.Text = "Cancel";
			buttonCancel.UseVisualStyleBackColor = true;
			// 
			// buttonOK
			// 
			buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonOK.AutoSize = true;
			buttonOK.DialogResult = DialogResult.OK;
			buttonOK.Location = new Point(300, 270);
			buttonOK.Name = "buttonOK";
			buttonOK.Size = new Size(93, 29);
			buttonOK.TabIndex = 4;
			buttonOK.Text = "OK";
			buttonOK.UseVisualStyleBackColor = true;
			// 
			// groupBoxLibrary
			// 
			groupBoxLibrary.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			groupBoxLibrary.Controls.Add(buttonEditLibraryItem);
			groupBoxLibrary.Controls.Add(buttonUnlink);
			groupBoxLibrary.Controls.Add(labelCurve);
			groupBoxLibrary.Controls.Add(buttonSaveToLibrary);
			groupBoxLibrary.Controls.Add(buttonLoadFromLibrary);
			groupBoxLibrary.Location = new Point(14, 159);
			groupBoxLibrary.Name = "groupBoxLibrary";
			groupBoxLibrary.Size = new Size(478, 98);
			groupBoxLibrary.TabIndex = 9;
			groupBoxLibrary.TabStop = false;
			groupBoxLibrary.Text = "Library";
			groupBoxLibrary.Paint += groupBoxes_Paint;
			// 
			// buttonEditLibraryItem
			// 
			buttonEditLibraryItem.AutoSize = true;
			buttonEditLibraryItem.BackColor = Color.Transparent;
			buttonEditLibraryItem.Location = new Point(247, 58);
			buttonEditLibraryItem.Name = "buttonEditLibraryItem";
			buttonEditLibraryItem.Size = new Size(128, 29);
			buttonEditLibraryItem.TabIndex = 4;
			buttonEditLibraryItem.Text = "Edit Library Gradient";
			buttonEditLibraryItem.UseVisualStyleBackColor = false;
			buttonEditLibraryItem.Click += buttonEditLibraryItem_Click;
			// 
			// buttonUnlink
			// 
			buttonUnlink.AutoSize = true;
			buttonUnlink.BackColor = Color.Transparent;
			buttonUnlink.Location = new Point(124, 58);
			buttonUnlink.Name = "buttonUnlink";
			buttonUnlink.Size = new Size(117, 29);
			buttonUnlink.TabIndex = 3;
			buttonUnlink.Text = "Unlink Gradient";
			buttonUnlink.UseVisualStyleBackColor = false;
			buttonUnlink.Click += buttonUnlink_Click;
			// 
			// labelCurve
			// 
			labelCurve.AutoSize = true;
			labelCurve.Location = new Point(120, 29);
			labelCurve.Name = "labelCurve";
			labelCurve.Size = new Size(276, 15);
			labelCurve.TabIndex = 2;
			labelCurve.Text = "This curve is linked to the library curve: 'ASDFASDF'";
			// 
			// buttonSaveToLibrary
			// 
			buttonSaveToLibrary.AutoSize = true;
			buttonSaveToLibrary.Location = new Point(14, 58);
			buttonSaveToLibrary.Name = "buttonSaveToLibrary";
			buttonSaveToLibrary.Size = new Size(93, 29);
			buttonSaveToLibrary.TabIndex = 1;
			buttonSaveToLibrary.Text = "Save Preset";
			buttonSaveToLibrary.UseVisualStyleBackColor = false;
			buttonSaveToLibrary.Click += buttonSaveToLibrary_Click;
			// 
			// buttonLoadFromLibrary
			// 
			buttonLoadFromLibrary.AutoSize = true;
			buttonLoadFromLibrary.Location = new Point(14, 22);
			buttonLoadFromLibrary.Name = "buttonLoadFromLibrary";
			buttonLoadFromLibrary.Size = new Size(93, 29);
			buttonLoadFromLibrary.TabIndex = 0;
			buttonLoadFromLibrary.Text = "Load Preset";
			buttonLoadFromLibrary.UseVisualStyleBackColor = false;
			buttonLoadFromLibrary.Click += buttonLoadFromLibrary_Click;
			// 
			// gradientEditPanel
			// 
			gradientEditPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			gradientEditPanel.AutoSize = true;
			gradientEditPanel.BackColor = Color.FromArgb(68, 68, 68);
			gradientEditPanel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			gradientEditPanel.ForeColor = Color.FromArgb(235, 235, 235);
			gradientEditPanel.Location = new Point(14, 14);
			gradientEditPanel.Margin = new Padding(4, 3, 4, 3);
			gradientEditPanel.MinimumSize = new Size(416, 138);
			gradientEditPanel.Name = "gradientEditPanel";
			gradientEditPanel.Size = new Size(478, 138);
			gradientEditPanel.TabIndex = 0;
			// 
			// ColorGradientEditor
			// 
			AcceptButton = buttonOK;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			BackColor = Color.FromArgb(68, 68, 68);
			CancelButton = buttonCancel;
			ClientSize = new Size(506, 314);
			Controls.Add(gradientEditPanel);
			Controls.Add(groupBoxLibrary);
			Controls.Add(buttonCancel);
			Controls.Add(buttonOK);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			KeyPreview = true;
			MinimumSize = new Size(522, 352);
			Name = "ColorGradientEditor";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Color Gradient Editor";
			KeyDown += ColorGradientEditor_KeyDown;
			groupBoxLibrary.ResumeLayout(false);
			groupBoxLibrary.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

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
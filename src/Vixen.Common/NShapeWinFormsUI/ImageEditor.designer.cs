namespace Dataweb.NShape.WinFormsUI {
	partial class ImageEditor {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
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
		private void InitializeComponent() {
			this.browseButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.nameTextBox = new System.Windows.Forms.TextBox();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.clearButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// browseButton
			// 
			this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.browseButton.Location = new System.Drawing.Point(358, 309);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(24, 23);
			this.browseButton.TabIndex = 0;
			this.browseButton.Text = "...";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(277, 350);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(358, 350);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox.BackColor = System.Drawing.Color.White;
			this.pictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(12, 12);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(421, 291);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox.TabIndex = 3;
			this.pictureBox.TabStop = false;
			// 
			// nameTextBox
			// 
			this.nameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.nameTextBox.Location = new System.Drawing.Point(12, 311);
			this.nameTextBox.Name = "nameTextBox";
			this.nameTextBox.Size = new System.Drawing.Size(340, 20);
			this.nameTextBox.TabIndex = 3;
			this.nameTextBox.TextChanged += new System.EventHandler(this.nameTextBox_TextChanged);
			// 
			// clearButton
			// 
			this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.clearButton.Location = new System.Drawing.Point(388, 309);
			this.clearButton.Name = "clearButton";
			this.clearButton.Size = new System.Drawing.Size(45, 23);
			this.clearButton.TabIndex = 6;
			this.clearButton.Text = "ClearStyles";
			this.clearButton.UseVisualStyleBackColor = true;
			this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
			// 
			// ImageEditor
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(445, 385);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.clearButton);
			this.Controls.Add(this.browseButton);
			this.Controls.Add(this.nameTextBox);
			this.Controls.Add(this.pictureBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "ImageEditor";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Select Image";
			this.TopMost = true;
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.TextBox nameTextBox;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.Button clearButton;
	}
}
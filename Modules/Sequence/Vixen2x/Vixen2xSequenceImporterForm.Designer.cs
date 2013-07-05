namespace VixenModules.SequenceType.Vixen2x
{
	partial class Vixen2xSequenceImporterForm
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
			this.sequenceToConvertLabel = new System.Windows.Forms.Label();
			this.vixen2SequenceTextBox = new System.Windows.Forms.TextBox();
			this.vixen2ProfileTextBox = new System.Windows.Forms.TextBox();
			this.vixen2ProfileLabel = new System.Windows.Forms.Label();
			this.createMapButton = new System.Windows.Forms.Button();
			this.covertButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// sequenceToConvertLabel
			// 
			this.sequenceToConvertLabel.AutoSize = true;
			this.sequenceToConvertLabel.Location = new System.Drawing.Point(7, 19);
			this.sequenceToConvertLabel.Name = "sequenceToConvertLabel";
			this.sequenceToConvertLabel.Size = new System.Drawing.Size(97, 13);
			this.sequenceToConvertLabel.TabIndex = 4;
			this.sequenceToConvertLabel.Text = "Vixen 2 Sequence:";
			// 
			// vixen2SequenceTextBox
			// 
			this.vixen2SequenceTextBox.Location = new System.Drawing.Point(110, 12);
			this.vixen2SequenceTextBox.Name = "vixen2SequenceTextBox";
			this.vixen2SequenceTextBox.ReadOnly = true;
			this.vixen2SequenceTextBox.Size = new System.Drawing.Size(612, 20);
			this.vixen2SequenceTextBox.TabIndex = 5;
			// 
			// vixen2ProfileTextBox
			// 
			this.vixen2ProfileTextBox.Location = new System.Drawing.Point(110, 38);
			this.vixen2ProfileTextBox.Name = "vixen2ProfileTextBox";
			this.vixen2ProfileTextBox.ReadOnly = true;
			this.vixen2ProfileTextBox.Size = new System.Drawing.Size(612, 20);
			this.vixen2ProfileTextBox.TabIndex = 8;
			// 
			// vixen2ProfileLabel
			// 
			this.vixen2ProfileLabel.AutoSize = true;
			this.vixen2ProfileLabel.Location = new System.Drawing.Point(30, 45);
			this.vixen2ProfileLabel.Name = "vixen2ProfileLabel";
			this.vixen2ProfileLabel.Size = new System.Drawing.Size(74, 13);
			this.vixen2ProfileLabel.TabIndex = 7;
			this.vixen2ProfileLabel.Text = "Vixen 2 Profile";
			// 
			// createMapButton
			// 
			this.createMapButton.Location = new System.Drawing.Point(12, 70);
			this.createMapButton.Name = "createMapButton";
			this.createMapButton.Size = new System.Drawing.Size(75, 23);
			this.createMapButton.TabIndex = 13;
			this.createMapButton.Text = "Create Map";
			this.createMapButton.UseVisualStyleBackColor = true;
			this.createMapButton.Click += new System.EventHandler(this.createMapButton_Click);
			// 
			// covertButton
			// 
			this.covertButton.Enabled = false;
			this.covertButton.Location = new System.Drawing.Point(93, 70);
			this.covertButton.Name = "covertButton";
			this.covertButton.Size = new System.Drawing.Size(75, 23);
			this.covertButton.TabIndex = 14;
			this.covertButton.Text = "Convert";
			this.covertButton.UseVisualStyleBackColor = true;
			this.covertButton.Click += new System.EventHandler(this.covertButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(174, 70);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 15;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// Vixen2xSequenceImporterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(732, 101);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.covertButton);
			this.Controls.Add(this.createMapButton);
			this.Controls.Add(this.vixen2ProfileTextBox);
			this.Controls.Add(this.vixen2ProfileLabel);
			this.Controls.Add(this.vixen2SequenceTextBox);
			this.Controls.Add(this.sequenceToConvertLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MinimizeBox = false;
			this.Name = "Vixen2xSequenceImporterForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Vixen 2 to Vixen 3 Converter";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.Label sequenceToConvertLabel;
        private System.Windows.Forms.TextBox vixen2SequenceTextBox;
        private System.Windows.Forms.TextBox vixen2ProfileTextBox;
        private System.Windows.Forms.Label vixen2ProfileLabel;
        private System.Windows.Forms.Button createMapButton;
        private System.Windows.Forms.Button covertButton;
		private System.Windows.Forms.Button cancelButton;
	}
}
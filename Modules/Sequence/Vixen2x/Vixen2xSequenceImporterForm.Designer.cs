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
			this.vixen2ProfileLabel = new System.Windows.Forms.Label();
			this.createMapButton = new System.Windows.Forms.Button();
			this.convertButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.vixen2ToVixen3MappingListBox = new System.Windows.Forms.ListBox();
			this.vixen2ProfileTextBox = new System.Windows.Forms.TextBox();
			this.vixen2ToVixen3MappingTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// sequenceToConvertLabel
			// 
			this.sequenceToConvertLabel.AutoSize = true;
			this.sequenceToConvertLabel.Location = new System.Drawing.Point(9, 9);
			this.sequenceToConvertLabel.Name = "sequenceToConvertLabel";
			this.sequenceToConvertLabel.Size = new System.Drawing.Size(97, 13);
			this.sequenceToConvertLabel.TabIndex = 4;
			this.sequenceToConvertLabel.Text = "Vixen 2 Sequence:";
			// 
			// vixen2SequenceTextBox
			// 
			this.vixen2SequenceTextBox.Location = new System.Drawing.Point(12, 25);
			this.vixen2SequenceTextBox.Name = "vixen2SequenceTextBox";
			this.vixen2SequenceTextBox.ReadOnly = true;
			this.vixen2SequenceTextBox.Size = new System.Drawing.Size(442, 20);
			this.vixen2SequenceTextBox.TabIndex = 5;
			// 
			// vixen2ProfileLabel
			// 
			this.vixen2ProfileLabel.AutoSize = true;
			this.vixen2ProfileLabel.Location = new System.Drawing.Point(9, 48);
			this.vixen2ProfileLabel.Name = "vixen2ProfileLabel";
			this.vixen2ProfileLabel.Size = new System.Drawing.Size(77, 13);
			this.vixen2ProfileLabel.TabIndex = 7;
			this.vixen2ProfileLabel.Text = "Vixen 2 Profile:";
			// 
			// createMapButton
			// 
			this.createMapButton.Location = new System.Drawing.Point(365, 141);
			this.createMapButton.Name = "createMapButton";
			this.createMapButton.Size = new System.Drawing.Size(75, 23);
			this.createMapButton.TabIndex = 13;
			this.createMapButton.Text = "Create Map";
			this.createMapButton.UseVisualStyleBackColor = true;
			this.createMapButton.Click += new System.EventHandler(this.createMapButton_Click);
			// 
			// convertButton
			// 
			this.convertButton.Enabled = false;
			this.convertButton.Location = new System.Drawing.Point(446, 141);
			this.convertButton.Name = "convertButton";
			this.convertButton.Size = new System.Drawing.Size(75, 23);
			this.convertButton.TabIndex = 14;
			this.convertButton.Text = "Convert";
			this.convertButton.UseVisualStyleBackColor = true;
			this.convertButton.Click += new System.EventHandler(this.convertButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(527, 141);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 15;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 87);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(119, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "Vixen 2 to Vixen 3 Map:";
			// 
			// vixen2ToVixen3MappingListBox
			// 
			this.vixen2ToVixen3MappingListBox.FormattingEnabled = true;
			this.vixen2ToVixen3MappingListBox.Location = new System.Drawing.Point(482, 28);
			this.vixen2ToVixen3MappingListBox.Name = "vixen2ToVixen3MappingListBox";
			this.vixen2ToVixen3MappingListBox.Size = new System.Drawing.Size(120, 95);
			this.vixen2ToVixen3MappingListBox.TabIndex = 17;
			this.vixen2ToVixen3MappingListBox.SelectedIndexChanged += new System.EventHandler(this.vixen2ToVixen3MappingListBox_SelectedIndexChanged);
			// 
			// vixen2ProfileTextBox
			// 
			this.vixen2ProfileTextBox.Location = new System.Drawing.Point(12, 64);
			this.vixen2ProfileTextBox.Name = "vixen2ProfileTextBox";
			this.vixen2ProfileTextBox.ReadOnly = true;
			this.vixen2ProfileTextBox.Size = new System.Drawing.Size(442, 20);
			this.vixen2ProfileTextBox.TabIndex = 8;
			// 
			// vixen2ToVixen3MappingTextBox
			// 
			this.vixen2ToVixen3MappingTextBox.Location = new System.Drawing.Point(12, 103);
			this.vixen2ToVixen3MappingTextBox.Name = "vixen2ToVixen3MappingTextBox";
			this.vixen2ToVixen3MappingTextBox.ReadOnly = true;
			this.vixen2ToVixen3MappingTextBox.Size = new System.Drawing.Size(442, 20);
			this.vixen2ToVixen3MappingTextBox.TabIndex = 18;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(479, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 13);
			this.label2.TabIndex = 19;
			this.label2.Text = "Available Maps";
			// 
			// Vixen2xSequenceImporterForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(621, 178);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.vixen2ToVixen3MappingTextBox);
			this.Controls.Add(this.vixen2ToVixen3MappingListBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.convertButton);
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
        private System.Windows.Forms.Label vixen2ProfileLabel;
        private System.Windows.Forms.Button createMapButton;
        private System.Windows.Forms.Button convertButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox vixen2ToVixen3MappingListBox;
		private System.Windows.Forms.TextBox vixen2ProfileTextBox;
		private System.Windows.Forms.TextBox vixen2ToVixen3MappingTextBox;
		private System.Windows.Forms.Label label2;
	}
}